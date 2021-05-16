using MediaPortal.Common;
using MediaPortal.Common.Logging;
using SharpLib.Hid;
using System.Collections.Generic;
using System.Globalization;

namespace Emulators.LibRetro.Controllers.Hid
{
  abstract class AbstractHidDevice : IHidDevice
  {
    protected string _subDeviceId;
    protected ushort _vendorId;
    protected ushort _productId;
    protected string _name;
    protected string _friendlyName;
    protected string _mp2DeviceId;

    protected Dictionary<long, bool> _knownMP2KeyCodes = new Dictionary<long, bool>();

    public AbstractHidDevice(ushort vendorId, ushort productId, string friendlyName)
    {
      _vendorId = vendorId;
      _productId = productId;
      _subDeviceId = vendorId > 0 || productId > 0 ? string.Format(CultureInfo.InvariantCulture, "{0}/{1}", vendorId, productId) : string.Empty;
      _friendlyName = friendlyName;

      // This is the same id used in the InputDeviceManager plugin,
      // used to tell it not to handle events from this device.
      // We could use this instead of our subDeviceId, but to preserve
      // existing libretro mappings we'll keep it separate
      _mp2DeviceId = ((vendorId << 16) | productId).ToString("X");
    }

    public string SubDeviceId
    {
      get { return _subDeviceId; }
    }

    public string DeviceName
    {
      get { return _friendlyName; }
    }

    public string Mp2DeviceId
    {
      get { return _mp2DeviceId; }
    }

    /// <summary>
    /// Returns whether the key code, as specified by the InputDeviceManager, is mapped to a libretro input for this HID device.
    /// </summary>
    /// <param name="keyCode">The InputDEviceManager key code.</param>
    /// <returns><c>True</c> if the key code is mapped, else <c>false</c>.</returns>
    public virtual bool IskeyCodeMapped(long keyCode)
    {
      bool isMapped;

      if (_knownMP2KeyCodes.TryGetValue(keyCode, out isMapped))
        return isMapped;

      isMapped = IsKeyCodeMappedOverride(keyCode);

      return _knownMP2KeyCodes[keyCode] = isMapped;
    }

    protected abstract bool IsKeyCodeMappedOverride(long keyCode);

    public virtual bool UpdateState(Event hidEvent)
    {
      if (_name != null)
      {
        if ((hidEvent.Device?.Name ?? "Unknown Device") != _name)
          return false;
      }
      else
      {
        if (hidEvent.Device?.ProductId != _productId || hidEvent.Device?.VendorId != _vendorId)
          return false;
        _name = hidEvent.Device?.Name ?? "Unknown Device";
        ServiceRegistration.Get<ILogger>().Debug("HidDevice: Mapped Hid controller configuration to device {0}, {1}, {2}", _name, _productId, _vendorId);
      }

      HandleEvent(hidEvent);
      return true;
    }

    protected abstract void HandleEvent(Event hidEvent);
  }
}
