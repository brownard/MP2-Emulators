using Emulators.LibRetro.Settings;
using MediaPortal.Common.Configuration.ConfigurationClasses;

namespace Emulators.Settings.Configuration.LibRetro
{
  public class StateBufferDurationSetting : LimitedNumberSelect
  {
    public override void Load()
    {
      _type = NumberType.Integer;
      _step = 1;
      _lowerLimit = 1;
      _upperLimit = int.MaxValue;
      _value = SettingsManager.Load<LibRetroSettings>().StateBufferDurationSeconds;
    }

    public override void Save()
    {
      LibRetroSettings settings = SettingsManager.Load<LibRetroSettings>();
      settings.StateBufferDurationSeconds = (int)_value;
      SettingsManager.Save(settings);
    }
  }
}
