using System;

namespace SharpRetro.Cores
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
  public sealed class CoreEntryPointAttribute : Attribute
  {
    private string _entryPoint;

    public CoreEntryPointAttribute(string entryPoint)
    {
      _entryPoint = entryPoint;
    }

    public string EntryPoint
    {
      get { return _entryPoint; }
      set { _entryPoint = value; }
    }
  }
}
