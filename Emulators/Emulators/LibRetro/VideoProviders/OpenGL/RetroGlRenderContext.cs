using Emulators.LibRetro.VideoProviders.OpenGL.Objects;
using OpenGL;
using SharpRetro.LibRetro;
using System;

namespace Emulators.LibRetro.VideoProviders.OpenGL
{
  public class RetroGlRenderContext : IDisposable
  {
    // These delegates will be called by the libretro core
    public readonly retro_hw_get_proc_address_t GetProcAddressDelegate;
    public readonly retro_hw_get_current_framebuffer_t GetCurrentFramebufferDlgt;
    
    protected bool _depth;
    protected bool _stencil;
    protected bool _bottomLeftOrigin;

    protected Framebuffer _framebuffer;
    protected Texture2D _textureBuffer;
    protected RenderBuffer _depthBuffer;

    public RetroGlRenderContext(bool depth, bool stencil, bool bottomLeftOrigin)
    {
      GetProcAddressDelegate = new retro_hw_get_proc_address_t(OpenGlUtils.wglGetProcAddress);
      GetCurrentFramebufferDlgt = new retro_hw_get_current_framebuffer_t(GetCurrentFramebuffer);

      _depth = depth;
      _stencil = stencil;
      _bottomLeftOrigin = bottomLeftOrigin;
    }

    public bool Depth
    {
      get { return _depth; }
    }

    public bool Stencil
    {
      get { return _stencil; }
    }

    public bool BottomLeftOrigin
    {
      get { return _bottomLeftOrigin; }
    }

    public Framebuffer Framebuffer
    {
      get { return _framebuffer; }
    }

    public Texture2D TextureBuffer
    {
      get { return _textureBuffer; }
    }

    public RenderBuffer DepthBuffer
    {
      get { return _depthBuffer; }
    }

    public void CreateBuffers(int width, int height)
    {
      _framebuffer = new Framebuffer();
      _framebuffer.Bind(FramebufferTarget.Framebuffer);

      _textureBuffer = new Texture2D();
      _textureBuffer.Bind();
      _textureBuffer.SetStorage(width, height);
      _framebuffer.AttachTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _textureBuffer, 0);
      _textureBuffer.Unbind();

      if (_depth)
      {
        _depthBuffer = new RenderBuffer();
        _depthBuffer.Bind();
        if (_stencil)
        {
          _depthBuffer.SetStorage(InternalFormat.Depth24Stencil8, width, height);
          _framebuffer.AttachRenderBuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, _depthBuffer);
        }
        else
        {
          _depthBuffer.SetStorage(InternalFormat.DepthComponent24, width, height);
          _framebuffer.AttachRenderBuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, _depthBuffer);
        }
        _depthBuffer.Unbind();
      }

      _framebuffer.Unbind(FramebufferTarget.Framebuffer);
    }

    public void DestroyBuffers()
    {
      OpenGlUtils.TryDispose(ref _textureBuffer);
      OpenGlUtils.TryDispose(ref _depthBuffer);
      OpenGlUtils.TryDispose(ref _framebuffer);
    }

    protected uint GetCurrentFramebuffer()
    {
      return _framebuffer?.Id ?? 0;
    }

    public void Dispose()
    {
      DestroyBuffers();
    }
  }
}
