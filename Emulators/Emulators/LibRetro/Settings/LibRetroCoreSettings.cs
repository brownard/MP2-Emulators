using MediaPortal.Common.Settings;
using SharpRetro.LibRetro;
using System.Collections.Generic;
using System.Linq;

namespace Emulators.LibRetro.Settings
{
  public class LibRetroCoreSettings
  {
    protected List<CoreSetting> _coreSettings;
    [Setting(SettingScope.User, null)]
    public List<CoreSetting> CoreSettings
    {
      get
      {
        if (_coreSettings == null)
          _coreSettings = new List<CoreSetting>();
        return _coreSettings;
      }
      set { _coreSettings = value; }
    }

    public bool TryGetCoreSetting(string corePath, out CoreSetting coreSetting)
    {
      coreSetting = CoreSettings.FirstOrDefault(s => s.CorePath == corePath);
      return coreSetting != null;
    }

    public void AddOrUpdateCoreSetting(string corePath, List<VariableDescription> variables)
    {
      List<CoreOption> options = variables.Select(v => new CoreOption { Name = v.Name, Value = v.SelectedOption }).ToList();
      CoreSetting coreSetting = CoreSettings.FirstOrDefault(s => s.CorePath == corePath);
      if (coreSetting != null)
        coreSetting.Options = options;
      else
        CoreSettings.Add(new CoreSetting() { CorePath = corePath, Options = options });
    }
  }

  public class CoreSetting
  {
    protected List<CoreOption> _options;
    public string CorePath { get; set; }
    public List<CoreOption> Options
    {
      get
      {
        if (_options == null)
          _options = new List<CoreOption>();
        return _options;
      }
      set { _options = value; }
    }
  }

  public class CoreOption
  {
    public string Name { get; set; }
    public string Value { get; set; }
  }
}