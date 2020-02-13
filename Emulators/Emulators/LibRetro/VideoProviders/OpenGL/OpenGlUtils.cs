using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Emulators.LibRetro.VideoProviders.OpenGL
{
  public static class OpenGlUtils
  {
    [DllImport("opengl32", EntryPoint = "wglGetProcAddress", ExactSpelling = true)]
    public static extern IntPtr wglGetProcAddress(IntPtr function_name);
       
    public static void TryDispose<T>(ref T obj)
    {
      if (obj is IDisposable d)
        d.Dispose();
      obj = default(T);
    }
  }
}
