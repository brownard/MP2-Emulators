using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulators.LibRetro.Controllers.Hid
{
  public interface IHidDevice
  {
    string Mp2DeviceId { get; }
    bool UpdateState(HidState state);
  }
}
