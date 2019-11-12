using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulators.LibRetro.VideoProviders
{
  public class SafeTexture : Texture
  {
    protected readonly object _syncRoot = new object();
    protected bool _isDisposing;

    public SafeTexture(Device device, int width, int height, int levelCount, Usage usage, Format format, Pool pool)
      : base(device, width, height, levelCount, usage, format, pool)
    {
      Disposing += OnDisposing;
    }

    public SafeTexture(Device device, int width, int height, int levelCount, Usage usage, Format format, Pool pool, ref IntPtr sharedHandle)
      : base(device, width, height, levelCount, usage, format, pool, ref sharedHandle)
    {
      Disposing += OnDisposing;
    }

    public object SyncRoot
    {
      get { return _syncRoot; }
    }

    public bool IsDisposing
    {
      get { return _isDisposing; }
    }

    protected void OnDisposing(object sender, EventArgs e)
    {
      lock (_syncRoot)
        _isDisposing = true;
    }
  }
}