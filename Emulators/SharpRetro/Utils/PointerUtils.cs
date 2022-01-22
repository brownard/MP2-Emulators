using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SharpRetro.Utils
{
  public static class PointerUtils
  {
    public delegate bool EnumeratedItemDelegate<T>(ref T item);

    public static void EnumerateUnmanagedArray<T>(IntPtr arrayOfT, EnumeratedItemDelegate<T> takeWhile) where T : struct
    {
      int size = Marshal.SizeOf(typeof(T));
      for (int i = 0; ; i++)
      {
        T value = Marshal.PtrToStructure<T>(IntPtr.Add(arrayOfT, i * size));
        if (!takeWhile(ref value))
          return;
      }
    }
  }
}
