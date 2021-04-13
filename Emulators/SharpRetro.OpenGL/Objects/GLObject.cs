using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRetro.OpenGL.Objects
{
  /// <summary>
  /// Base class for OpenGL objects. Implementations of this class should
  /// implement the <see cref="Generate"/> and <see cref="Delete(uint)"/>
  /// methods
  /// </summary>
  public abstract class GLObject : IDisposable
  {  
    protected uint _id;

    public GLObject()
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

    /// <summary>
    /// Disposes the specified object if it implements <see cref="IDisposable"/>,
    /// and sets it's value to the deafult value for <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object, will be used to determine the default value.</typeparam>
    /// <param name="obj">The object to dispose, the reference will be set to the default value of <typeparamref name="T"/>.</param>
    public static void TryDispose<T>(ref T obj)
    {
      if (obj is IDisposable d)
        d.Dispose();
      obj = default(T);
    }
  }
}
