using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SharpRetro.Native
{
  public class Library : ILibrary
  {
    SafeLibraryHandle _hModule;

    public Library(string dllPath)
    {
      LoadLibrary(dllPath);
    }

    public Delegate GetProcDelegate(string procName, Type type)
    {
      IntPtr ptr = GetProcAddress(procName);
      return ptr != IntPtr.Zero ? Marshal.GetDelegateForFunctionPointer(ptr, type) : null;
    }

    protected void LoadLibrary(string dllPath)
    {
      //try to locate dlls in the current directory (for libretro cores)
      //this isnt foolproof but its a little better than nothing
      string path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
      try
      {
        string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string dllDirectory = Path.GetDirectoryName(dllPath);
        string alteredPath = string.Format("{0};{1};{2}", assemblyDirectory, dllDirectory, path);
        Environment.SetEnvironmentVariable("PATH", alteredPath, EnvironmentVariableTarget.Process);
        _hModule = NativeMethods.LoadLibrary(dllPath);
        if (_hModule.IsInvalid)
        {
          int hr = Marshal.GetHRForLastWin32Error();
          Marshal.ThrowExceptionForHR(hr);
        }
      }
      finally
      {
        Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);
      }
    }

    protected IntPtr GetProcAddress(string procName)
    {
      return NativeMethods.GetProcAddress(_hModule, procName);
    }

    public void Dispose()
    {
      if (!_hModule.IsClosed)
        _hModule.Close();
    }
  }
}