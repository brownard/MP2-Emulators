using SharpDX;
using SharpDX.XAudio2;
using System;

namespace Emulators.LibRetro.SoundProviders
{
  /// <summary>
  /// Implementation of a ring of <see cref="AudioBuffer"/> objects that can
  /// be used to write a stream of audio samples to <see cref="XAudio2"/>.
  /// </summary>
  public class XAudioRingBuffer : IDisposable
  {
    protected int _bufferCount;
    protected int _bufferSize;
    protected AudioBuffer[] _ringBuffer;
    protected int _currentBuffer;

    /// <summary>
    /// Creates a new <see cref="XAudioRingBuffer"/> with the specified
    /// number of buffers of the specified size.
    /// </summary>
    /// <param name="bufferCount">The number of buffers.</param>
    /// <param name="bufferSize">The size in bytes of each buffer.</param>
    public XAudioRingBuffer(int bufferCount, int bufferSize)
    {
      _bufferCount = bufferCount;
      _bufferSize = bufferSize;
      CreateBuffers();
    }

    /// <summary>
    /// The <see cref="AudioBuffer"/> currently used to write audio samples.
    /// </summary>
    public AudioBuffer CurrentBuffer
    {
      get { return _ringBuffer[_currentBuffer]; }
    }

    /// <summary>
    /// The number of <see cref="AudioBuffer"/> objects contained in this ring buffer.
    /// </summary>
    public int BufferCount
    {
      get { return _bufferCount; }
    }

    /// <summary>
    /// Writes a batch of audio samples to the <see cref="CurrentBuffer"/>.
    /// </summary>
    /// <param name="buffer">A pointer to the audio sample buffer to write.</param>
    /// <param name="offset">The offset at which to begin copying bytes from the <paramref name="buffer"/>.</param>
    /// <param name="count">The number of bytes to copy from the <paramref name="buffer"/>.</param>
    public void Write(IntPtr buffer, int offset, int count)
    {
      AudioBuffer audioBuffer = _ringBuffer[_currentBuffer];
      DataStream stream = audioBuffer.Stream;
      // Resize the buffer if necessary
      if (stream.Length < count)
      {
        stream.Dispose();
        stream = audioBuffer.Stream = new DataStream(count, true, true);
      }
      stream.Write(buffer, offset, count);
      audioBuffer.AudioBytes = (int)stream.Position;
    }

    /// <summary>
    /// Writes a single audio frame to the <see cref="CurrentBuffer"/>.
    /// </summary>
    /// <param name="left">The left sample.</param>
    /// <param name="right">The right sample.</param>
    /// <returns><c>False</c> if there was not enough space in the current buffer to write the audio frame.</returns>
    public bool WriteSamples(short left, short right)
    {
      AudioBuffer audioBuffer = _ringBuffer[_currentBuffer];
      DataStream stream = audioBuffer.Stream;
      // We could resize here but we'd then need to copy the existing
      // data to the resized stream, for now we'll just drop samples
      // until the current buffer is cycled.
      if (stream.RemainingLength < 4)
        return false;
      stream.Write(left);
      stream.Write(right);
      audioBuffer.AudioBytes = (int)stream.Position;
      return true;
    }

    /// <summary>
    /// Cycles the <see cref="CurrentBuffer"/> to the next <see cref="AudioBuffer"/>
    /// in the ring and returns the previous buffer.
    /// </summary>
    /// <returns>The previous <see cref="CurrentBuffer"/>.</returns>
    public AudioBuffer CycleCurrentBuffer()
    {
      AudioBuffer oldBuffer = _ringBuffer[_currentBuffer];
      oldBuffer.Stream.Position = 0;

      _currentBuffer = (_currentBuffer + 1) % _ringBuffer.Length;
      AudioBuffer currentBuffer = _ringBuffer[_currentBuffer];
      currentBuffer.Stream.Position = 0;
      currentBuffer.AudioBytes = 0;

      return oldBuffer;
    }

    /// <summary>
    /// Creates the array of <see cref="AudioBuffer"/> objects that
    /// will be used to back this ring buffer.
    /// </summary>
    protected void CreateBuffers()
    {
      _ringBuffer = new AudioBuffer[_bufferCount];
      for (int i = 0; i < _ringBuffer.Length; i++)
        _ringBuffer[i] = new AudioBuffer(new DataStream(_bufferSize, true, true));
    }

    /// <summary>
    /// Destroys all <see cref="AudioBuffer"/> objects.
    /// </summary>
    protected void DestroyBuffers()
    {
      if (_ringBuffer == null)
        return;
      for (int i = 0; i < _ringBuffer.Length; i++)
        _ringBuffer[i].Stream.Dispose();
      _ringBuffer = null;
    }

    /// <summary>
    /// Releases all resources used by this ring buffer.
    /// </summary>
    public void Dispose()
    {
      DestroyBuffers();
    }
  }
}
