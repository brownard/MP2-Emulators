using Microsoft.Win32.SafeHandles;
using System.Security.Permissions;

namespace SharpRetro.Native
{
  [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
  sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
  {
    private SafeLibraryHandle() : base(true) { }

    protected override bool ReleaseHandle()
    {
      return NativeMethods.FreeLibrary(handle);
    }
  }
}
