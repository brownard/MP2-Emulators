using OpenGL;
using SharpRetro.OpenGL.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRetro.OpenGL.Render
{
  public abstract class RenderContext : IRenderContext, IDisposable
  {
    protected Framebuffer _framebuffer;

    protected int _width;
    protected int _height;

    /// <summary>
    /// The framebuffer to render to.
    /// </summary>
    public Framebuffer Framebuffer
    {
      get { return _framebuffer; }
    }

    public void CreateBuffers(int width, int height)
    {
      if (width < 1 && height < 1)
        return;

      if (_framebuffer == null)
        _framebuffer = new Framebuffer();
      else if (width == _width && height == _height)
        return;

      _width = width;
      _height = height;

      _framebuffer.Bind(FramebufferTarget.Framebuffer);
      CreateBuffersOverride(width, height);
      _framebuffer.Unbind(FramebufferTarget.Framebuffer);
    }

    public void DestroyBuffers()
    {
      DestroyBuffersOverride();
      GLObject.TryDispose(ref _framebuffer);
    }

    /// <summary>
    /// Reads the contents of the first color attachment to the provided IntPtr. Callers
    /// should ensure that the IntPtr points to writable memory of at least the required size.
    /// </summary>
    /// <param name="width">The width of the pixel data to read.</param>
    /// <param name="height">The height of the pixel data to read.</param>
    /// <param name="data">POinter to write the pixel data to.</param>
    public virtual void ReadPixels(int width, int height, IntPtr data)
    {
      if (_framebuffer == null)
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

    public virtual void Dispose()
    {
      DestroyBuffers();
    }

    /// <summary>
    /// Called after the framebuffer has been created. Implementations of this method should create and attach any required buffers here. 
    /// </summary>
    /// <param name="width">The maximum required width of the buffers.</param>
    /// <param name="height">The maximum required height of the buffers.</param>
    protected abstract void CreateBuffersOverride(int width, int height);

    /// <summary>
    /// Called before the framebuffer has been destroyed. Implementations of this method should destroy any created buffers here. 
    /// </summary>
    protected abstract void DestroyBuffersOverride();
  }
}
