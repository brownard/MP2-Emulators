using Emulators.Common.GoodMerge;
using Emulators.Common.WebRequests;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.Settings;
using SharpRetro.Info;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Emulators.LibRetro.Cores
{
  public class CoreHandler
  {
    protected string _baseUrl;
    protected string _latestUrl;
    protected string _infoUrl;
    protected string _customCoresUrl;
    protected string _coresDirectory;
    protected string _infoDirectory;
    protected List<CustomCore> _customCores;
    protected List<LocalCore> _cores;
    protected HashSet<string> _unsupportedCores;
    protected HtmlDownloader _downloader;

    public CoreHandler(string coresDirectory, string infoDirectory)
    {
      _downloader = new HtmlDownloader();
      _cores = new List<LocalCore>();

      _coresDirectory = coresDirectory;
      _infoDirectory = infoDirectory;

      CoreUpdaterSettings settings = ServiceRegistration.Get<ISettingsManager>().Load<CoreUpdaterSettings>();
      _baseUrl = settings.BaseUrl;
      _infoUrl = settings.CoreInfoZipUrl;
      _unsupportedCores = new HashSet<string>(CoreUpdaterSettings.DEFAULT_UNSUPPORTED);

      settings.GetPlatformSpecificCoresUrls(out _latestUrl, out _customCoresUrl);
    }

    public List<LocalCore> Cores
    {
      get { return _cores; }
    }

    public void Update()
    {
      UpdateCustomCores();
      UpdateCoreInfos();
      UpdateCores();
    }

    public bool DownloadCore(LocalCore core)
    {
      if (string.IsNullOrEmpty(core.Url))
        return false;
      if (!TryCreateDirectory(_coresDirectory))
        return false;

      string path = Path.Combine(_coresDirectory, core.ArchiveName);
      return _downloader.DownloadFileAsync(core.Url, path, true).Result && ExtractCore(path);
    }

    public static CoreInfo LoadCoreInfo(string coreName, string infoDirectory)
    {
      try
      {
        string path = Path.Combine(infoDirectory, Path.GetFileNameWithoutExtension(coreName) + ".info");
        if (File.Exists(path))
          return new CoreInfo(coreName, File.ReadAllText(path));
      }
      catch (Exception ex)
      {
        ServiceRegistration.Get<ILogger>().Error("CoreInfoHandler: Exception loading core info for '{0}'", ex, coreName);
      }
      return null;
    }

    protected void UpdateCores()
    {
      List<OnlineCore> onlineCores = new List<OnlineCore>();
      string url = _baseUrl + _latestUrl + ".index-extended";
      CoreList coreList = _downloader.Download<CoreList>(url);
      if (coreList != null)
        onlineCores.AddRange(coreList.CoreUrls.OrderBy(c => c.Name));

      CreateLocalCores(onlineCores);
    }

    protected void UpdateCoreInfos()
    {
      if (!TryCreateDirectory(_infoDirectory))
        return;

      if (!TryCreateAbsoluteUrl(_baseUrl, _infoUrl, out Uri uri))
      {
        ServiceRegistration.Get<ILogger>().Error("CoreHandler: Unable to create absolute core info url from settings, base url: '{0}', info url: '{1}'", _baseUrl, _infoUrl);
        return;
      }

      try
      {
        byte[] data = _downloader.DownloadDataAsync(uri.AbsoluteUri).Result;
        if (data == null || data.Length == 0)
        {
          ServiceRegistration.Get<ILogger>().Error("CoreInfoHandler: Failed to download core infos from '{0}', response was null or empty", uri.AbsoluteUri);
          return;
        }
        using (Stream stream = new MemoryStream(data))
        using (IExtractor extractor = ExtractorFactory.Create(uri.AbsoluteUri, stream))
          extractor.ExtractAll(_infoDirectory);
      }
      catch (Exception ex)
      {
        ServiceRegistration.Get<ILogger>().Error("CoreInfoHandler: Exception updating core infos", ex);
      }
    }

    protected void UpdateCustomCores()
    {
      _customCores = CustomCoreHandler.GetCustomCores(_customCoresUrl);

      if (!TryCreateDirectory(_infoDirectory))
        return;

      foreach (CustomCore customCore in _customCores)
      {
        if (string.IsNullOrEmpty(customCore.InfoUrl))
          continue;
        string path = Path.Combine(_infoDirectory, Path.GetFileNameWithoutExtension(customCore.CoreName) + ".info");
        _downloader.DownloadFileAsync(customCore.InfoUrl, path).Wait();
      }
    }

    protected void CreateLocalCores(IEnumerable<OnlineCore> onlineCores)
    {
      _cores = new List<LocalCore>();
      foreach (CustomCore customCore in _customCores)
      {
        LocalCore core = new LocalCore()
        {
          Url = customCore.CoreUrl,
          CoreName = customCore.CoreName,
          ArchiveName = customCore.CoreName,
          Info = LoadCoreInfo(customCore.CoreName, _infoDirectory)
        };
        _cores.Add(core);
      }

      foreach (OnlineCore onlineCore in onlineCores)
      {
        Uri uri;
        if (!TryCreateAbsoluteUrl(_baseUrl, _latestUrl + onlineCore.Name, out uri))
          continue;

        string coreName = Path.GetFileNameWithoutExtension(onlineCore.Name);
        LocalCore core = new LocalCore()
        {
          Url = uri.AbsoluteUri,
          ArchiveName = onlineCore.Name,
          CoreName = coreName,
          Supported = !_unsupportedCores.Contains(coreName),
          Info = LoadCoreInfo(coreName, _infoDirectory)
        };
        _cores.Add(core);
      }
    }

    protected bool ExtractCore(string path)
    {
      bool extracted;
      using (IExtractor extractor = ExtractorFactory.Create(path))
      {
        if (!extractor.IsArchive())
          return true;
        extracted = extractor.ExtractAll(Path.GetDirectoryName(path));
      }
      if (extracted)
        TryDeleteFile(path);
      return extracted;
    }

    protected static bool TryCreateAbsoluteUrl(string baseUrl, string url, out Uri uri)
    {
      if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
        return false;
      if (uri.IsAbsoluteUri)
        return true;
      Uri baseUri;
      if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out baseUri))
        return false;

      uri = new Uri(baseUri, uri);
      return true;
    }

    protected static bool TryCreateDirectory(string directory)
    {
      try
      {
        Directory.CreateDirectory(directory);
        return true;
      }
      catch (Exception ex)
      {
        ServiceRegistration.Get<ILogger>().Error("CoreHandler: Exception creating directory '{0}'", ex, directory);
      }
      return false;
    }

    protected static bool TryDeleteFile(string path)
    {
      try
      {
        File.Delete(path);
        return true;
      }
      catch (Exception ex)
      {
        ServiceRegistration.Get<ILogger>().Error("CoreHandler: Exception deleting file '{0}'", ex, path);
      }
      return false;
    }
  }
}
