using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SharpRetro.LibRetro
{
  public class LibRetroVariables
  {
    protected Dictionary<string, VariableDescription> _variables = new Dictionary<string, VariableDescription>();
    protected bool _updated;

    public bool Updated
    {
      get { return _updated; }
    }

    public List<VariableDescription> GetAllVariables()
    {
      return _variables.Values.ToList();
    }

    public bool Contains(string variableName)
    {
      return _variables.ContainsKey(variableName);
    }

    public void AddOrUpdate(VariableDescription variable)
    {
      VariableDescription vd;
      if (_variables.TryGetValue(variable.Name, out vd))
        variable.SelectedOption = vd.SelectedOption;
      _variables[variable.Name] = variable;
      _updated = true;
    }

    public void AddOrUpdate(string variableName, string selectedOption)
    {
      VariableDescription vd;
      if (_variables.TryGetValue(variableName, out vd))
        vd.SelectedOption = selectedOption;
      else
        _variables[variableName] = new VariableDescription() { Name = variableName, SelectedOption = selectedOption };
      _updated = true;
    }

    public bool TryGet(string variableName, out VariableDescription variable)
    {
      _updated = false;
      return _variables.TryGetValue(variableName, out variable);
    }
  }

  public class VariableDescription
  {
    protected string _selectedOption;
    protected string _defaultOption;

    public VariableDescription() { }

    public VariableDescription(IntPtr keyPtr, IntPtr variablePtr)
    {
      string key = Marshal.PtrToStringAnsi(keyPtr);
      string[] parts = Marshal.PtrToStringAnsi(variablePtr).Split(';');

      Name = key;
      Description = parts[0];
      Options = parts[1].TrimStart(' ').Split('|');
    }

    public VariableDescription(ref retro_core_option_definition option)
    {
      Name = option.key;
      Description = option.desc;
      Info = option.info;
      Options = option.values.TakeWhile(v => v.value != null).Select(v => v.value).ToArray();
      _defaultOption = option.default_value;
    }

    public VariableDescription(ref retro_core_option_v2_definition option)
    {
      Name = option.key;
      Description = option.desc;
      Info = option.info;
      Options = option.values.TakeWhile(v => v.value != null).Select(v => v.value).ToArray();
      _defaultOption = option.default_value;
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public string Info { get; set; }
    public string[] Options { get; set; }

    public string DefaultOption
    {
      get 
      { 
        return IsValidOption(_defaultOption) ? _defaultOption :
          Options != null && Options.Length > 0 ? Options[0] : ""; 
      }
    }

    public string SelectedOption
    {
      get
      {
        return IsValidOption(_selectedOption) ? _selectedOption : DefaultOption;
      }
      set { _selectedOption = value; }
    }

    protected bool IsValidOption(string option)
    {
      return option != null && Options != null && Options.Contains(option);
    }

    public override string ToString()
    {
      return string.Format("Name: {0}, Description: {1}, Options: {2}", Name, Description, string.Join("|", Options));
    }
  }
}