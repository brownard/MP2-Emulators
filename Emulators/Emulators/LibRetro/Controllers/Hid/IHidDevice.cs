using SharpLib.Hid;

namespace Emulators.LibRetro.Controllers.Hid
{
  public interface IHidDevice
  {
    bool UpdateState(Event hidEvent);
  }
}
