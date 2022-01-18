using Emulators.LibRetro.Controllers;
using Emulators.LibRetro.Controllers.Mapping;
using Emulators.LibRetro.Render;
using Emulators.LibRetro.Settings;
using Emulators.LibRetro.SoundProviders;
using Emulators.LibRetro.VideoProviders;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.Settings;
using MediaPortal.UI.SkinEngine.Players;
using MediaPortal.UI.SkinEngine.SkinManagement;
using SharpDX.Direct3D9;
using SharpRetro.Audio;
using SharpRetro.LibRetro;
using SharpRetro.State;
using System;
using System.Collections.Generic;
using System.IO;

namespace Emulators.LibRetro
{
  public class LibRetroFrontend : IDisposable
  {
    #region ILogger
    protected static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
    #endregion

    #region Protected Members
    protected LibRetroSettings _settings;
    protected LibRetroThread _retroThread;
    protected LibRetroEmulator _retroEmulator;
    protected LibRetroSaveStateHandler _saveHandler;
    protected SerializedStateBuffer _stateBuffer;
    protected ISoundOutput _soundOutput;
    protected TextureOutput _textureOutput;
    protected ControllerWrapper _controllerWrapper;
    protected SynchronizationStrategy _synchronizationStrategy;    
    protected RenderDlgt _renderDlgt;    
    protected bool _guiInitialized;
    protected bool _isPaused;

    protected string _corePath;
    protected string _gamePath;
    protected string _saveName;
    protected bool _autoSave;
    protected double _targetFps;
    #endregion

    #region Ctor
    public LibRetroFrontend(string corePath, string gamePath, string saveName)
    {
      _corePath = corePath;
      _gamePath = gamePath;
      _saveName = saveName;
      _guiInitialized = true;
    }
    #endregion

    #region Public Properties

    public TextureOutput TextureOutput
    {
      get { return _textureOutput; }
    }

    public object SurfaceLock
    {
      get { return _textureOutput?.SurfaceLock; }
    }

    public bool Paused
    {
      get { return _isPaused; }
    }

    public Texture Texture
    {
      get { return _textureOutput?.Texture; }
    }
    #endregion

    #region Public Methods
    public bool Init()
    {
      _settings = ServiceRegistration.Get<ISettingsManager>().Load<LibRetroSettings>();
      InitializeControllerWrapper(); 
      _retroThread = new LibRetroThread();
      _retroThread.Initializing += RetroThreadInitializing;
      _retroThread.Started += RetroThreadStarted;
      _retroThread.Running += RetroThreadRunning;
      _retroThread.Finishing += RetroThreadFinishing;
      _retroThread.Finished += RetroThreadFinished;
      _retroThread.Paused += RetroThreadPaused;
      _retroThread.UnPaused += RetroThreadUnPaused;
      return _retroThread.Init();
    }

    public void Run()
    {
      if (_retroThread == null || !_retroThread.IsInit)
        return;
      _retroThread.Run();
    }

    public void Pause()
    {
      if (_isPaused)
        return;
      _isPaused = true;
      if (_retroThread != null)
        _retroThread.Pause();
    }

    public void Unpause()
    {
      if (!_isPaused)
        return;
      _isPaused = false;
      if (_retroThread != null)
        _retroThread.UnPause();
    }

    public void SetVolume(int volume)
    {
      _soundOutput.SetVolume(volume);
    }

    public bool SetRenderDelegate(RenderDlgt dlgt)
    {
      _renderDlgt = dlgt;
      return true;
    }

    public void ReallocGUIResources()
    {
      _textureOutput?.Reallocate();
      _synchronizationStrategy.Update();
    }

    public void ReleaseGUIResources()
    {
      _textureOutput?.Release();
    }

    public void SetStateIndex(int index)
    {
      if (_saveHandler != null)
        _saveHandler.StateIndex = index;
    }

    public void LoadState()
    {
      if (_retroThread != null && _saveHandler != null)
        _retroThread.EnqueueAction(_saveHandler.LoadState);
    }

    public void SaveState()
    {
      if (_retroThread != null && _saveHandler != null)
        _retroThread.EnqueueAction(_saveHandler.SaveState);
    }

    public TimeSpan BufferDuration
    {
      get { return _stateBuffer?.Duration ?? TimeSpan.Zero; }
    }

    public TimeSpan BufferedTime
    {
      get { return _stateBuffer?.BufferedTime ?? TimeSpan.Zero; }
    }

    public double TargetFps
    {
      get { return _targetFps; }
    }

    public void SetTime(TimeSpan time)
    {
      if (_retroThread != null && _stateBuffer != null)
        _retroThread.EnqueueAction(() => _stateBuffer.SetCurrentPosition(time));
    }

    #endregion

    #region Init
    protected void InitializeAll()
    {
      try
      {
        InitializeLibRetro();       
        
        if (!LoadGame())
          return;

        _targetFps = _retroEmulator.TimingInfo.FPS;
        _synchronizationStrategy = new SynchronizationStrategy(_retroEmulator.TimingInfo.FPS, _settings.SynchronizationType);
        _soundOutput.SetSynchronizationStrategy(_synchronizationStrategy);

        InitializeStateBuffer();
        InitializeSaveStateHandler();
        _retroThread.IsInit = true;
      }
      catch (Exception ex)
      {
        Logger.Error("LibRetroFrontend: Error initialising Libretro core", ex);
        if (_retroEmulator != null)
        {
          _retroEmulator.Dispose();
          _retroEmulator = null;
        }
      }
    }

    protected void InitializeLibRetro()
    {
      _textureOutput = new TextureOutput(SkinContext.Device);
      //_soundOutput = new LibRetroDirectSound(SkinContext.Form.Handle, _settings.AudioDeviceId, _settings.AudioBufferSize);
      _soundOutput = new LibRetroXAudio(_settings.AudioDeviceId, _settings.EnableAudioRateControl ? _settings.AudioRateControlDelta : -1);
      _retroEmulator = new LibRetroEmulator(_corePath)
      {
        SaveDirectory = _settings.SavesDirectory,
        SystemDirectory = _settings.SystemDirectory,
        LogDelegate = RetroLogDlgt,
        Controller = _controllerWrapper,
        AudioOutput = _soundOutput as IAudioOutput,
        VideoOutput = _textureOutput
      };

      SetCoreVariables();
      _retroEmulator.Init();
      Logger.Debug("LibRetroFrontend: Libretro initialized");
    }

    protected void InitializeStateBuffer()
    {
      if (!_settings.EnableStateBuffer || _settings.StateBufferDurationSeconds <= 0)
        return;

      bool allocated = true;
      try
      {
        // Check if core supports serialization
        if (_retroEmulator.Core.SerializeSize() == 0)
        {
          Logger.Info("LibRetroFrontend: Not creating state buffer, core does not support serialization.");
          return;
        }
        _stateBuffer = new SerializedStateBuffer(_retroEmulator.Core, (int)Math.Ceiling(_retroEmulator.TimingInfo.FPS), 1, TimeSpan.FromSeconds(_settings.StateBufferDurationSeconds));
        _stateBuffer.Allocate();
      }
      catch (OutOfMemoryException)
      {
        allocated = false;
        Logger.Error("LibRetroFrontend: Cannot create state buffer with length {0} seconds, there is not enough system memory available.", _settings.StateBufferDurationSeconds);
      }
      catch (Exception ex)
      {
        allocated = false;
        Logger.Error("LibRetroFrontend: Error creating state buffer", ex);
      }

      if (!allocated)
      {
        _stateBuffer.Dispose();
        _stateBuffer = null;
      }
    }

    protected void InitializeSaveStateHandler()
    {
      _autoSave = _settings.AutoSave;
      _saveHandler = new LibRetroSaveStateHandler(_retroEmulator, _saveName, _settings.SavesDirectory, _settings.AutoSaveInterval);
      _saveHandler.LoadSaveRam();
      Logger.Debug("LibRetroFrontend: Save handler Initialized");
    }

    protected void SetCoreVariables()
    {
      var sm = ServiceRegistration.Get<ISettingsManager>();
      CoreSetting coreSetting;
      if (!sm.Load<LibRetroCoreSettings>().TryGetCoreSetting(_corePath, out coreSetting) || coreSetting.Variables == null)
        return;
      foreach (VariableDescription variable in coreSetting.Variables)
        _retroEmulator.Variables.AddOrUpdate(variable);
    }

    protected void InitializeControllerWrapper()
    {
      _controllerWrapper = new ControllerWrapper(_settings.MaxPlayers);
      DeviceProxy deviceProxy = new DeviceProxy();
      List<IMappableDevice> deviceList = deviceProxy.GetDevices(false);
      MappingProxy mappingProxy = new MappingProxy();

      foreach (PortMapping port in mappingProxy.PortMappings)
      {
        IMappableDevice device = deviceProxy.GetDevice(port.DeviceId, port.SubDeviceId, deviceList);
        if (device != null)
        {
          RetroPadMapping mapping = mappingProxy.GetDeviceMapping(device);
          device.Map(mapping);
          _controllerWrapper.AddController(device, port.Port);
          Logger.Debug("LibRetroFrontend: Mapped controller {0} to port {1}", device.DeviceName, port.Port);
        }
      }
    }

    protected bool LoadGame()
    {
      bool result;
      //A core can support running without a game as well as with a game.
      //There is currently no way to check which case is needed as we currently use a dummy game
      //to import/load standalone cores.
      //Checking ValidExtensions is currently a hack which works better than nothing
      if (_retroEmulator.SupportsNoGame && _retroEmulator.SystemInfo.ValidExtensions == null)
      {
        Logger.Debug("LibRetroFrontend: Loading no game");
        result = _retroEmulator.LoadGame(new retro_game_info());
      }
      else
      {
        Logger.Debug("LibRetroFrontend: Loading game '{0}', NeedsFullPath: {1}", _gamePath, _retroEmulator.SystemInfo.NeedsFullPath);
        byte[] gameData = _retroEmulator.SystemInfo.NeedsFullPath ? null : File.ReadAllBytes(_gamePath);
        result = _retroEmulator.LoadGame(_gamePath, gameData);
      }
      Logger.Debug("LibRetroFrontend: Load game {0}", result ? "succeeded" : "failed");
      return result;
    }
    #endregion

    #region Retro Thread
    private void RetroThreadInitializing(object sender, EventArgs e)
    {
      InitializeAll();
    }

    private void RetroThreadStarted(object sender, EventArgs e)
    {
      _controllerWrapper.Start();
      _soundOutput.Play();
      Logger.Debug("LibRetroFrontend: Libretro thread running");
    }

    private void RetroThreadRunning(object sender, EventArgs e)
    {
      RunEmulator();
      Update();
      RenderFrame();
    }

    protected void RunEmulator()
    {
      //reset this every frame in case we get a frame without audio
      _soundOutput.HasAudio = false;
      _retroEmulator.Run();
      _stateBuffer?.AppendState();

      if (_autoSave)
        _saveHandler.AutoSave();
    }

    protected void Update()
    {
      _soundOutput.Update();
    }

    protected void RenderFrame()
    {
      RenderDlgt dlgt = _renderDlgt;
      bool wait = _synchronizationStrategy.SyncToAudio ? !_soundOutput.HasAudio : dlgt == null;
      if (dlgt != null)
        dlgt();

      if (wait)
        _synchronizationStrategy.Synchronize();
    }

    private void RetroThreadFinishing(object sender, EventArgs e)
    {
      Logger.Debug("LibRetroFrontend: Libretro thread finishing");
      _saveHandler.SaveSaveRam();
      _textureOutput.Destroy();
      _retroEmulator.UnloadGame();
      _retroEmulator.DeInit();
    }

    private void RetroThreadFinished(object sender, EventArgs e)
    {
      _retroEmulator.Dispose();
      _retroEmulator = null;
      if (_textureOutput != null)
      {
        _textureOutput.Dispose();
        _textureOutput = null;
      }
      if (_soundOutput != null)
      {
        _soundOutput.Dispose();
        _soundOutput = null;
      }
      Logger.Debug("LibRetroFrontend: Libretro thread finished");
    }

    private void RetroThreadPaused(object sender, EventArgs e)
    {
      _soundOutput.Pause();
    }

    private void RetroThreadUnPaused(object sender, EventArgs e)
    {
      _soundOutput.UnPause();
    }
    #endregion

    #region LibRetro Logging
    protected void RetroLogDlgt(RETRO_LOG_LEVEL level, string message)
    {
      string format = "LibRetro: {0}";
      switch (level)
      {
        case RETRO_LOG_LEVEL.INFO:
          Logger.Info(format, message);
          break;
        case RETRO_LOG_LEVEL.DEBUG:
          Logger.Debug(format, message);
          break;
        case RETRO_LOG_LEVEL.WARN:
          Logger.Warn(format, message);
          break;
        case RETRO_LOG_LEVEL.ERROR:
          Logger.Error(format, message);
          break;
        default:
          Logger.Debug(format, message);
          break;
      }
    }
    #endregion

    #region IDisposable
    public void Dispose()
    {
      if (_retroThread != null)
      {
        _retroThread.Dispose();
        _retroThread = null;
      }
      if (_stateBuffer != null)
      {
        _stateBuffer.Dispose();
        _stateBuffer = null;
      }
      if (_controllerWrapper != null)
      {
        _controllerWrapper.Dispose();
        _controllerWrapper = null;
      }
      if (_synchronizationStrategy != null)
      {
        _synchronizationStrategy.Stop();
        _synchronizationStrategy = null;
      }
    }
    #endregion
  }
}