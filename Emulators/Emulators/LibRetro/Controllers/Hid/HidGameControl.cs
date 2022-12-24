using Emulators.LibRetro.Controllers.Mapping;
using SharpLib.Hid;
using SharpRetro.Controller;
using SharpRetro.LibRetro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Emulators.LibRetro.Controllers.Hid
{
  class HidGameControl : AbstractHidDevice, IRetroPad, IRetroAnalog, IMappableDevice
  {
    public const short DEFAULT_DEADZONE = 8192;
    public static readonly Guid DEVICE_ID = new Guid("93543458-6267-4E2B-8DD9-E97A021BBD55");

    protected HidState _currentState;
    protected short _axisDeadZone = DEFAULT_DEADZONE;
    protected Dictionary<RETRO_DEVICE_ID_JOYPAD, ushort> _buttonToButtonMappings;
    protected Dictionary<RETRO_DEVICE_ID_JOYPAD, HidAxis> _analogToButtonMappings;
    protected Dictionary<RETRO_DEVICE_ID_JOYPAD, DirectionPadState> _directionPadToButtonMappings;
    protected Dictionary<RetroAnalogDevice, HidAxis> _analogToAnalogMappings;
    protected Dictionary<RetroAnalogDevice, ushort> _buttonToAnalogMappings;
    protected Dictionary<RetroAnalogDevice, DirectionPadState> _directionPadToAnalogMappings;

    protected HashSet<ushort> _mappedHidButtons;
    protected HashSet<ushort> _mappedHidAxis;
    protected HashSet<DirectionPadState> _mappedHidDirectionPadStates;

    public Guid DeviceId
    {
      get { return DEVICE_ID; }
    }

    public RetroPadMapping DefaultMapping
    {
      get { return null; }
    }

    public short AxisDeadZone
    {
      get { return _axisDeadZone; }
      set { _axisDeadZone = value; }
    }

    public HidGameControl(ushort vendorId, ushort productId, string friendlyName)
      : base(vendorId, productId, friendlyName)
    {
      _buttonToButtonMappings = new Dictionary<RETRO_DEVICE_ID_JOYPAD, ushort>();
      _analogToButtonMappings = new Dictionary<RETRO_DEVICE_ID_JOYPAD, HidAxis>();
      _directionPadToButtonMappings = new Dictionary<RETRO_DEVICE_ID_JOYPAD, DirectionPadState>();
      _analogToAnalogMappings = new Dictionary<RetroAnalogDevice, HidAxis>();
      _buttonToAnalogMappings = new Dictionary<RetroAnalogDevice, ushort>();
      _directionPadToAnalogMappings = new Dictionary<RetroAnalogDevice, DirectionPadState>();

      _mappedHidButtons = new HashSet<ushort>();
      _mappedHidAxis = new HashSet<ushort>();
      _mappedHidDirectionPadStates = new HashSet<DirectionPadState>();
    }

    public IDeviceMapper CreateMapper()
    {
      return new HidGameControlMapper(_vendorId, _productId);
    }

    public void Map(RetroPadMapping mapping)
    {
      ClearMappings();
      if (mapping == null)
        return;

      foreach (var kvp in mapping.ButtonMappings)
      {
        DeviceInput deviceInput = kvp.Value;
        if (deviceInput.InputType == InputType.Button)
        {
          if (ushort.TryParse(deviceInput.Id, out ushort button))
          {
            _buttonToButtonMappings.Add(kvp.Key, button);
            _mappedHidButtons.Add(button);
          }
          else if (Enum.TryParse(deviceInput.Id, out DirectionPadState directionPadState))
          {
            _directionPadToButtonMappings.Add(kvp.Key, directionPadState);
            _mappedHidDirectionPadStates.Add(directionPadState);
          }
        }
        else if (deviceInput.InputType == InputType.Axis)
        {
          if (ushort.TryParse(deviceInput.Id, out ushort axis))
          {
            _analogToButtonMappings.Add(kvp.Key, new HidAxis(axis, deviceInput.PositiveValues));
            _mappedHidAxis.Add(axis);
          }
        }
      }

      foreach (var kvp in mapping.AnalogMappings)
      {
        DeviceInput deviceInput = kvp.Value;
        if (deviceInput.InputType == InputType.Button)
        {
          if (ushort.TryParse(deviceInput.Id, out ushort button))
          {
            _buttonToAnalogMappings.Add(kvp.Key, button);
            _mappedHidButtons.Add(button);
          }
          else if (Enum.TryParse(deviceInput.Id, out DirectionPadState directionPadState))
          {
            _directionPadToAnalogMappings.Add(kvp.Key, directionPadState);
            _mappedHidDirectionPadStates.Add(directionPadState);
          }
        }
        else if (deviceInput.InputType == InputType.Axis)
        {
          if (ushort.TryParse(deviceInput.Id, out ushort axis))
          {
            _analogToAnalogMappings.Add(kvp.Key, new HidAxis(axis, deviceInput.PositiveValues));
            _mappedHidAxis.Add(axis);
          }
        }
      }
    }

    protected void ClearMappings()
    {
      _buttonToButtonMappings.Clear();
      _analogToButtonMappings.Clear();
      _directionPadToButtonMappings.Clear();
      _analogToAnalogMappings.Clear();
      _buttonToAnalogMappings.Clear();
      _directionPadToAnalogMappings.Clear();

      _mappedHidButtons.Clear();
      _mappedHidAxis.Clear();
      _mappedHidDirectionPadStates.Clear();
    }

    protected override bool HandleEvent(Event hidEvent)
    {
      HidState state = HidUtils.GetGamepadState(hidEvent);
      _currentState = state;
      return IsAnyMappedInputPressed(state);
    }

    protected bool IsAnyMappedInputPressed(HidState state)
    {
      if (state.Buttons.Any(b => _mappedHidButtons.Contains(b)))
        return true;

      if (_mappedHidDirectionPadStates.Contains(state.DirectionPadState))
          return true;

      if (state.AxisStates.Values.Any(a => (a.Value > _axisDeadZone || a.Value < -_axisDeadZone) && _mappedHidAxis.Contains(a.Index)))
        return true;

      return false;
    }

    public bool IsButtonPressed(uint port, RETRO_DEVICE_ID_JOYPAD button)
    {
      HidState state = _currentState;
      if (state == null)
        return false;

      if (_buttonToButtonMappings.TryGetValue(button, out ushort hidButton))
        return IsButtonPressed(hidButton, state);

      if (_directionPadToButtonMappings.TryGetValue(button, out DirectionPadState directionPadState))
        return IsDirectionPadPressed(directionPadState, state);

      if (_analogToButtonMappings.TryGetValue(button, out HidAxis axis))
        return IsAxisPressed(axis, state, _axisDeadZone);

      return false;
    }

    public short GetAnalog(uint port, RETRO_DEVICE_INDEX_ANALOG index, RETRO_DEVICE_ID_ANALOG direction)
    {
      HidState state = _currentState;
      if (state == null)
        return 0;

      RetroAnalogDevice positive;
      RetroAnalogDevice negative;
      RetroPadMapping.GetAnalogEnums(index, direction, out positive, out negative);
      short positivePosition = 0;
      short negativePosition = 0;

      HidAxis axis;
      ushort button;
      DirectionPadState directionPadState;
      if (_analogToAnalogMappings.TryGetValue(positive, out axis))
        positivePosition = GetAxisPositionMapped(axis, state, true);
      else if (_directionPadToAnalogMappings.TryGetValue(positive, out directionPadState) && IsDirectionPadPressed(directionPadState, state))
        positivePosition = short.MaxValue;
      else if (_buttonToAnalogMappings.TryGetValue(positive, out button) && IsButtonPressed(button, state))
        positivePosition = short.MaxValue;

      if (_analogToAnalogMappings.TryGetValue(negative, out axis))
        negativePosition = GetAxisPositionMapped(axis, state, false);
      else if (_directionPadToAnalogMappings.TryGetValue(negative, out directionPadState) && IsDirectionPadPressed(directionPadState, state))
        positivePosition = short.MinValue;
      else if (_buttonToAnalogMappings.TryGetValue(negative, out button) && IsButtonPressed(button, state))
        negativePosition = short.MinValue;

      if (positivePosition != 0 && negativePosition == 0)
        return positivePosition;
      if (positivePosition == 0 && negativePosition != 0)
        return negativePosition;
      return 0;
    }

    public static bool IsButtonPressed(ushort button, HidState state)
    {
      return state.Buttons.Contains(button);
    }

    public static bool IsDirectionPadPressed(DirectionPadState directionPadState, HidState state)
    {
      return state.DirectionPadState == directionPadState;
    }

    public static bool IsAxisPressed(HidAxis axis, HidState state, short deadZone)
    {
      if (!state.AxisStates.TryGetValue(axis.Axis, out HidAxisState axisState))
        return false;
      short value = NumericUtils.UIntToShort(axisState.Value);
      return axis.PositiveValues ? value > deadZone : value < -deadZone;
    }

    public static short GetAxisPosition(HidAxis axis, HidState state)
    {
      if (!state.AxisStates.TryGetValue(axis.Axis, out HidAxisState axisState))
        return 0;
      return NumericUtils.UIntToShort(axisState.Value);
    }

    public static short GetAxisPositionMapped(HidAxis axis, HidState state, bool isMappedToPositive)
    {
      short position = GetAxisPosition(axis, state);
      if (position == 0 || (axis.PositiveValues && position <= 0) || (!axis.PositiveValues && position >= 0))
        return 0;

      bool shouldInvert = (axis.PositiveValues && !isMappedToPositive) || (!axis.PositiveValues && isMappedToPositive);
      if (shouldInvert)
        position = (short)(-position - 1);
      return position;
    }
  }
}