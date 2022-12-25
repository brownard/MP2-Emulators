using MediaPortal.Common;
using MediaPortal.Common.Logging;
using SharpLib.Hid;
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

    public AbstractHidDevice(ushort vendorId, ushort productId, string friendlyName)
    {
      _vendorId = vendorId;
      _productId = productId;
      _subDeviceId = vendorId > 0 || productId > 0 ? string.Format(CultureInfo.InvariantCulture, "{0}/{1}", vendorId, productId) : string.Empty;
      _friendlyName = friendlyName;
    }

    public string SubDeviceId
    {
      get { return _subDeviceId; }
    }

    public string DeviceName
    {
      get { return _friendlyName; }
    }

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

      return HandleEvent(hidEvent);
    }

    protected abstract bool HandleEvent(Event hidEvent);
  }
}
