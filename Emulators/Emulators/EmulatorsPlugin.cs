using Emulators.MediaExtensions;
using MediaPortal.Common.PluginManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulators
{
  public class EmulatorsPlugin : IPluginStateTracker
  {
    public void Activated(PluginRuntime pluginRuntime)
    {
      GamesLibrary.RegisterOnMediaLibrary();

      // Init OpenGl here as it creates a hidden window which we later
      // use to create an opengl context for cores that support hardware
      // rendering. The window should be created on the main thread, which
      // we should be on here, otherwise the window's device context
      // becomes invalid when the thread that created it dies.
      OpenGL.Gl.Initialize();
    }

    public bool RequestEnd()
    {
      return true;
    }

    public void Stop()
    {
    }

    public void Continue()
    {
    }

    public void Shutdown()
    {
    }
  }
}
