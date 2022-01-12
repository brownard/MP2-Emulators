using Emulators.LibRetro.Settings;
using Emulators.Settings.Configuration.ConfigurationClasses;

namespace Emulators.Settings.Configuration.LibRetro
{
  public class StateBufferSetting : YesNoNumberSelect
  {
    public override void Load()
    {
      LibRetroSettings settings = SettingsManager.Load<LibRetroSettings>();
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = int.MaxValue;
      _value = settings.StateBufferDurationSeconds;
      _yes = settings.EnableStateBuffer;
    }

    public override void Save()
    {
      LibRetroSettings settings = SettingsManager.Load<LibRetroSettings>();
      settings.StateBufferDurationSeconds = (int)_value;
      settings.EnableStateBuffer = _yes;
      SettingsManager.Save(settings);
    }
  }
}
