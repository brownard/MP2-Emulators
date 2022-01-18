using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System;
using System.Threading;

namespace Emulators.LibRetro.SoundProviders
{
  class XAudioPlayer : IDisposable
  {
    protected string _deviceId;
    protected XAudio2 _xAudio;
    // Mastering voice used for playback
    protected MasteringVoice _masteringVoice;
    // Source voice used to receive audio data
    protected SourceVoice _sourceVoice;

    protected bool _isPlaying;
    protected AutoResetEvent _bufferEndEvent;

    protected float _volume = 1.0f;

    protected long _totalSamplesSubmitted;

    /// <summary>
    /// Creates a new <see cref="XAudioPlayer"/> that uses the audio device
    /// with the specified <paramref name="deviceId"/> for playback.
    /// </summary>
    /// <param name="deviceId">The XAudio2 device id of the audio device to use for playback.</param>
    public XAudioPlayer(string deviceId)
    {
      _deviceId = deviceId;
    }

    /// <summary>
    /// Whether the player is currently playing.
    /// </summary>
    public bool IsPlaying
    {
      get { return _isPlaying; }
    }

    /// <summary>
    /// Specifies the format of the audio data to play. Must be called at least once
    /// before any calls to <see cref="SubmitBuffer(AudioBuffer)"/> will succeed, but
    /// can be called at any time to change the format during playback.
    /// </summary>
    /// <param name="sampleRate"></param>
    /// <param name="bits"></param>
    /// <param name="channels"></param>
    public void SetSourceFormat(int sampleRate, int bits, int channels)
    {
      // create the XAudio device if necessary, will be
      // the case on the first call to this method
      if (_xAudio == null)
        CreateXAudio();
      // Dispose any existing source voice and create
      // a new one with the specified format.
      RecreateSourceVoice(sampleRate, bits, channels);
    }

    /// <summary>
    /// Starts or resumes audio playback.
    /// </summary>
    public void Play()
    {
      if (_isPlaying)
        return;
      if (IsSourceVoiceValid)
        _sourceVoice.Start();
      _isPlaying = true;
    }

    /// <summary>
    /// Stops audio playback.
    /// </summary>
    public void Stop()
    {
      if (!_isPlaying)
        return;
      if (IsSourceVoiceValid)
        _sourceVoice.Stop();
      _isPlaying = false;
    }

    /// <summary>
    /// Sets the volume of the sound output, specified as an amplitude
    /// ratio where 0 is silent and 1 is the original volume.
    /// </summary>
    /// <param name="volume"></param>
    public void SetVolume(float volume)
    {      
      _volume = volume;
      _masteringVoice?.SetVolume(_volume);
    }

    /// <summary>
    /// Adds a new <see cref="AudioBuffer"/> to the playback queue.
    /// </summary>
    /// <param name="buffer"><see cref="AudioBuffer"/> to add to the playback queue.</param>
    public void SubmitBuffer(AudioBuffer buffer)
    {
      if (!IsSourceVoiceValid)
        return;
      _sourceVoice.SubmitSourceBuffer(buffer, null);
      _totalSamplesSubmitted += buffer.AudioBytes / 4;
    }

    /// <summary>
    /// Returns whether a buffer can be submitted without exceeding the maximum
    /// number of queued buffers specified in <paramref name="maxQueuedBuffers"/>.
    /// </summary>
    /// <param name="maxQueuedBuffers">The maximum number of buffers allowed in the queue.</param>
    /// <param name="wait">Whether to try and wait for space in the queue.</param>
    /// <returns><c>true</c> if there is space in the queue for the buffer.</returns>
    public bool CanSubmitBuffer(int maxQueuedBuffers, bool wait)
    {
      if (!IsSourceVoiceValid)
        return false;

      // BuffersQueued needs to be less than the max so
      // that queueing a new buffer doesn't exceed it
      if (_sourceVoice.State.BuffersQueued < maxQueuedBuffers)
        return true;

      if (!wait || !_isPlaying)
        return false;
      
      return WaitForQueuedBuffers(maxQueuedBuffers, 100);
    }

    public int GetBufferedSampleCount()
    {
      if (!IsSourceVoiceValid)
        return 0;

      return (int)(_totalSamplesSubmitted - _sourceVoice.State.SamplesPlayed);
    }

    public void SetFrequencyRatio(float ratio, out float currentRatio)
    {
      if (!IsSourceVoiceValid)
      {
        currentRatio = 1;
        return;
      }
      _sourceVoice.SetFrequencyRatio(ratio);
      _sourceVoice.GetFrequencyRatio(out currentRatio);
    }

    protected void CreateXAudio()
    {
      _xAudio = new XAudio2();
      _xAudio.StartEngine();
      _masteringVoice = new MasteringVoice(_xAudio, 2, XAudio2.DefaultSampleRate, _deviceId);
      _masteringVoice.SetVolume(_volume);
    }

    protected void RecreateSourceVoice(int sampleRate, int bits, int channels)
    {
      bool wasPlaying = _isPlaying;
      DestroySourceVoice();
      _bufferEndEvent = new AutoResetEvent(false);
      var format = new WaveFormat(sampleRate, bits, channels);
      _sourceVoice = new SourceVoice(_xAudio, format, VoiceFlags.None);
      _sourceVoice.BufferEnd += OnBufferEnd;
      if (wasPlaying)
        Play();
    }

    protected bool WaitForQueuedBuffers(int maxQueuedBuffers, int millisecondsTimeout)
    {
      // While we're playing, wait for the buffer end event until the queue is below the maximum allowed length 
      while (_isPlaying && IsSourceVoiceValid && _sourceVoice.State.BuffersQueued >= maxQueuedBuffers)
        if (!_bufferEndEvent.WaitOne(millisecondsTimeout))
          return false;

      return _isPlaying && IsSourceVoiceValid;
    }

    void OnBufferEnd(IntPtr obj)
    {
      _bufferEndEvent?.Set();
    }

    protected bool IsSourceVoiceValid
    {
      get { return _sourceVoice != null && !_sourceVoice.IsDisposed; }
    }

    protected void DestroySourceVoice()
    {
      if (_sourceVoice != null)
      {
        _sourceVoice.BufferEnd -= OnBufferEnd;
        if (!_sourceVoice.IsDisposed)
        {
          _sourceVoice.DestroyVoice();
          _sourceVoice.Dispose();
        }
        _sourceVoice = null;
      }
      if (_bufferEndEvent != null)
        _bufferEndEvent.Dispose();
      _bufferEndEvent = null;

      _isPlaying = false;
      _totalSamplesSubmitted = 0;
    }

    protected void DestroyXAudio()
    {
      if (_masteringVoice != null)
      {
        if (!_masteringVoice.IsDisposed)
        {
          _masteringVoice.DestroyVoice();
          _masteringVoice.Dispose();
        }
        _masteringVoice = null;
      }

      if (_xAudio != null && !_xAudio.IsDisposed)
        _xAudio.Dispose();
      _xAudio = null;
    }

    public void Dispose()
    {
      DestroySourceVoice();
      DestroyXAudio();
    }
  }
}
