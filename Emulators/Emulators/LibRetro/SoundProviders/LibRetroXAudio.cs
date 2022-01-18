using Emulators.LibRetro.Render;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using SharpRetro.Audio;
using SharpRetro.LibRetro;
using System;
using System.Linq;

namespace Emulators.LibRetro.SoundProviders
{
  class LibRetroXAudio : ISoundOutput, IAudioOutput, IDisposable
  {
    protected int _bufferCount = 3;
    protected int _bufferSize = 8 * 1024;

    protected string _deviceId;
    protected XAudioPlayer _player;
    protected XAudioRingBuffer _buffer;

    protected int _sampleRate;
    protected SynchronizationStrategy _strategy;

    protected double _dynamicRateControlDelta;
    protected DynamicAudioRateControl _rateControl;
    protected bool _doDynamicRateControl;

    public bool HasAudio { get; set; }

    public LibRetroXAudio(Guid audioDeviceId, double dynamicRateControlDelta)
    {
      _deviceId = GetDeviceId(audioDeviceId);
      _player = new XAudioPlayer(_deviceId);
      _buffer = new XAudioRingBuffer(_bufferCount, _bufferSize);
      _dynamicRateControlDelta = dynamicRateControlDelta;
    }

    public bool Play()
    {
      if (_player == null)
        return false;
      _player.Play();
      return true;
    }

    public void Pause()
    {
      _player?.Stop();
    }

    public void UnPause()
    {
      Play();
    }

    public void Update()
    {
      UpdateDynamicRateControl();

      // If in batch mode, the audio data was submitted in the call to
      // IAudioOutput.AudioSampleBatch, so doesn't need to be done here.
      if (_player == null || _buffer.CurrentBuffer.Stream.Position == 0)
        return;

      // Check whether there's space in the buffer queue. If syncing to audio,
      // this will try and block until space becomes available. If there's
      // enough space, cycle the current buffer and submit the old buffer for playback
      if (_player.CanSubmitBuffer(2, _strategy?.SyncToAudio == true))
        _player.SubmitBuffer(_buffer.CycleCurrentBuffer());
      else
      {
        // Buffer queue is full, drop the current data and reuse the buffer.
        _buffer.CurrentBuffer.Stream.Position = 0;
        _buffer.CurrentBuffer.AudioBytes = 0;
      }
    }

    public void SetVolume(int volume)
    {
      // MP2 volume is 1 to 100, XAudio2 uses 0 to 1
      _player?.SetVolume((float)volume / 100);
    }

    public void SetSynchronizationStrategy(SynchronizationStrategy strategy)
    {
      _strategy = strategy;
    }

    void IAudioOutput.SetTimingInfo(retro_system_timing timing)
    {
      int newSampleRate = (int)timing.sample_rate;
      if (newSampleRate == _sampleRate)
        return;
      _sampleRate = newSampleRate;
      _player?.SetSourceFormat(newSampleRate, 16, 2);

      if (_dynamicRateControlDelta > 0)
        _rateControl = new DynamicAudioRateControl(_bufferCount * _bufferSize / 4, _dynamicRateControlDelta);
    }

    void IAudioOutput.AudioSample(short left, short right)
    {
      HasAudio = true;
      _buffer?.WriteSamples(left, right);
    }

    uint IAudioOutput.AudioSampleBatch(IntPtr data, uint frames)
    {
      HasAudio = true;

      if (_player == null)
        return frames;

      int count = (int)(frames * 4);

      if (!_buffer.CanFitInCurrentBuffer(count))
      {
        if (_player.CanSubmitBuffer(_bufferCount - 1, _strategy?.SyncToAudio == true))
          _player.SubmitBuffer(_buffer.CycleCurrentBuffer());
        else
          return frames;
      }

      _buffer.Write(data, 0, count);

      return frames;
    }

    //uint IAudioOutput.AudioSampleBatch(IntPtr data, uint frames)
    //{
    //  unsafe
    //  {
    //    IAudioOutput output = this;
    //    short* samples = (short*)data;
    //    for (int i = 0; i < frames; i++)
    //      output.AudioSample(*samples++, *samples++);
    //    return frames;
    //  }
    //}

    /// <summary>
    /// Attempts to dynamically adjust the audio playback rate to match the video playback rate.
    /// </summary>
    protected void UpdateDynamicRateControl()
    {
      if (_rateControl == null || _player == null)
        return;

      if (_strategy == null || _strategy.SyncToAudio)
      {
        // Dynamic rate control not active, just return
        if (!_doDynamicRateControl)
          return;
        //Dynamic rate control active, reset and disable it
        _doDynamicRateControl = false;
        Logger.Debug("LibRetroXAudio: Disabling dynamic rate control");
      }
      else if (!_doDynamicRateControl)
      {
        _doDynamicRateControl = true;
        Logger.Debug("LibRetroXAudio: Enabling dynamic rate control with maximum delta {0}", _dynamicRateControlDelta);
      }

      double adjustment;
      if (_doDynamicRateControl)
      {
        int playerBuffered = _player.GetBufferedSampleCount();
        int bufferBuffered = _buffer.CurrentBuffer.AudioBytes / 4;
        adjustment = _rateControl.Update(playerBuffered + bufferBuffered);
        //Logger.Debug("Current playback rate: {0} ({1})", adjustment, playerBuffered + bufferBuffered);
      }
      else
      {
        // Resetting rate control, set frequency ratio back to 1
        adjustment = 1;
      }

      try
      {
        float currentRatio;
        _player.SetFrequencyRatio((float)adjustment, out currentRatio);
        //Logger.Debug("Current frequency ratio {0}", currentRatio);
      }
      catch (Exception ex)
      {
        Logger.Error("Error setting audio frequency ratio", ex);
      }
    }

    protected string GetDeviceId(Guid driverId)
    {
      string moduleName = SharpDX.DirectSound.DirectSound.GetDevices()?
        .FirstOrDefault(d => d.DriverGuid == driverId)?.ModuleName;
      return string.IsNullOrWhiteSpace(moduleName) ? null : moduleName;
    }

    public void Dispose()
    {
      if (_player != null)
      {
        _player.Dispose();
        _player = null;
      }
      if (_buffer != null)
      {
        _buffer.Dispose();
        _buffer = null;
      }
    }

    protected static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
  }
}