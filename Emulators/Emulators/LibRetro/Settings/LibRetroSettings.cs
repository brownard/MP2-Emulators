using Emulators.Common;
using Emulators.LibRetro.Render;
using Emulators.LibRetro.SoundProviders;
using MediaPortal.Common;
using MediaPortal.Common.PathManager;
using MediaPortal.Common.Settings;
using System;

namespace Emulators.LibRetro.Settings
{
  public class LibRetroSettings
  {
    protected static readonly string DEFAULT_CORES_DIRECTORY = ServiceRegistration.Get<IPathManager>().GetPath(@"<DATA>\LibRetro\cores");
    protected static readonly string DEFAULT_CORES_DIRECTORY_64BIT = ServiceRegistration.Get<IPathManager>().GetPath(@"<DATA>\LibRetro\cores\x64");
    protected static readonly string DEFAULT_INFO_DIRECTORY = ServiceRegistration.Get<IPathManager>().GetPath(@"<DATA>\LibRetro\info");
    protected static readonly string DEFAULT_SAVES_DIRECTORY = ServiceRegistration.Get<IPathManager>().GetPath(@"<DATA>\LibRetro\saves");
    protected static readonly string DEFAULT_SYSTEM_DIRECTORY = ServiceRegistration.Get<IPathManager>().GetPath(@"<DATA>\LibRetro\system");

    protected string _coresDirectory;
    protected string _coresDirectory64Bit;
    protected string _infoDirectory;
    protected string _savesDirectory;
    protected string _systemDirectory;

    public string GetPlatformSpecificCoresDirectory()
    {
      return Utils.IsCurrentPlatform64Bit() ? CoresDirectory64Bit : CoresDirectory;
    }

    public void SetPlatformSpecificCoresDirectory(string directory)
    {
      if (Utils.IsCurrentPlatform64Bit())
        CoresDirectory64Bit = directory;
      else
        CoresDirectory = directory;
    }

    [Setting(SettingScope.Global)]
    public string CoresDirectory
    {
      get { return string.IsNullOrEmpty(_coresDirectory) ? DEFAULT_CORES_DIRECTORY : _coresDirectory; }
      set { _coresDirectory = value; }
    }

    [Setting(SettingScope.Global)]
    public string CoresDirectory64Bit
    {
      get { return string.IsNullOrEmpty(_coresDirectory64Bit) ? DEFAULT_CORES_DIRECTORY_64BIT : _coresDirectory64Bit; }
      set { _coresDirectory64Bit = value; }
    }

    [Setting(SettingScope.Global)]
    public string InfoDirectory
    {
      get { return string.IsNullOrEmpty(_infoDirectory) ? DEFAULT_INFO_DIRECTORY : _infoDirectory; }
      set { _infoDirectory = value; }
    }

    [Setting(SettingScope.Global)]
    public string SavesDirectory
    {
      get { return string.IsNullOrEmpty(_savesDirectory) ? DEFAULT_SAVES_DIRECTORY : _savesDirectory; }
      set { _savesDirectory = value; }
    }

    [Setting(SettingScope.Global)]
    public string SystemDirectory
    {
      get { return string.IsNullOrEmpty(_systemDirectory) ? DEFAULT_SYSTEM_DIRECTORY : _systemDirectory; }
      set { _systemDirectory = value; }
    }

    [Setting(SettingScope.User, 60d)]
    public double CoreUpdateIntervalMinutes { get; set; }

    [Setting(SettingScope.User, true)]
    public bool OnlyShowSupportedCores { get; set; }

    [Setting(SettingScope.User, 4)]
    public int MaxPlayers { get; set; }

    [Setting(SettingScope.User, true)]
    public bool AutoSave { get; set; }

    [Setting(SettingScope.User, 30)]
    public int AutoSaveInterval { get; set; }

    [Setting(SettingScope.User, false)]
    public bool EnableStateBuffer { get; set; }

    [Setting(SettingScope.User, 30)]
    public int StateBufferDurationSeconds { get; set; }

    [Setting(SettingScope.User, SynchronizationType.Audio)]
    public SynchronizationType SynchronizationType { get; set; }

    [Setting(SettingScope.User, LibRetroDirectSound.DEFAULT_BUFFER_SIZE_SECONDS)]
    public double AudioBufferSize { get; set; }

    [Setting(SettingScope.User)]
    public Guid AudioDeviceId { get; set; }
  }
}