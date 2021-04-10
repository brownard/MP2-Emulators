using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulators.Common
{
  public static class Utils
  {
    /// <summary>
    /// Convenience method to determine whether the current platform is 64 bit. 
    /// </summary>
    /// <returns>True if the current platform is 64 bit, else false.</returns>
    public static bool IsCurrentPlatform64Bit()
    {
      return IntPtr.Size == 8;
    }
  }
}
