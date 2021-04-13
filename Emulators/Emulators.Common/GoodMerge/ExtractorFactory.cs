using System.IO;

namespace Emulators.Common.GoodMerge
{
  public static class ExtractorFactory
  {
    public static IExtractor Create(string path, Stream stream = null)
    {
      return new SharpCompressExtractor(path, stream);
    }
  }
}