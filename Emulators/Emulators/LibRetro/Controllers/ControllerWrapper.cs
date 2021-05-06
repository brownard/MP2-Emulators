using Emulators.LibRetro.Controllers.Hid;
using Emulators.LibRetro.Controllers.Mapping;
using Emulators.LibRetro.Controllers.XInput;
using MediaPortal.Common;
using MediaPortal.Plugins.InputDeviceManager;
using SharpRetro.Controller;
using SharpRetro.LibRetro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Emulators.LibRetro.Controllers
{
  public class ControllerWrapper : IRetroPad, IRetroAnalog, IRetroRumble, IDisposable
  {
    #region Dummy Controller
    class DummyController : IRetroPad, IRetroAnalog, IRetroRumble
    {
      public bool IsButtonPressed(uint port, RETRO_DEVICE_ID_JOYPAD button)
      {
        return false;
      }
      public short GetAnalog(uint port, RETRO_DEVICE_INDEX_ANALOG index, RETRO_DEVICE_ID_ANALOG direction)
      {
        return 0;
      }

      public bool SetRumbleState(uint port, retro_rumble_effect effect, ushort strength)
      {
        return false;
      }
    }
    #endregion

    protected int _maxControllers;
    protected IRetroPad[] _retroPads;
    protected IRetroAnalog[] _retroAnalogs;
    protected IRetroRumble[] _retroRumbles;
    protected List<IHidDevice> _hidDevices;
    protected List<IXInputDevice> _xInputDevices;
    protected HidListener _hidListener;

    public ControllerWrapper(int maxControllers)
    {
      _maxControllers = maxControllers > 0 ? maxControllers : 1;
      _retroPads = new IRetroPad[_maxControllers];
      _retroAnalogs = new IRetroAnalog[_maxControllers];
      _retroRumbles = new IRetroRumble[_maxControllers];
      _hidDevices = new List<IHidDevice>(_maxControllers);
      _xInputDevices = new List<IXInputDevice>(_maxControllers);

      DummyController dummy = new DummyController();
      for (int i = 0; i < _maxControllers; i++)
      {
        _retroPads[i] = dummy;
        _retroAnalogs[i] = dummy;
        _retroRumbles[i] = dummy;
      }
    }

    public void AddController(IMappableDevice controller, int port)
    {
      if (port >= _maxControllers)
        return;

      IRetroPad retroPad = controller as IRetroPad;
      if (retroPad != null)
        _retroPads[port] = retroPad;

      IRetroAnalog retroAnalog = controller as IRetroAnalog;
      if (retroAnalog != null)
        _retroAnalogs[port] = retroAnalog;

      IRetroRumble retroRumble = controller as IRetroRumble;
      if (retroRumble != null)
        _retroRumbles[port] = retroRumble;

      IHidDevice hidDevice = controller as IHidDevice;
      if (hidDevice != null)
        _hidDevices.Add(hidDevice);

      IXInputDevice xInputDevice = controller as IXInputDevice;
      if (xInputDevice != null)
        _xInputDevices.Add(xInputDevice);
    }

    public void Start()
    {
      ServiceRegistration.Get<IInputDeviceManager>().RegisterExternalKeyHandling(ExternalKeyHandler);
      if (_hidDevices.Count > 0)
      {
        _hidListener = new HidListener();
        _hidListener.StateChanged += HidListener_StateChanged;
        _hidListener.Init();
      }
    }

    private bool ExternalKeyHandler(object sender, string deviceName, string deviceFriendlyName, string deviceId, IDictionary<string, long> pressedKeys)
    {
      // For HID devices we can check if the HID key handler was invoked for our device and mapped key.
      IHidDevice hidDevice = _hidDevices.FirstOrDefault(d => d.Mp2DeviceId == deviceId);
      if (hidDevice != null)
        return pressedKeys.Any(kvp => hidDevice.IskeyCodeMapped(kvp.Value));

      // For XInput devices the best we can do is check whether the key handler was invoked for any XInput device.
      return _xInputDevices.Count > 0 && HidUtils.IsXInputDevice(deviceName);
    }

    private void HidListener_StateChanged(object sender, HidStateEventArgs e)
    {
      foreach (IHidDevice device in _hidDevices)
        if (device.UpdateState(e.State))
          return;
    }

    public bool IsButtonPressed(uint port, RETRO_DEVICE_ID_JOYPAD button)
    {
      return port < _maxControllers ? _retroPads[port].IsButtonPressed(port, button) : false;
    }

    public short GetAnalog(uint port, RETRO_DEVICE_INDEX_ANALOG index, RETRO_DEVICE_ID_ANALOG direction)
    {
      return port < _maxControllers ? _retroAnalogs[port].GetAnalog(port, index, direction) : (short)0;
    }

    public bool SetRumbleState(uint port, retro_rumble_effect effect, ushort strength)
    {
      return port < _maxControllers ? _retroRumbles[port].SetRumbleState(port, effect, strength) : false;
    }

    public void Dispose()
    {
      ServiceRegistration.Get<IInputDeviceManager>().UnRegisterExternalKeyHandling(ExternalKeyHandler);
      if (_hidListener != null)
      {
        _hidListener.Dispose();
        _hidListener = null;
      }
    }
  }
}