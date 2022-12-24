using Emulators.LibRetro.Controllers.Mapping;
using SharpLib.Hid;
using System.Linq;

namespace Emulators.LibRetro.Controllers.Hid
{
  class HidGameControlMapper : AbstractHidMapper
  {
    protected ushort _vendorId;
    protected ushort _productId;
    protected HidState _currentState;

    public HidGameControlMapper(ushort vendorId, ushort productId)
    {
      _vendorId = vendorId;
      _productId = productId;
    }

    public override bool SupportsDeadZone
    {
      get { return true; }
    }

    public override void EndMapping()
    {
      base.EndMapping();
      _currentState = null;
    }

    public override DeviceInput GetPressedInput()
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

    protected override void HidListener_StateChanged(object sender, StateChangedEventArgs e)
    {
      if ((e.HidEvent.Device?.VendorId ?? 0) == _vendorId && (e.HidEvent.Device?.ProductId ?? 0) == _productId)
      {
        _currentState = HidUtils.GetGamepadState(e.HidEvent);
        e.Handled = true;
      }
    }

    protected bool IsDirectionPadStateValid(DirectionPadState directionPadState)
    {
      return directionPadState == DirectionPadState.Up
        || directionPadState == DirectionPadState.Right
        || directionPadState == DirectionPadState.Down
        || directionPadState == DirectionPadState.Left;
    }
  }
}
