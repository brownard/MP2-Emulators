using Emulators.LibRetro.Controllers.Mapping;
using MediaPortal.Common;
using MediaPortal.Plugins.InputDeviceManager;
using SharpLib.Hid;
using System;

namespace Emulators.LibRetro.Controllers.Hid
{
  abstract class AbstractHidMapper : IDeviceMapper, IDisposable
  {
    protected HidListener _hidListener;

    public AbstractHidMapper()
    {
      _hidListener = new HidListener();
      _hidListener.StateChanged += HidListener_StateChanged;
      _hidListener.Init();
    }

    public abstract bool SupportsDeadZone { get; }

    protected abstract void HidListener_StateChanged(object sender, Event e);

    public virtual void BeginMapping()
    {
      ServiceRegistration.Get<IInputDeviceManager>().KeyPressed += ExternalKeyHandler;
    }

    public virtual void EndMapping()
    {
      ServiceRegistration.Get<IInputDeviceManager>().KeyPressed -= ExternalKeyHandler;
    }

    protected abstract void ExternalKeyHandler(object sender, KeyPressHandlerEventArgs e);

    public abstract DeviceInput GetPressedInput();

    public virtual void Dispose()
    {
      if (_hidListener != null)
      {
        _hidListener.Dispose();
        _hidListener = null;
      }
    }
  }
}
