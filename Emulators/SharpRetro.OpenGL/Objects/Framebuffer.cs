using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRetro.OpenGL.Objects
{
  public class Framebuffer : GLObject
  {
    public void Bind(FramebufferTarget target)
    {
      Gl.BindFramebuffer(target, _id);
    }

    public void Unbind(FramebufferTarget target)
    {
      Gl.BindFramebuffer(target, 0);
    }

    public void AttachTexture(FramebufferTarget target, FramebufferAttachment attachment, Texture2D texture, int level)
    {
      Gl.FramebufferTexture2D(target, attachment, TextureTarget.Texture2d, texture.Id, level);
    }

    public void AttachRenderBuffer(FramebufferTarget target, FramebufferAttachment attachment, RenderBuffer renderBuffer)
    {
      Gl.FramebufferRenderbuffer(target, attachment, RenderbufferTarget.Renderbuffer, renderBuffer.Id);
    }

    protected override uint Generate()
    {
      return Gl.GenFramebuffer();
    }

    protected override void Delete(uint id)
    {
      Gl.DeleteFramebuffers(id);
    }
  }
}
