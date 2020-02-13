using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulators.LibRetro.VideoProviders.OpenGL.Objects
{
  public abstract class AbstractObject : IDisposable
  {
    protected uint _id;

    public AbstractObject()
    {
      _id = Generate();
    }

    public uint Id
    {
      get { return _id; }
    }

    protected abstract uint Generate();

    protected abstract void Delete(uint id);

    public virtual void Dispose()
    {
      if (_id == 0)
        return;
      Delete(_id);
      _id = 0;
    }
  }
}
