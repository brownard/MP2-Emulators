using Emulators.LibRetro.VideoProviders.OpenGL.Objects;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulators.LibRetro.VideoProviders.OpenGL
{
  public class FrontendRenderContext : IDisposable
  {
    protected Framebuffer _framebuffer;
    protected Texture2D _textureBuffer;

    public Framebuffer Framebuffer
    {
      get { return _framebuffer; }
    }

    public Texture2D TextureBuffer
    {
      get { return _textureBuffer; }
    }

    public virtual void Init()
    {
    }

    public virtual void ReadPixels(int width, int height, IntPtr data)
    {
      if (_framebuffer == null || _textureBuffer == null)
        return;
      _framebuffer.Bind(FramebufferTarget.Framebuffer);
      Gl.ReadBuffer(ReadBufferMode.ColorAttachment0);
      Gl.ReadPixels(0, 0, width, height, PixelFormat.Bgra, PixelType.UnsignedByte, data);
      _framebuffer.Unbind(FramebufferTarget.Framebuffer);
    }

    public virtual void BeginRender(int width, int height)
    {
      if (_framebuffer != null)
        _framebuffer.Bind(FramebufferTarget.Framebuffer);
    }

    public virtual void EndRender()
    {
      if (_framebuffer != null)
        _framebuffer.Unbind(FramebufferTarget.Framebuffer);
    }

    public void CreateBuffers(int width, int height)
    {
      if (width < 1 || height < 1)
        return;

      _framebuffer = new Framebuffer();
      _framebuffer.Bind(FramebufferTarget.Framebuffer);
      CreateTextureBuffer(width, height);      
      _framebuffer.Unbind(FramebufferTarget.Framebuffer);
    }

    public void DestroyBuffers()
    {
      DestroyTextureBuffer();
      OpenGlUtils.TryDispose(ref _framebuffer);
    }

    protected virtual void CreateTextureBuffer(int width, int height)
    {
      _textureBuffer = new Texture2D();
      _textureBuffer.Bind();
      _textureBuffer.SetStorage(width, height);
      _framebuffer.AttachTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _textureBuffer, 0);
      _textureBuffer.Unbind();
    }

    protected virtual void DestroyTextureBuffer()
    {
      OpenGlUtils.TryDispose(ref _textureBuffer);
    }

    public virtual void Dispose()
    {
      DestroyBuffers();
    }
  }
}
