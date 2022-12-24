using Emulators.LibRetro.Controllers.Mapping;
using SharpLib.Hid;
using SharpRetro.Controller;
using SharpRetro.LibRetro;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Emulators.LibRetro.Controllers.Hid
{
  class HidKeyboard : AbstractHidDevice, IRetroPad, IRetroAnalog, IMappableDevice, IKeyboard
  {
    public static readonly Guid DEVICE_ID = new Guid("7910C3E3-F3D2-405F-B09B-8C73EEFB6C70");

    protected HashSet<Keys> _pressedkeys;
    protected Dictionary<RETRO_DEVICE_ID_JOYPAD, Keys> _buttonMappings;
    protected Dictionary<RetroAnalogDevice, Keys> _analogMappings;

    protected HashSet<Keys> _mappedKeys;

    public HidKeyboard()
      : base(0, 0, "Keyboard")
    {
      _pressedkeys = new HashSet<Keys>();
      _buttonMappings = new Dictionary<RETRO_DEVICE_ID_JOYPAD, Keys>();
      _analogMappings = new Dictionary<RetroAnalogDevice, Keys>();

      _mappedKeys = new HashSet<Keys>();
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
      ClearMappings();
      if (mapping == null)
        return;

      foreach (var kvp in mapping.ButtonMappings)
      {
        if (Enum.TryParse(kvp.Value.Id, out Keys key))
        {
          _buttonMappings[kvp.Key] = key;
          _mappedKeys.Add(key);
        }
      }

      foreach (var kvp in mapping.AnalogMappings)
      {
        if (Enum.TryParse(kvp.Value.Id, out Keys key))
        {
          _analogMappings[kvp.Key] = key;
          _mappedKeys.Add(key);
        }
      }
    }

    protected void ClearMappings()
    {
      _buttonMappings.Clear();
      _analogMappings.Clear();

      _mappedKeys.Clear();
    }

    public bool IsButtonPressed(uint port, RETRO_DEVICE_ID_JOYPAD button)
    {
      return _buttonMappings.TryGetValue(button, out Keys key) && _pressedkeys.Contains(key);
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
      return HandleEvent(hidEvent);
    }

    protected override bool HandleEvent(Event hidEvent)
    {
      Keys key = hidEvent.VirtualKey;
      if (key <= 0 || !_mappedKeys.Contains(key))
        return false;

      if (hidEvent.IsButtonDown)
        _pressedkeys.Add(key);
      else
        _pressedkeys.Remove(key);
      return true;
    }
  }
}
