using Emulators.LibRetro.Settings;
using Emulators.Settings.Configuration.ConfigurationClasses;

namespace Emulators.Settings.Configuration.LibRetro
{
  public class AutoSaveSetting : YesNoNumberSelect
  {
    public override void Load()
    {
      LibRetroSettings settings = SettingsManager.Load<LibRetroSettings>();
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = int.MaxValue;
      _value = settings.AutoSaveInterval;
      _yes = settings.AutoSave;
    }

    public override void Save()
    {
      LibRetroSettings settings = SettingsManager.Load<LibRetroSettings>();
      settings.AutoSaveInterval = (int)_value;
      settings.AutoSave = _yes;
      SettingsManager.Save(settings);
    }
  }
}
