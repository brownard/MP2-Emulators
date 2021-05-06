using Emulators.LibRetro.Controllers.Mapping;
using MediaPortal.Common;
using MediaPortal.Plugins.InputDeviceManager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Emulators.LibRetro.Controllers.Hid
{
  class HidMapper : IDeviceMapper, IDisposable
  {
    protected ushort _vendorId;
    protected ushort _productId;
    protected string _mp2DeviceId;
    protected HidListener _hidListener;
    protected HidState _currentState;

    public HidMapper(ushort vendorId, ushort productId, string mp2DeviceId)
    {
      _vendorId = vendorId;
      _productId = productId;
      _mp2DeviceId = mp2DeviceId;
      _hidListener = new HidListener();
      _hidListener.StateChanged += HidListener_StateChanged;
      _hidListener.Init();
    }

    public bool SupportsDeadZone
    {
      get { return true; }
    }

    protected void HidListener_StateChanged(object sender, HidStateEventArgs e)
    {
      HidState state = e.State;
      if (state.VendorId == _vendorId && state.ProductId == _productId)
        _currentState = e.State;
    }

    public void BeginMapping()
    {
      ServiceRegistration.Get<IInputDeviceManager>().RegisterExternalKeyHandling(ExternalKeyHandler);
    }

    public void EndMapping()
    {
      ServiceRegistration.Get<IInputDeviceManager>().UnRegisterExternalKeyHandling(ExternalKeyHandler);
    }

    private bool ExternalKeyHandler(object sender, KeyPressHandlerEventArgs e)
    {
      return _mp2DeviceId == e.DeviceId;
    }

    public DeviceInput GetPressedInput()
    {
      HidState state = _currentState;
      if (state == null)
        return null;

      if (state.Buttons.Count > 0)
      {
        string buttonId = state.Buttons.First().ToString();
        return new DeviceInput(buttonId, buttonId, InputType.Button);
      }

      if (IsDirectionPadStateValid(state.DirectionPadState))
      {
        string direction = state.DirectionPadState.ToString();
        return new DeviceInput(direction, direction, InputType.Button);
      }

      foreach (HidAxisState axis in state.AxisStates.Values)
      {
        short value = NumericUtils.UIntToShort(axis.Value);
        if (value > HidGameControl.DEFAULT_DEADZONE || value < -HidGameControl.DEFAULT_DEADZONE)
        {
          bool isPositive = value > 0;
          return new DeviceInput(string.Format("{0}({1}){2}", axis.Index, axis.Name, isPositive ? "+" : "-"), axis.Index.ToString(), InputType.Axis, isPositive);
        }
      }

      return null;
    }

    protected bool IsDirectionPadStateValid(SharpLib.Hid.DirectionPadState directionPadState)
    {
      return directionPadState == SharpLib.Hid.DirectionPadState.Up
        || directionPadState == SharpLib.Hid.DirectionPadState.Right
        || directionPadState == SharpLib.Hid.DirectionPadState.Down
        || directionPadState == SharpLib.Hid.DirectionPadState.Left;
    }

    public void Dispose()
    {
      if (_hidListener != null)
      {
        _hidListener.Dispose();
        _hidListener = null;
      }
    }
  }
}
