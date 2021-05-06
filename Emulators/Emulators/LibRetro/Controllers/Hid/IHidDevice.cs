namespace Emulators.LibRetro.Controllers.Hid
{
  public interface IHidDevice
  {
    string Mp2DeviceId { get; }
    bool UpdateState(HidState state);

    /// <summary>
    /// Returns whether the key code, as specified by the InputDeviceManager is mapped to a libretro input for this HID device.
    /// </summary>
    /// <param name="keyCode">The InputDEviceManager key code.</param>
    /// <returns><c>True</c> if the key code is mapped, else <c>false</c>.</returns>
    bool IskeyCodeMapped(long keyCode);
  }
}
