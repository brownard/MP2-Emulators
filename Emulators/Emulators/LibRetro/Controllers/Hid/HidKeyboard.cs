using Emulators.LibRetro.Controllers.Mapping;
using SharpLib.Hid;
using SharpRetro.Controller;
using SharpRetro.LibRetro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Emulators.LibRetro.Controllers.Hid
{
  class HidKeyboard : AbstractHidDevice, IRetroPad, IRetroAnalog, IMappableDevice, IKeyboard
  {
    public static readonly Guid DEVICE_ID = new Guid("7910C3E3-F3D2-405F-B09B-8C73EEFB6C70");

    protected HashSet<Keys> _pressedkeys;
    protected Dictionary<RETRO_DEVICE_ID_JOYPAD, Keys> _buttonMappings;
    protected Dictionary<RetroAnalogDevice, Keys> _analogMappings;

    public HidKeyboard()
      : base(0, 0, "Keyboard")
    {
      _pressedkeys = new HashSet<Keys>();
      _buttonMappings = new Dictionary<RETRO_DEVICE_ID_JOYPAD, Keys>();
      _analogMappings = new Dictionary<RetroAnalogDevice, Keys>();
    }

    public Guid DeviceId
    {
      get { return DEVICE_ID; }
    }

    public RetroPadMapping DefaultMapping
    {
      get { return null; }
    }

    public IDeviceMapper CreateMapper()
    {
      return new HidkeyboardMapper();
    }

    public void Map(RetroPadMapping mapping)
    {
      foreach (var kvp in mapping.ButtonMappings)
      {
        Keys key;
        if (Enum.TryParse(kvp.Value.Id, out key))
          _buttonMappings[kvp.Key] = key;
      }

      foreach (var kvp in mapping.AnalogMappings)
      {
        Keys key;
        if (Enum.TryParse(kvp.Value.Id, out key))
          _analogMappings[kvp.Key] = key;
      }
    }

    public bool IsButtonPressed(uint port, RETRO_DEVICE_ID_JOYPAD button)
    {
      Keys key;
      return _buttonMappings.TryGetValue(button, out key) && _pressedkeys.Contains(key);
    }

    public short GetAnalog(uint port, RETRO_DEVICE_INDEX_ANALOG index, RETRO_DEVICE_ID_ANALOG direction)
    {
      RetroAnalogDevice positive;
      RetroAnalogDevice negative;
      RetroPadMapping.GetAnalogEnums(index, direction, out positive, out negative);

      short positivePosition = 0;
      short negativePosition = 0;

      Keys key;
      if (_analogMappings.TryGetValue(positive, out key) && _pressedkeys.Contains(key))
        positivePosition = short.MaxValue;
      if (_analogMappings.TryGetValue(negative, out key) && _pressedkeys.Contains(key))
        negativePosition = short.MinValue;

      if (positivePosition != 0 && negativePosition == 0)
        return positivePosition;
      if (positivePosition == 0 && negativePosition != 0)
        return negativePosition;
      return 0;
    }

    public override bool UpdateState(Event hidEvent)
    {
      if (!hidEvent.IsKeyboard)
        return false;
      HandleEvent(hidEvent);
      return true;
    }

    protected override void HandleEvent(Event hidEvent)
    {
      if (hidEvent.VirtualKey <= 0)
        return;

      if (hidEvent.IsButtonDown)
        _pressedkeys.Add(hidEvent.VirtualKey);
      else
        _pressedkeys.Remove(hidEvent.VirtualKey);
    }

    protected override bool IsKeyCodeMappedOverride(long keyCode)
    {
      if (keyCode < 1 || keyCode > 1000 || !Enum.IsDefined(typeof(Keys), (int)keyCode))
        return false;
      Keys key = (Keys)keyCode;
      return _buttonMappings.Any(kvp => kvp.Value == key) || _analogMappings.Any(kvp => kvp.Value == key);
    }
  }
}
