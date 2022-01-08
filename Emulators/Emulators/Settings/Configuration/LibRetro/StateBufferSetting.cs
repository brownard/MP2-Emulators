using Emulators.LibRetro.Settings;
using MediaPortal.Common.Configuration.ConfigurationClasses;

namespace Emulators.Settings.Configuration.LibRetro
{
  public class StateBufferSetting : YesNo
  {
    public override void Load()
    {
      var settings = SettingsManager.Load<LibRetroSettings>();
      _yes = settings.EnableStateBuffer;
    }

    public override void Save()
    {
      LibRetroSettings settings = SettingsManager.Load<LibRetroSettings>();
      settings.EnableStateBuffer = _yes;
      SettingsManager.Save(settings);
    }
  }
}
