using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulators.LibRetro.VideoProviders.OpenGL.Objects
{
  public class Texture2D : AbstractObject
  {
    public void Bind()
    {
      Gl.BindTexture(TextureTarget.Texture2d, _id);
    }

    public void Unbind()
    {
      Gl.BindTexture(TextureTarget.Texture2d, 0);
    }

    public void SetStorage(int width, int height)
    {
      Gl.TextureImage2DEXT(_id, TextureTarget.Texture2d, 0, InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);
      Gl.TextureParameter(_id, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
      Gl.TextureParameter(_id, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
    }

    protected override uint Generate()
    {
      return Gl.GenTexture();
    }

    protected override void Delete(uint id)
    {
      Gl.DeleteTextures(id);
    }
  }
}
