using SharpRetro.Audio;
using SharpRetro.Controller;
using SharpRetro.Cores;
using SharpRetro.Utils;
using SharpRetro.Video;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SharpRetro.LibRetro
{
  public delegate void LogDelegate(RETRO_LOG_LEVEL level, string message);

  /// <summary>
  /// Implements the LibRetro API, enabling the loading and running of LibRetro cores
  /// </summary>
  public unsafe class LibRetroEmulator
  {
    #region LibRetro Interfaces
    retro_log_printf_t retro_log_printf_cb;
    retro_perf_callback retro_perf_callback = new retro_perf_callback();
    retro_rumble_interface retro_rumble_interface = new retro_rumble_interface();
    retro_core_options_update_display_callback_t retro_core_options_update_display_cb;
    #endregion

    #region Protected Members
    protected const short TRUE_SHORT = 1;
    protected const short FALSE_SHORT = 0;
    
    protected UnmanagedResourceHeap _unmanagedResources = new UnmanagedResourceHeap();
    protected LibRetroVariables _variables = new LibRetroVariables();
    protected LogDelegate _logDelegate;

    protected bool _firstRun = true;
    protected string _systemDirectory;
    protected string _saveDirectory;
    protected bool _canDupe = true;
    protected uint _performanceLevel;

    protected IAudioOutput _audioOutput;
    protected IVideoOutput _videoOutput;

    protected bool _supportsNoGame;

    protected SystemInfo _systemInfo;
    protected VideoInfo _videoInfo;
    protected TimingInfo _timingInfo;
    protected RETRO_PIXEL_FORMAT _pixelFormat = RETRO_PIXEL_FORMAT.XRGB1555;

    protected IRetroController _retroController;
    protected IRetroPad _retroPad;
    protected IRetroAnalog _retroAnalog;
    protected IRetroKeyboard _retroKeyboard;
    protected IRetroPointer _retroPointer;
    protected IRetroRumble _retroRumble;

    protected string _corePath;
    protected ICore _core;

    #endregion

    #region Public Properties

    /// <summary>
    /// The underlying core.
    /// </summary>
    public ICore Core
    {
      get { return _core; }
    }

    /// <summary>
    /// Gets or sets the <see cref="IAudioOutput"/>
    /// implementation to use to play audio samples.
    /// </summary>
    public IAudioOutput AudioOutput
    {
      get { return _audioOutput; }
      set { _audioOutput = value; }
    }

    /// <summary>
    /// Gets or sets the <see cref="IVideoOutput"/>
    /// implementation to use to display video.
    /// </summary>
    public IVideoOutput VideoOutput
    {
      get { return _videoOutput; }
      set { _videoOutput = value; }
    }

    /// <summary>
    /// Gets or sets a delegate to use for logging
    /// </summary>
    public LogDelegate LogDelegate
    {
      get { return _logDelegate; }
      set { _logDelegate = value; }
    }

    /// <summary>
    /// Gets or sets a controller interface to use for polling for input events
    /// </summary>
    public IRetroController Controller
    {
      get { return _retroController; }
      set
      {
        _retroController = value;
        _retroPad = value as IRetroPad;
        _retroAnalog = value as IRetroAnalog;
        _retroKeyboard = value as IRetroKeyboard;
        _retroPointer = value as IRetroPointer;
        _retroRumble = value as IRetroRumble;
      }
    }

    /// <summary>
    /// Gets or sets the system directory to pass to the LibRetro core
    /// </summary>
    public string SystemDirectory
    {
      get { return _systemDirectory; }
      set { _systemDirectory = value; }
    }

    /// <summary>
    /// Gets or sets the save directory to provide to the LibRetro core
    /// </summary>
    public string SaveDirectory
    {
      get { return _saveDirectory; }
      set { _saveDirectory = value; }
    }

    /// <summary>
    /// Gets the system information provided by the LibRetro core
    /// </summary>
    public SystemInfo SystemInfo
    {
      get { return _systemInfo; }
    }

    /// <summary>
    /// Gets the available variables defined by the LibRetro core and allows variables to be set
    /// </summary>
    public LibRetroVariables Variables
    {
      get { return _variables; }
    }

    /// <summary>
    /// Gets information about the video output of the LibRetro core.
    /// Potentially updated every time the video buffer/OpenGL context is updated
    /// </summary>
    public VideoInfo VideoInfo
    {
      get { return _videoInfo; }
    }

    /// <summary>
    /// Gets the timing information provided by the LibRetro core
    /// </summary>
    public TimingInfo TimingInfo
    {
      get { return _timingInfo; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the frontend can duplicate video frames 
    /// </summary>
    public bool CanDupeFrame
    {
      get { return _canDupe; }
      set { _canDupe = value; }
    }

    public bool SupportsNoGame
    {
      get { return _supportsNoGame; }
    }

    /// <summary>
    /// Gets the suggested performance level of the frontend.
    /// Values range from 1 - 15, where 1 is low performance and 15 high performance.
    /// </summary>
    public uint PerformanceLevel
    {
      get { return _performanceLevel; }
    }
    #endregion

    #region Ctor
    /// <summary>
    /// 
    /// </summary>
    /// <param name="corePath">The path to the LibRetro core to load</param>
    public LibRetroEmulator(string corePath)
    {
      _corePath = corePath;
      _core = new Core(corePath);
      _core.SetEnvironment(RetroEnvironment);
      _core.SetVideoRefresh(RetroVideoRefresh);
      _core.SetAudioSample(RetroAudioSample);
      _core.SetAudioSampleBatch(RetroAudioSampleBatch);
      _core.SetInputPoll(RetroInputPoll);
      _core.SetInputStateCallback(RetroInputState);
    }
    #endregion

    #region Init/Deinit
    /// <summary>
    /// Initializes the LibRetro core
    /// </summary>
    public void Init()
    {
      try
      {
        InitInterfaces();
        InitPaths();
        _core.Init();
        UpdateSystemInfo();
      }
      catch
      {
        Dispose();
        throw;
      }
    }

    /// <summary>
    /// Deinitialize the LibRetro core
    /// </summary>
    public void DeInit()
    {
      //Mupen64 crashes if deinit is called when run hasn't been called
      if (!_firstRun)
        _core.Deinit();
    }

    protected void InitInterfaces()
    {
      retro_log_printf_cb = new retro_log_printf_t(RetroLogPrintf);
      retro_rumble_interface.set_rumble_state = new retro_set_rumble_state_t(RetroSetRumbleState);

      //no way (need new mechanism) to check for SSSE3, MMXEXT, SSE4, SSE42
      retro_perf_callback.get_cpu_features = new retro_get_cpu_features_t(() => (ulong)(
          (Win32PInvokes.IsProcessorFeaturePresent(Win32PInvokes.ProcessorFeature.InstructionsXMMIAvailable) ? RETRO_SIMD.SSE : 0) |
          (Win32PInvokes.IsProcessorFeaturePresent(Win32PInvokes.ProcessorFeature.InstructionsXMMI64Available) ? RETRO_SIMD.SSE2 : 0) |
          (Win32PInvokes.IsProcessorFeaturePresent(Win32PInvokes.ProcessorFeature.InstructionsSSE3Available) ? RETRO_SIMD.SSE3 : 0) |
          (Win32PInvokes.IsProcessorFeaturePresent(Win32PInvokes.ProcessorFeature.InstructionsMMXAvailable) ? RETRO_SIMD.MMX : 0)
        ));
      retro_perf_callback.get_perf_counter = new retro_perf_get_counter_t(() => System.Diagnostics.Stopwatch.GetTimestamp());
      retro_perf_callback.get_time_usec = new retro_perf_get_time_usec_t(() => DateTime.Now.Ticks / 10);
      retro_perf_callback.perf_log = new retro_perf_log_t(() => { });
      retro_perf_callback.perf_register = new retro_perf_register_t((ref retro_perf_counter counter) => { });
      retro_perf_callback.perf_start = new retro_perf_start_t((ref retro_perf_counter counter) => { });
      retro_perf_callback.perf_stop = new retro_perf_stop_t((ref retro_perf_counter counter) => { });
    }

    protected void InitPaths()
    {
      if (string.IsNullOrEmpty(_systemDirectory))
        _systemDirectory = Path.GetDirectoryName(_corePath);
      if (string.IsNullOrEmpty(_saveDirectory))
        _saveDirectory = Path.GetDirectoryName(_corePath);
    }

    protected void UpdateSystemInfo()
    {
      retro_system_info system_info = new retro_system_info();
      _core.GetSystemInfo(ref system_info);
      _systemInfo = new SystemInfo()
      {
        LibraryName = Marshal.PtrToStringAnsi(system_info.library_name),
        LibraryVersion = Marshal.PtrToStringAnsi(system_info.library_version),
        ValidExtensions = Marshal.PtrToStringAnsi(system_info.valid_extensions),
        NeedsFullPath = system_info.need_fullpath,
        BlockExtract = system_info.block_extract
      };
    }
    #endregion

    #region Load Game
    /// <summary>
    /// Loads the specified game.
    /// </summary>
    /// <param name="path">The path to the game to load</param>
    /// <param name="data">If <see cref="SystemInfo.NeedsFullPath"/> is <c>false</c> this should be a byte array containing the game. Otherwise <c>null</c></param>
    /// <returns>True if the game was loaded successfully</returns>
    public bool LoadGame(string path, byte[] data)
    {
      return LoadGame(path, data, "");
    }

    /// <summary>
    /// Loads the specified game.
    /// </summary>
    /// <param name="path">The path to the game to load</param>
    /// <param name="data">If <see cref="SystemInfo.NeedsFullPath"/> is <c>false</c> this should be a byte array containing the game. Otherwise <c>null</c></param>
    /// <param name="meta">The metadata to pass to the core</param>
    /// <returns>True if the game was loaded successfully</returns>
    public bool LoadGame(string path, byte[] data, string meta)
    {
      retro_game_info gameInfo = new retro_game_info();
      gameInfo.path = path;
      gameInfo.meta = meta;
      if (data == null || data.Length == 0)
        return LoadGame(gameInfo);

      fixed (byte* p = &data[0])
      {
        gameInfo.data = (IntPtr)p;
        gameInfo.size = (uint)data.Length;
        return LoadGame(gameInfo);
      }
    }

    /// <summary>
    /// Loads the specified game.
    /// </summary>
    /// <param name="gameInfo">The game to load</param>
    /// <returns>True if the game was loaded successfully</returns>
    public bool LoadGame(retro_game_info gameInfo)
    {
      if (!_core.LoadGame(ref gameInfo))
      {
        Log(RETRO_LOG_LEVEL.WARN, "retro_load_game() failed");
        return false;
      }
      GetAVInfo();
      return true;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Reset()
    {
      _core.Reset();
    }

    /// <summary>
    /// Unloads the currently loaded game
    /// </summary>
    public void UnloadGame()
    {
      _core.UnloadGame();
    }

    protected void GetAVInfo()
    {
      retro_system_av_info av = new retro_system_av_info();
      _core.GetSystemAVInfo(ref av);
      _videoInfo = new VideoInfo((int)av.geometry.base_width, (int)av.geometry.base_height, av.geometry.aspect_ratio);
      _timingInfo = new TimingInfo(av.timing.fps, av.timing.sample_rate);
      _audioOutput?.SetTimingInfo(av.timing);
      _videoOutput?.SetGeometry(av.geometry);
    }
    #endregion

    #region Run
    /// <summary>
    /// Advances the currently loaded game by 1 frame
    /// </summary>
    public void Run()
    {
      _core.Run();
      _firstRun = false;
    }
    #endregion

    #region Memory
    /// <summary>
    /// Returns the size of the specified memory type
    /// </summary>
    /// <param name="memoryType">The type of memory</param>
    /// <returns>The size of the memory type</returns>
    public int GetMemorySize(RETRO_MEMORY memoryType)
    {
      return (int)_core.GetMemorySize(memoryType);
    }

    /// <summary>
    /// Writes the specified memory data to the provided buffer
    /// </summary>
    /// <param name="memoryType">The type of memory</param>
    /// <param name="buffer">A buffer to write the memory to. This should be at least the size returned by <see cref="GetMemorySize(RETRO_MEMORY)"/></param>
    /// <returns>True if the memory data was successfully retrieved</returns>
    public bool GetMemoryData(RETRO_MEMORY memoryType, byte[] buffer)
    {
      int size = (int)_core.GetMemorySize(memoryType);
      if (size == 0)
        return false;
      IntPtr ptr = _core.GetMemoryData(memoryType);
      if (ptr == IntPtr.Zero)
        return false;
      Marshal.Copy(ptr, buffer, 0, size);
      return true;
    }

    /// <summary>
    /// Convenience method for <see cref="GetMemoryData(RETRO_MEMORY, byte[])"/> that allocates and returns a buffer containing the memory data
    /// </summary>
    /// <param name="memoryType">The type of memory to retrieve</param>
    /// <returns>A byte array containing the memory data</returns>
    public byte[] GetMemoryData(RETRO_MEMORY memoryType)
    {
      uint size = _core.GetMemorySize(memoryType);
      if (size == 0)
        return null;
      IntPtr ptr = _core.GetMemoryData(memoryType);
      if (ptr == IntPtr.Zero)
        return null;
      byte[] saveBuffer = new byte[size];
      Marshal.Copy(ptr, saveBuffer, 0, saveBuffer.Length);
      return saveBuffer;
    }

    /// <summary>
    /// Loads memory data of the specified type into the LibRetro core
    /// </summary>
    /// <param name="memoryType">The type of memory to load</param>
    /// <param name="buffer">A byte array containing the memory data</param>
    /// <returns>True if the memory data was successfully loaded</returns>
    public bool SetMemoryData(RETRO_MEMORY memoryType, byte[] buffer)
    {
      int size = (int)_core.GetMemorySize(memoryType);
      if (size > buffer.Length)
        size = buffer.Length;
      IntPtr ptr = _core.GetMemoryData(memoryType);
      if (ptr == IntPtr.Zero)
        return false;
      Marshal.Copy(buffer, 0, ptr, size);
      return true;
    }

    /// <summary>
    /// Gets the size of the serialized state
    /// </summary>
    /// <returns>The size of the serialized state</returns>
    public int GetSerializeSize()
    {
      return (int)_core.SerializeSize();
    }

    /// <summary>
    /// Serializes the current state of the LibRetro core to the provided buffer
    /// </summary>
    /// <param name="buffer">The buffer to write the serialized state to. This should be at least the size returned by <see cref="GetSerializeSize"/></param>
    /// <returns>True if the state was successfully serialized</returns>
    public bool Serialize(byte[] buffer)
    {
      uint size = _core.SerializeSize();
      if (size == 0 || buffer.Length < size)
        return false;
      fixed (byte* p = &buffer[0])
        return _core.Serialize((IntPtr)p, size);
    }

    /// <summary>
    /// Convenience method for <see cref="Serialize(byte[])"/> that allocates and returns a buffer containing the serialized state
    /// </summary>
    /// <returns>A buffer containing the current state of the LibRetro core if successful, otherwise null</returns>
    public byte[] Serialize()
    {
      uint size = _core.SerializeSize();
      byte[] buffer = new byte[size];
      bool result;
      fixed (byte* p = &buffer[0])
        result = _core.Serialize((IntPtr)p, size);
      return result ? buffer : null;
    }

    /// <summary>
    /// Unserializes a serialized state into the LibRetro core
    /// </summary>
    /// <param name="buffer">A buffer containing the serialized state</param>
    public void Unserialize(byte[] buffer)
    {
      uint size = _core.SerializeSize();
      fixed (byte* p = &buffer[0])
        _core.Unserialize((IntPtr)p, size);
    }
    #endregion

    #region Environment

    protected bool RetroEnvironment(RETRO_ENVIRONMENT cmd, IntPtr data)
    {      
      //Log("Environment: {0}", cmd);
      switch (cmd)
      {
        case RETRO_ENVIRONMENT.SET_ROTATION:
          return false;
        case RETRO_ENVIRONMENT.GET_OVERSCAN:
          return false;
        case RETRO_ENVIRONMENT.GET_CAN_DUPE:
          *(bool*)data.ToPointer() = _canDupe;
          return true;
        case RETRO_ENVIRONMENT.SET_MESSAGE:
          return SetMessage(data);
        case RETRO_ENVIRONMENT.SHUTDOWN:
          return false;
        case RETRO_ENVIRONMENT.SET_PERFORMANCE_LEVEL:
          _performanceLevel = *(uint*)data.ToPointer();
          Log(RETRO_LOG_LEVEL.DEBUG, "Core suggested SET_PERFORMANCE_LEVEL {0}", _performanceLevel);
          return true;
        case RETRO_ENVIRONMENT.GET_SYSTEM_DIRECTORY:
          Log(RETRO_LOG_LEVEL.DEBUG, "returning system directory: " + _systemDirectory);
          return SetDirectory(data, _systemDirectory);
        case RETRO_ENVIRONMENT.SET_PIXEL_FORMAT:
          return SetPixelFormat(data);
        case RETRO_ENVIRONMENT.SET_INPUT_DESCRIPTORS:
          return false;
        case RETRO_ENVIRONMENT.SET_KEYBOARD_CALLBACK:
          return false;
        case RETRO_ENVIRONMENT.SET_DISK_CONTROL_INTERFACE:
          return false;
        case RETRO_ENVIRONMENT.SET_HW_RENDER:
          return InitGlContext(data);
        case RETRO_ENVIRONMENT.GET_VARIABLE:
          return GetVariable(data);
        case RETRO_ENVIRONMENT.SET_VARIABLES:
          return SetVariables(data);
        case RETRO_ENVIRONMENT.GET_VARIABLE_UPDATE:
          return _variables.Updated;
        case RETRO_ENVIRONMENT.SET_SUPPORT_NO_GAME:
          _supportsNoGame = true;
          return true;
        case RETRO_ENVIRONMENT.GET_LIBRETRO_PATH:
          Log(RETRO_LOG_LEVEL.DEBUG, "returning libretro path: " + _corePath);
          *((IntPtr*)data.ToPointer()) = _unmanagedResources.StringToHGlobalAnsiCached(_corePath);
          return true;
        case RETRO_ENVIRONMENT.SET_AUDIO_CALLBACK:
          return false;
        case RETRO_ENVIRONMENT.SET_FRAME_TIME_CALLBACK:
          return false;
        case RETRO_ENVIRONMENT.GET_RUMBLE_INTERFACE:
          Marshal.StructureToPtr(retro_rumble_interface, data, false);
          return true;
        case RETRO_ENVIRONMENT.GET_INPUT_DEVICE_CAPABILITIES:
          return false;
        case RETRO_ENVIRONMENT.GET_LOG_INTERFACE:
          *(IntPtr*)data = Marshal.GetFunctionPointerForDelegate(retro_log_printf_cb);
          return true;
        case RETRO_ENVIRONMENT.GET_PERF_INTERFACE:
          Marshal.StructureToPtr(retro_perf_callback, data, false);
          return true;
        case RETRO_ENVIRONMENT.GET_LOCATION_INTERFACE:
          return false;
        case RETRO_ENVIRONMENT.GET_CORE_ASSETS_DIRECTORY:
          return false;
        case RETRO_ENVIRONMENT.GET_SAVE_DIRECTORY:
          Log(RETRO_LOG_LEVEL.DEBUG, "returning save directory: " + _saveDirectory);
          return SetDirectory(data, _saveDirectory);
        case RETRO_ENVIRONMENT.SET_SYSTEM_AV_INFO:
          return SetAVInfo(data);
        case RETRO_ENVIRONMENT.SET_CONTROLLER_INFO:
          return true;
        case RETRO_ENVIRONMENT.SET_MEMORY_MAPS:
          return false;
        case RETRO_ENVIRONMENT.SET_GEOMETRY:
          return SetGeometry(data);
        case RETRO_ENVIRONMENT.SET_SUBSYSTEM_INFO:
          return SetSubsystemInfo(data);
        case RETRO_ENVIRONMENT.SET_SERIALIZATION_QUIRKS:
          RETRO_SERIALIZATION_QUIRK quirks = (RETRO_SERIALIZATION_QUIRK)(*(ulong*)data.ToPointer());
          return false;
        case RETRO_ENVIRONMENT.SET_HW_SHARED_CONTEXT:
          return true;
        case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_GET_AUDIO_VIDEO_ENABLE:
          // Enable video and audio
          *(int*)data.ToPointer() = (int)(AUDIO_VIDEO_ENABLE.ENABLE_VIDEO | AUDIO_VIDEO_ENABLE.ENABLE_AUDIO);
          return true;
        case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_GET_CORE_OPTIONS_VERSION:
          *(uint*)data.ToPointer() = 2;
          return true;
        case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_CORE_OPTIONS:
          return SetCoreOptions(data);
        case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_CORE_OPTIONS_INTL:
          return SetCoreOptionsIntl(data);
        case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_CORE_OPTIONS_DISPLAY:
          return true;
        case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_CORE_OPTIONS_V2:
          return SetCoreOptionsV2(data);
        case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_CORE_OPTIONS_V2_INTL:
          return SetCoreOptionsIntlV2(data);
        case RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_SET_CORE_OPTIONS_UPDATE_DISPLAY_CALLBACK:
          retro_core_options_update_display_callback cb = Marshal.PtrToStructure<retro_core_options_update_display_callback>(data);
          retro_core_options_update_display_cb = cb.callback;
          return true;
        default:
          Log(RETRO_LOG_LEVEL.DEBUG, "Unknkown retro_environment command {0} - {1}", (int)cmd, cmd & (~RETRO_ENVIRONMENT.RETRO_ENVIRONMENT_EXPERIMENTAL));
          return false;
      }
    }

    protected bool SetMessage(IntPtr data)
    {
      retro_message msg = Marshal.PtrToStructure<retro_message>(data);
      if (!string.IsNullOrEmpty(msg.msg))
        Log(RETRO_LOG_LEVEL.DEBUG, "LibRetro Message: {0}", msg.msg);
      return true;
    }

    protected bool SetDirectory(IntPtr data, string directory)
    {
      if (!CreateDirectory(directory))
        return false;
      *((IntPtr*)data.ToPointer()) = _unmanagedResources.StringToHGlobalAnsiCached(directory);
      return true;
    }

    protected bool CreateDirectory(string directory)
    {
      try
      {
        Directory.CreateDirectory(directory);
        return true;
      }
      catch (Exception ex)
      {
        Log(RETRO_LOG_LEVEL.ERROR, "Error creating directory '{0}' - {1}", directory, ex);
      }
      return false;
    }

    protected bool SetPixelFormat(IntPtr data)
    {
      RETRO_PIXEL_FORMAT format = (RETRO_PIXEL_FORMAT)Marshal.ReadInt32(data);
      switch (format)
      {
        case RETRO_PIXEL_FORMAT.RGB565:
        case RETRO_PIXEL_FORMAT.XRGB1555:
        case RETRO_PIXEL_FORMAT.XRGB8888:
          _pixelFormat = format;
          _videoOutput?.SetPixelFormat(format);
          Log(RETRO_LOG_LEVEL.DEBUG, "New pixel format set: {0}", _pixelFormat);
          return true;
        default:
          Log(RETRO_LOG_LEVEL.DEBUG, "Unrecognized pixel format: {0}", (int)format);
          return false;
      }
    }

    protected bool GetVariable(IntPtr data)
    {
      void** variablesPtr = (void**)data.ToPointer();
      IntPtr pKey = new IntPtr(*variablesPtr++);
      string key = Marshal.PtrToStringAnsi(pKey);
      Log(RETRO_LOG_LEVEL.DEBUG, "Requesting variable: {0}", key);
      VariableDescription variable;
      if (!_variables.TryGet(key, out variable))
      {
        Log(RETRO_LOG_LEVEL.WARN, "Variable {0}: not found", key);
        return false;
      }
      Log(RETRO_LOG_LEVEL.DEBUG, "Variable {0}: {1}", key, variable.SelectedOption);
      *variablesPtr = _unmanagedResources.StringToHGlobalAnsiCached(variable.SelectedOption).ToPointer();
      return true;
    }

    protected bool SetVariables(IntPtr data)
    {
      void** variablesPtr = (void**)data.ToPointer();
      for (;;)
      {
        IntPtr pKey = new IntPtr(*variablesPtr++);
        if (pKey == IntPtr.Zero)
          break;
        IntPtr pValue = new IntPtr(*variablesPtr++);
        VariableDescription variable = new VariableDescription(pKey, pValue);
        _variables.AddOrUpdate(variable);
        Log(RETRO_LOG_LEVEL.DEBUG, "Set variable: {0}", variable);
      }
      return true;
    }

    protected bool SetCoreOptions(IntPtr data)
    {
      PointerUtils.EnumerateUnmanagedArray(data, (ref retro_core_option_definition option) =>
      {
        if (option.key == null)
          return false;
        VariableDescription variable = new VariableDescription(ref option);
        _variables.AddOrUpdate(variable);
        Log(RETRO_LOG_LEVEL.DEBUG, "Set core option: {0}", variable);
        return true;
      });
      return true;
    }

    protected bool SetCoreOptionsIntl(IntPtr data)
    {
      retro_core_options_intl intl = Marshal.PtrToStructure<retro_core_options_intl>(data);
      return SetCoreOptions(intl.us);
    }

    protected bool SetCoreOptionsV2(IntPtr data)
    {
      retro_core_options_v2 options = Marshal.PtrToStructure<retro_core_options_v2>(data);
      PointerUtils.EnumerateUnmanagedArray(options.definitions, (ref retro_core_option_v2_definition option) =>
      {
        if (option.key == null)
          return false;
        VariableDescription variable = new VariableDescription(ref option);
        _variables.AddOrUpdate(variable);
        Log(RETRO_LOG_LEVEL.DEBUG, "Set core option v2: {0}", variable);
        return true;
      });
      return true;
    }

    protected bool SetCoreOptionsIntlV2(IntPtr data)
    {
      retro_core_options_v2_intl intl = Marshal.PtrToStructure<retro_core_options_v2_intl>(data);
      return SetCoreOptionsV2(intl.us);
    }

    protected bool SetSubsystemInfo(IntPtr data)
    {      
      return true;
    }

    protected bool InitGlContext(IntPtr data)
    {
      IHardwareRenderer hardwareRenderContext = _videoOutput as IHardwareRenderer;
      if (hardwareRenderContext == null)
        return false;

      retro_hw_render_callback hwRenderCallback = Marshal.PtrToStructure<retro_hw_render_callback>(data);
      if (hardwareRenderContext.SetHWRender(ref hwRenderCallback))
      {
        Marshal.StructureToPtr(hwRenderCallback, data, false);
        hardwareRenderContext.Create();
        return true;
      }
      return false;
    }

    protected bool SetAVInfo(IntPtr data)
    {
      retro_system_av_info* av = (retro_system_av_info*)data.ToPointer();
      _videoInfo = new VideoInfo((int)av->geometry.base_width, (int)av->geometry.base_height, av->geometry.aspect_ratio);
      _timingInfo = new TimingInfo(av->timing.fps, av->timing.sample_rate);
      _audioOutput?.SetTimingInfo(av->timing);
      _videoOutput?.SetGeometry(av->geometry);
      return true;
    }

    protected bool SetGeometry(IntPtr data)
    {
      retro_game_geometry geometry = *((retro_game_geometry*)data.ToPointer());
      _videoInfo = new VideoInfo((int)geometry.base_width, (int)geometry.base_height, geometry.aspect_ratio);
      _videoOutput?.SetGeometry(geometry);
      return true;
    }
    #endregion

    #region Video

    protected void RetroVideoRefresh(IntPtr data, uint width, uint height, uint pitch)
    {
      if (width > 0 && height > 0)
        _videoOutput?.VideoRefresh(data, width, height, pitch);
    }

    #endregion

    #region Audio

    protected void RetroAudioSample(short left, short right)
    {
      _audioOutput?.AudioSample(left, right);
    }

    protected uint RetroAudioSampleBatch(IntPtr data, uint frames)
    {
      if (frames > 0)
        frames = _audioOutput?.AudioSampleBatch(data, frames) ?? frames;
      return frames;
    }

    #endregion

    #region Input
    protected short RetroInputState(uint port, uint device, uint index, uint id)
    {
      switch ((RETRO_DEVICE)device)
      {
        case RETRO_DEVICE.POINTER:
          return GetPointerStatus((RETRO_DEVICE_ID_POINTER)id);
        case RETRO_DEVICE.KEYBOARD:
          return GetKeyboardStatus((RETRO_KEY)id) ? TRUE_SHORT : FALSE_SHORT;
        case RETRO_DEVICE.JOYPAD:
          return GetRetroPadStatus(port, (RETRO_DEVICE_ID_JOYPAD)id) ? TRUE_SHORT : FALSE_SHORT;
        case RETRO_DEVICE.ANALOG:
          return GetAnalogStatus(port, (RETRO_DEVICE_INDEX_ANALOG)index, (RETRO_DEVICE_ID_ANALOG)id);
      }
      return 0;
    }

    protected void RetroInputPoll()
    {
    }

    protected bool RetroSetRumbleState(uint port, retro_rumble_effect effect, ushort strength)
    {
      return _retroRumble != null && _retroRumble.SetRumbleState(port, effect, strength);
    }

    protected short GetPointerStatus(RETRO_DEVICE_ID_POINTER id)
    {
      if (_retroPointer != null)
      {
        switch (id)
        {
          case RETRO_DEVICE_ID_POINTER.X: return _retroPointer.GetPointerX();
          case RETRO_DEVICE_ID_POINTER.Y: return _retroPointer.GetPointerY();
          case RETRO_DEVICE_ID_POINTER.PRESSED: return _retroPointer.IsPointerPressed() ? TRUE_SHORT : FALSE_SHORT;
        }
      }
      return 0;
    }

    protected bool GetKeyboardStatus(RETRO_KEY key)
    {
      return _retroKeyboard != null && _retroKeyboard.IsKeyPressed(key);
    }

    protected bool GetRetroPadStatus(uint port, RETRO_DEVICE_ID_JOYPAD button)
    {
      return _retroPad != null && _retroPad.IsButtonPressed(port, button);
    }

    protected short GetAnalogStatus(uint port, RETRO_DEVICE_INDEX_ANALOG analogIndex, RETRO_DEVICE_ID_ANALOG analogDirection)
    {
      return _retroAnalog != null ? _retroAnalog.GetAnalog(port, analogIndex, analogDirection) : FALSE_SHORT;
    }
    #endregion

    #region Log
    protected void RetroLogPrintf(RETRO_LOG_LEVEL level, string fmt, IntPtr a0, IntPtr a1, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6, IntPtr a7, IntPtr a8, IntPtr a9, IntPtr a10, IntPtr a11, IntPtr a12, IntPtr a13, IntPtr a14, IntPtr a15)
    {
      if (_logDelegate == null)
        return;
      //avert your eyes, these things were not meant to be seen in c#
      //I'm not sure this is a great idea. It would suck for silly logging to be unstable. But.. I dont think this is unstable. The sprintf might just print some garbledy stuff.
      IntPtr[] args = new IntPtr[] { a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15 };
      string message = Printf(fmt, args);
      _logDelegate(level, message);
    }

    protected void Log(RETRO_LOG_LEVEL level, string format, params object[] args)
    {
      if (_logDelegate != null)
        _logDelegate(level, string.Format(format, args));
    }

    protected string Printf(string format, IntPtr[] args)
    {
      int idx = 0;
      string message;
      try
      {
        message = Sprintf.sprintf(format, () => args[idx++]);
      }
      catch (Exception ex)
      {
        message = string.Format("Error in sprintf - {0}", ex);
      }
      return message;
    }
    #endregion

    #region IDisposable
    /// <summary>
    /// Disposes the LibRetro core, the OpenGL context and any unmanaged resources
    /// </summary>
    public void Dispose()
    {
      _core.Dispose();
      _unmanagedResources.Dispose();
    }
    #endregion
  }
}