using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRetro.OpenGL.Objects
{
  public class RenderBuffer : GLObject
  {
    public void Bind()
    {
      Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _id);
    }

    public void Unbind()
    {
      Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
    }

    public void SetStorage(InternalFormat internalFormat, int width, int height)
    {
      Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, internalFormat, width, height);
    }

    protected override uint Generate()
    {
      return Gl.GenRenderbuffer();
    }

    protected override void Delete(uint id)
    {
      Gl.DeleteRenderbuffers(id);
    }
  }
}
