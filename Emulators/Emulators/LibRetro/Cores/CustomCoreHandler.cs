using Emulators.Common;
using Emulators.Common.WebRequests;
using MediaPortal.Common;
using MediaPortal.Common.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Emulators.LibRetro.Cores
{
  public static class CustomCoreHandler
  {
    [XmlRootAttribute]
    public class CustomCores
    {
      [XmlElement("CustomCore")]
      public CustomCore[] Cores { get; set; }
    }

    public static List<CustomCore> GetCustomCores(string url)
    {
      if (!string.IsNullOrEmpty(url))
      {
        XmlDownloader downloader = new XmlDownloader();
        CustomCores customCores = downloader.Download<CustomCores>(url);
        if (customCores != null && customCores.Cores != null)
          return new List<CustomCore>(customCores.Cores);
      }

      List<CustomCore> customCoresList = new List<CustomCore>();
      if (!Utils.IsCurrentPlatform64Bit())
      {
        var settings = ServiceRegistration.Get<ISettingsManager>().Load<CoreUpdaterSettings>();
        customCoresList.AddRange(CoreUpdaterSettings.DEFAULT_CUSTOM_CORES);
      }
      return customCoresList;
    }
  }
}
