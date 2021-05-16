using Emulators.LibRetro.Controllers.Mapping;
using MediaPortal.Plugins.InputDeviceManager;
using SharpLib.Hid;
using System.Linq;
using System.Windows.Forms;

namespace Emulators.LibRetro.Controllers.Hid
{
  class HidkeyboardMapper : AbstractHidMapper
  {
    protected Keys? _pressedkey = null;

    public override bool SupportsDeadZone
    {
      get { return false; }
    }

    public override void BeginMapping()
    {
      _pressedkey = null;
      base.BeginMapping();
    }

    public override DeviceInput GetPressedInput()
    {
      if (!_pressedkey.HasValue)
        return null;
      return new DeviceInput(_pressedkey.ToString(), _pressedkey.ToString(), InputType.Button);
    }

    protected override void HidListener_StateChanged(object sender, Event e)
    {
      if (!e.IsKeyboard || e.VirtualKey <= 0)
        return;
      if (e.IsButtonDown)
        _pressedkey = e.VirtualKey;
      else if (e.VirtualKey == _pressedkey)
        _pressedkey = null;
    }

    protected override void ExternalKeyHandler(object sender, KeyPressHandlerEventArgs e)
    {
      if (e.PressedKeys.Any(kvp => kvp.Value > 0 && kvp.Value < 1000))
        e.Handled = true;
    }
  }
}
