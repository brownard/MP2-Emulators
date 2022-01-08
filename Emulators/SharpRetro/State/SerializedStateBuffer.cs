using SharpRetro.Cores;
using System;
using System.Runtime.InteropServices;

namespace SharpRetro.State
{
  /// <summary>
  /// Keeps a buffer of n previous core states that allows 'timeshifting' backwards to a previous state.
  /// </summary>
  public class SerializedStateBuffer : IDisposable
  {
    /// <summary>
    /// Internal class used to store a pointer to a serialized state.
    /// </summary>
    protected class SerializedState
    {
      public IntPtr Buffer { get; set; } = IntPtr.Zero;
      public uint Size { get; set; }
    }

    protected ICore _core;
    protected int _bufferSize;
    protected SerializedState[] _states;
    protected int _writePosition;
    protected int _actualSize;

    protected int _resolution;
    protected TimeSpan _duration;
    protected int _updateFrame;
    protected int _currentFrame;

    protected bool _isAllocated;

    /// <summary>
    /// Creates a new <see cref="SerializedStateBuffer"/>.
    /// </summary>
    /// <param name="core">The initialized core to [un]serialize states to and from.</param>
    /// <param name="frameRate">The frame rate of the core, used to calculate on which frame to serialize a state for the given <paramref name="resolution"/> and <paramref name="duration"/>.</param>
    /// <param name="resolution">The number of states to store per second.</param>
    /// <param name="duration">The timespan of game time to store serialized states for.</param>
    public SerializedStateBuffer(ICore core, int frameRate, int resolution, TimeSpan duration) 
    {
      _core = core;
      _resolution = resolution;
      _duration = duration;
      _bufferSize = resolution * (int)duration.TotalSeconds;
      // only frames that are a multiple of this frame will be serialized
      _updateFrame = frameRate / resolution;
    }

    /// <summary>
    /// The total duration of the buffer.
    /// </summary>
    public TimeSpan Duration
    {
      get { return _duration; }
    }

    /// <summary>
    /// The current timespan of stored states.
    /// </summary>
    public TimeSpan BufferedTime
    {
      get { return TimeSpan.FromSeconds((double)_actualSize / _resolution); }
    }

    /// <summary>
    /// Allocates memory to store the states, must be called before the first call to <see cref="AppendState"/>.
    /// If there's not enough memory to allocate buffers for the required <see cref="Duration"/> then an <see cref="OutOfMemoryException"/> will be thrown.
    /// </summary>
    /// <exception cref="OutOfMemoryException"/>
    public void Allocate()
    {
      try
      {
        uint size = _core.SerializeSize();
        AllocateAll(size);
      }
      catch
      {
        ReleaseAll();
        throw;
      }
    }

    /// <summary>
    /// Serializes the current state of the core and appends it to the buffer if the current frame is a
    /// serializable frame based on the frame rate and resolution specified in the constructor.
    /// </summary>
    public void AppendState()
    {
      if (!_isAllocated)
        return;

      if (_currentFrame++ % _updateFrame != 0)
        return;

      uint size = _core.SerializeSize();
      
      SerializedState state = _states[_writePosition];
      if (state.Size < size)
      {
        ReleaseBuffer(state.Buffer);
        state.Buffer = AllocateBuffer((int)size);
        if (state.Buffer == IntPtr.Zero)
          return;
      }

      _core.Serialize(state.Buffer, size);
      state.Size = size;
      _writePosition = (_writePosition + 1) % _bufferSize;
      if (_actualSize < _bufferSize)
        _actualSize++;
    }

    /// <summary>
    /// Unserializes a previously serialized state, but does not alter the 'live' point of the buffer. Can be used when seeking.
    /// </summary>
    /// <param name="time">The game time, relative to the start of the buffer, that should be loaded.</param>
    /// <returns><c>True</c> if the state at the specified time was present in the buffer and successfully unserialized.</returns>
    public bool SeekToState(TimeSpan time)
    {
      if (!_isAllocated)
        return false;

      int offset = _resolution * (int)time.TotalSeconds;
      if (offset > _actualSize)
        return false;

      int position = (_writePosition - _actualSize + offset) % _bufferSize;
      if (position < 0)
        position += _bufferSize;
      SerializedState state = _states[position];
      if (state == null || state.Buffer == IntPtr.Zero)
        return false;

      return _core.Unserialize(state.Buffer, state.Size);
    }

    /// <summary>
    /// Unserializes a previously serialized state and sets it to the 'live' point of the buffer, any following states will be overwritten. Should be called after seeking has finished.
    /// </summary>
    /// <param name="time">The game time, relative to the start of the buffer, that should be loaded.</param>
    /// <returns><c>True</c> if the state at the specified time was present in the buffer and successfully unserialized.</returns>
    public bool SetCurrentPosition(TimeSpan time)
    {
      if (!SeekToState(time))
        return false;

      int offset = _resolution * (int)time.TotalSeconds;
      _writePosition = (_writePosition - _actualSize + offset) % _bufferSize;
      if (_writePosition < 0)
        _writePosition += _bufferSize;
      _actualSize = offset;
      return true;
    }

    protected IntPtr AllocateBuffer(int size)
    {
      return size > 0 ? Marshal.AllocHGlobal(size) : IntPtr.Zero;
    }

    protected void ReleaseBuffer(IntPtr buffer)
    {
      if (buffer != IntPtr.Zero)
        Marshal.FreeHGlobal(buffer);
    }

    protected void AllocateAll(uint stateSize)
    {
      _isAllocated = true;
      _states = new SerializedState[_bufferSize];
      for (int i = 0; i < _bufferSize; i++)
      {
        SerializedState state = new SerializedState();
        _states[i] = state;
        state.Buffer = AllocateBuffer((int)stateSize);
        state.Size = stateSize;
      }
    }

    protected void ReleaseAll()
    {
      _isAllocated = false;
      _writePosition = 0;
      _actualSize = 0;
      if (_states == null)
        return;

      foreach (SerializedState state in _states)
        if (state != null)
        {
          ReleaseBuffer(state.Buffer);
          state.Buffer = IntPtr.Zero;
          state.Size = 0;
        }
      _states = null;
    }

    #region IDisposable

    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          // TODO: dispose managed state (managed objects).
        }

        ReleaseAll();
        disposedValue = true;
      }
    }
    
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    ~SerializedStateBuffer()
    {
      Dispose(false);
    }

    #endregion
  }
}
