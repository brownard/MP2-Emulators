using Emulators.LibRetro.Controllers.Mapping;
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

    protected override void HidListener_StateChanged(object sender, StateChangedEventArgs e)
    {
      if (!e.HidEvent.IsKeyboard)
        return;

      Keys key = e.HidEvent.VirtualKey;
      if (key <= 0)
        return;

      if (e.HidEvent.IsButtonDown)
        _pressedkey = key;
      else if (key == _pressedkey)
        _pressedkey = null;
      e.Handled = true;
    }
  }
}
