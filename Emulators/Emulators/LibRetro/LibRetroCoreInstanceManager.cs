using MediaPortal.Common;
using MediaPortal.Common.Logging;
using System.Collections.Concurrent;

namespace Emulators.LibRetro
{
  public class LibRetroCoreInstanceManager : ILibRetroCoreInstanceManager
  {
    const byte DUMMY = 0;
    protected ConcurrentDictionary<string, byte> _loadedCores = new ConcurrentDictionary<string, byte>();

    public bool TrySetCoreLoading(string corePath)
    {
      if (corePath == null)
        return false;

      bool loaded = _loadedCores.TryAdd(corePath, DUMMY);
      if (!loaded)
        ServiceRegistration.Get<ILogger>().Warn("LibRetroCoreInstanceManager: Attempt to load a core that was already loaded '{0}'", corePath);
      return loaded;
    }

    public void SetCoreUnloaded(string corePath)
    {
      _loadedCores.TryRemove(corePath, out _);
    }
  }
}
