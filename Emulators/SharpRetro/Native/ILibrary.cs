using System;

namespace SharpRetro.Native
{
  public interface ILibrary : IDisposable
  {
    Delegate GetProcDelegate(string procName, Type type);
  }
}