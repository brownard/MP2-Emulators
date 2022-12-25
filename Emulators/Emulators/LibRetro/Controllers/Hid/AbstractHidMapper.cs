using Emulators.LibRetro.Controllers.Mapping;
using System;

namespace Emulators.LibRetro.Controllers.Hid
{
  abstract class AbstractHidMapper : IDeviceMapper, IDisposable
  {
    protected HidListener _hidListener;

    public abstract bool SupportsDeadZone { get; }

    protected abstract void HidListener_StateChanged(object sender, StateChangedEventArgs e);

    public virtual void BeginMapping()
    {
      StartListener();
    }

    public virtual void EndMapping()
    {
      StopListener();
    }

    public abstract DeviceInput GetPressedInput();

    protected void StartListener()
    {
      if (_hidListener != null)
        return;
      _hidListener = new HidListener();
      _hidListener.StateChanged += HidListener_StateChanged;
      _hidListener.Init();
    }

    protected void StopListener()
    {
      if (_hidListener != null)
      {
        _hidListener.Dispose();
        _hidListener = null;
      }
    }

    public virtual void Dispose()
    {
      StopListener();
    }
  }
}
