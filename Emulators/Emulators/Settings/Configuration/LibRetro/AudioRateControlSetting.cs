using Emulators.LibRetro.Settings;
using Emulators.Settings.Configuration.ConfigurationClasses;

namespace Emulators.Settings.Configuration.LibRetro
{
  public class AudioRateControlSetting : YesNoNumberSelect
  {
    public override void Load()
    {
      LibRetroSettings settings = SettingsManager.Load<LibRetroSettings>();
      _type = NumberType.FloatingPoint;
      _step = 0.001;
      _lowerLimit = 0;
      _upperLimit = int.MaxValue;
      _maxNumDigits = 4;
      _value = settings.AudioRateControlDelta;
      _yes = settings.EnableAudioRateControl;
    }

    public override void Save()
    {
      LibRetroSettings settings = SettingsManager.Load<LibRetroSettings>();
      settings.AudioRateControlDelta = _value;
      settings.EnableAudioRateControl = _yes;
      SettingsManager.Save(settings);
    }
  }
}
