using Emulators.LibRetro.Render;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using SharpDX.DirectSound;
using SharpDX.Multimedia;
using SharpRetro.Audio;
using SharpRetro.LibRetro;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Emulators.LibRetro.SoundProviders
{
  public class LibRetroDirectSound : ISoundOutput, IAudioOutput
  {
    public const double DEFAULT_BUFFER_SIZE_SECONDS = 0.4;

    protected readonly object _syncObj = new object();

    protected IntPtr _windowHandle;
    protected Guid _audioDeviceId;
    protected double _bufferSizeSeconds;
    SynchronizationStrategy _synchronizationStrategy;

    protected DirectSound _directSound;
    protected SecondarySoundBuffer _secondaryBuffer;
    protected int _bufferBytes;
    protected int _nextWrite;
    protected double _samplesPerMs;

    protected short[] _sampleBuffer;
    protected bool _isPlaying = false;

    /// <summary>
    /// Whether audio data has been received since
    /// the last time this property was [re]set.
    /// </summary>
    public bool HasAudio { get; set; }

    public LibRetroDirectSound(IntPtr windowHandle, Guid audioDeviceId, double bufferSizeSeconds)
    {
      _windowHandle = windowHandle;
      _audioDeviceId = audioDeviceId;
      _bufferSizeSeconds = bufferSizeSeconds > 0 ? bufferSizeSeconds : DEFAULT_BUFFER_SIZE_SECONDS;
    }
    
    protected void CreateDirectSound(int sampleRate)
    {
      lock (_syncObj)
      {
        try
        {
          // Destroy and cleanup any existing sound output
          DestroyDirectSound();
          CreateDevice(_windowHandle, _audioDeviceId);
          CreateSoundBuffer(sampleRate, _bufferSizeSeconds);
          ServiceRegistration.Get<ILogger>().Debug("LibRetroDirectSound: Audio initialized");
        }
        catch (Exception ex)
        {
          DestroyDirectSound();
          ServiceRegistration.Get<ILogger>().Warn("LibRetroDirectSound: Failed to create device {0}", ex, _audioDeviceId);
        }
      }
    }

    void CreateDevice(IntPtr windowHandle, Guid audioDeviceId)
    {
      _directSound = new DirectSound(audioDeviceId);
      // Set the cooperative level to priority so the format of the primary sound buffer can be modified.
      _directSound.SetCooperativeLevel(windowHandle, CooperativeLevel.Priority);
    }

    protected void CreateSoundBuffer(int sampleRate, double bufferSizeSeconds)
    {
      // Buffer to use when marshalling the samples from an unmanaged pointer.
      // Give it a reasonable initial size, it will be resized dynamically if needed.
      _sampleBuffer = new short[4096];

      // Setup the directsound buffer description
      var format = new WaveFormat(sampleRate, 16, 2);
      var buffer = new SoundBufferDescription();
      buffer.Flags = BufferFlags.GlobalFocus | BufferFlags.ControlVolume;
      buffer.BufferBytes = (int)(format.AverageBytesPerSecond * bufferSizeSeconds);
      buffer.Format = format;
      buffer.AlgorithmFor3D = Guid.Empty;

      // Create a sound buffer with the specified buffer description.
      _secondaryBuffer = new SecondarySoundBuffer(_directSound, buffer);

      // Get the actual buffer size so we can keep 
      // track of the current/next write position.
      _bufferBytes = _secondaryBuffer.Capabilities.BufferBytes;

      // When syncing to audio, we need samples per millisecond to estimate how
      // long to sleep for when waiting for enough free space in the playback buffer.
      _samplesPerMs = format.AverageBytesPerSecond / (sizeof(short) * 1000d);
    }

    /// <summary>
    /// Disposes of the current sound buffer and device.
    /// </summary>
    protected void DestroyDirectSound()
    {
      if (_secondaryBuffer != null)
      {
        _secondaryBuffer.Dispose();
        _secondaryBuffer = null;
      }
      if (_directSound != null)
      {
        _directSound.Dispose();
        _directSound = null;
      }
      _isPlaying = false;
    }

    public void SetSynchronizationStrategy(SynchronizationStrategy strategy)
    {
      lock (_syncObj)
        _synchronizationStrategy = strategy;
    }

    public bool Play()
    {
      try
      {
        lock (_syncObj)
        {
          if (_secondaryBuffer == null)
            return false;
          // Set the position at the beginning of the sound buffer.
          _secondaryBuffer.CurrentPosition = 0;
          // Set volume of the buffer to 100%
          _secondaryBuffer.Volume = 0;
          // Play the contents of the secondary sound buffer.
          _secondaryBuffer.Play(0, PlayFlags.Looping);
          _isPlaying = true;
        }
      }
      catch (Exception ex)
      {
        ServiceRegistration.Get<ILogger>().Warn("LibRetroDirectSound: Failed to start playback", ex);
        return false;
      }
      return true;
    }

    public void Pause()
    {
      lock (_syncObj)
        if (_isPlaying && _secondaryBuffer != null)
        {
          _secondaryBuffer.Stop();
          _isPlaying = false;
        }
    }

    public void UnPause()
    {
      lock (_syncObj)
        if (!_isPlaying && _secondaryBuffer != null)
        {
          _secondaryBuffer.Play(0, PlayFlags.Looping);
          _isPlaying = true;
        }
    }

    public void SetVolume(int volume)
    {
      lock (_syncObj)
        if (_secondaryBuffer != null)
          _secondaryBuffer.Volume = volume;
    }

    void IAudioOutput.SetTimingInfo(TimingInfo timingInfo)
    {
      // If we are currently playing then we need to
      //  resume playbackafter recreating the device.
      bool wasPlaying = _isPlaying;      
      CreateDirectSound((int)timingInfo.SampleRate);
      if (wasPlaying)
        Play();
    }

    void IAudioOutput.RetroAudioSample(short left, short right)
    {
      lock (_syncObj)
      {
        if (_secondaryBuffer == null)
          return;

        // Single audio frame
        _sampleBuffer[0] = left;
        _sampleBuffer[1] = right;
        WriteSamples(_sampleBuffer, 2);
      }
    }

    uint IAudioOutput.RetroAudioSampleBatch(IntPtr data, uint frames)
    {
      if (frames == 0)
        return frames;

      // Audio is stereo so there are 2 samples per frame
      int samples = (int)(frames * 2);
      lock (_syncObj)
      {
        if (_secondaryBuffer == null)
          return frames;

        // Resize the buffer if needed
        CheckSampleBufferSize(samples);
        Marshal.Copy(data, _sampleBuffer, 0, samples);
        // Write the samples to the playback buffer
        WriteSamples(_sampleBuffer, samples);
      }
      return frames;
    }

    protected void CheckSampleBufferSize(int requiredSize)
    {
      if (_sampleBuffer.Length >= requiredSize)
        return;
      int newSize = _sampleBuffer.Length * 2;
      while (newSize < requiredSize)
        newSize *= 2;
      _sampleBuffer = new short[newSize];
    }

    protected void WriteSamples(short[] samples, int count)
    {
      if (count == 0)
        return;

      HasAudio = true;

      if (_secondaryBuffer.Status == (int)BufferStatus.BufferLost)
        _secondaryBuffer.Restore();
      //If synchronise wait until there is enough free space
      if (_synchronizationStrategy != null && _synchronizationStrategy.SyncToAudio)
        Synchronize(count);
      int samplesNeeded = GetSamplesNeeded();
      if (samplesNeeded < 1)
        return;
      if (count > samplesNeeded)
        count = samplesNeeded;
      _secondaryBuffer.Write(samples, 0, count, _nextWrite, LockFlags.None);
      IncrementWritePosition(count * sizeof(short));
    }

    protected void IncrementWritePosition(int count)
    {
      _nextWrite = (_nextWrite + count) % _bufferBytes;
    }

    protected void Synchronize(int count)
    {
      int samplesNeeded = GetSamplesNeeded();
      while (samplesNeeded < count)
      {
        int sleepTime = (int)((count - samplesNeeded) / _samplesPerMs);
        Thread.Sleep(sleepTime / 2);
        samplesNeeded = GetSamplesNeeded();
      }
    }

    protected int GetSamplesNeeded()
    {
      return GetBytesNeeded() / sizeof(short);
    }

    protected int GetBytesNeeded()
    {
      int pPos;
      int wPos;
      _secondaryBuffer.GetCurrentPosition(out pPos, out wPos);
      return wPos < _nextWrite ? wPos + _bufferBytes - _nextWrite : wPos - _nextWrite;
    }

    public void Dispose()
    {
      lock (_syncObj)
        DestroyDirectSound();
    }
  }
}
