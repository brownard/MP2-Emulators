using OpenGL;
using SharpRetro.OpenGL.Objects;
using System;

namespace SharpRetro.OpenGL.Render
{
  public class LibretroRenderContext : TextureRenderContext
  {    
    protected bool _depth;
    protected bool _stencil;
    protected bool _bottomLeftOrigin;
    
    protected RenderBuffer _depthBuffer;

    public LibretroRenderContext(bool depth, bool stencil, bool bottomLeftOrigin)
    {
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

    public RenderBuffer DepthBuffer
    {
      get { return _depthBuffer; }
    }

    protected override void CreateBuffersOverride(int width, int height)
    {
      base.CreateBuffersOverride(width, height);

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
    }

    protected override void DestroyBuffersOverride()
    {
      base.DestroyBuffersOverride();
      GLObject.TryDispose(ref _depthBuffer);
    }
  }
}
