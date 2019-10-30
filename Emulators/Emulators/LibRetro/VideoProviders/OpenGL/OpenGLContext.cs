using OpenGL;
using System;
using System.Runtime.InteropServices;

namespace Emulators.LibRetro.VideoProviders.OpenGL
{
  /// <summary>
  /// Represents a libretro compatible OpenGl context that can be used with a <see cref="SharpRetro.Native.retro_hw_render_callback"/>.
  /// </summary>
  public class OpenGLContext : IDisposable, IHardwareContext
  {
    [DllImport("opengl32", EntryPoint = "wglGetProcAddress", ExactSpelling = true)]
    private static extern IntPtr wglGetProcAddress(IntPtr function_name);

    protected DeviceContext _deviceContext;
    protected IntPtr _glContext;

    protected uint _backBuffer;
    protected uint _backTexture;
    protected uint _backDepthBuffer;

    protected uint _frontBuffer;
    protected uint _frontTexture;

    public uint GetCurrentFramebuffer()
    {
      return _backBuffer;
    }

    public IntPtr GetProcAddress(IntPtr sym)
    {
      return wglGetProcAddress(sym);
    }

    public virtual void Create(int width, int height)
    {
      // Clean up any existing context
      Destroy();
            
      _deviceContext = DeviceContext.Create();
      _glContext = _deviceContext.CreateContext(IntPtr.Zero);

      if (_glContext == IntPtr.Zero)
        throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

      _deviceContext.MakeCurrent(_glContext);

      CreateFrameBuffers(width, height);
    }

    public void SetDimensions(int width, int height)
    {
      if (_deviceContext == null)
        Create(width, height);
      else
      {
        DestroyFrameBuffers();
        CreateFrameBuffers(width, height);
      }        
    }

    public virtual void ReadPixels(int width, int height, IntPtr data)
    {
      if (_deviceContext == null)
        return;

      Gl.BindFramebufferEXT(FramebufferTarget.Framebuffer, _frontBuffer);
      Gl.ReadBuffer(ReadBufferMode.ColorAttachment0);
      Gl.ReadPixels(0, 0, width, height, PixelFormat.Bgra, PixelType.UnsignedByte, data);
      Gl.BindFramebufferEXT(FramebufferTarget.Framebuffer, 0);
    }

    /// <summary>
    /// Renders the contents of the back buffer into the front buffer, flipping the
    /// texture to correct the origin if <paramref name="bottomLeftOrigin"/> is <c>true</c>.
    /// </summary>
    /// <param name="width">The width of the back buffer contents to render.</param>
    /// <param name="height">The height of the back buffer contents to render.</param>
    /// <param name="bottomLeftOrigin">Whether the backbuffer's origin is bottom left.</param>
    public virtual void Render(int width, int height, bool bottomLeftOrigin)
    {
      Gl.PushAttrib(AttribMask.TextureBit | AttribMask.DepthBufferBit | AttribMask.LightingBit);
      Gl.Disable(EnableCap.DepthTest);
      Gl.Disable(EnableCap.Lighting);
      Gl.UseProgram(0);

      Gl.MatrixMode(MatrixMode.Projection);
      Gl.PushMatrix();
      Gl.LoadIdentity();

      // Scale by -1 if bottom left origin is true to flip
      // the image to the 'correct' orientation.
      Gl.Scale(1, bottomLeftOrigin ? -1 : 1, 1);

      Gl.Ortho(0d, width, 0d, height, -1d, 1d);

      Gl.MatrixMode(MatrixMode.Modelview);
      Gl.PushMatrix();
      Gl.LoadIdentity();

      Gl.BindFramebufferEXT(FramebufferTarget.Framebuffer, _frontBuffer);

      Gl.Enable(EnableCap.Texture2d);
      Gl.BindTexture(TextureTarget.Texture2d, _backTexture);

      // Draw a textured quad
      Gl.Begin(PrimitiveType.Quads);
      Gl.TexCoord2(0, 0); Gl.Vertex3(0, 0, 0);
      Gl.TexCoord2(0, 1); Gl.Vertex3(0, height, 0);
      Gl.TexCoord2(1, 1); Gl.Vertex3(width, height, 0);
      Gl.TexCoord2(1, 0); Gl.Vertex3(width, 0, 0);
      Gl.End();

      Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
      Gl.Disable(EnableCap.Texture2d);

      Gl.BindFramebufferEXT(FramebufferTarget.Framebuffer, 0);

      Gl.MatrixMode(MatrixMode.Projection);
      Gl.PopMatrix();

      Gl.MatrixMode(MatrixMode.Modelview);
      Gl.PopMatrix();

      Gl.PopAttrib();
    }

    /// <summary>
    /// Creates the front and back framebuffer objects.
    /// </summary>
    /// <param name="width">Width of the framebuffers.</param>
    /// <param name="height">Height of the framebuffers.</param>
    protected void CreateFrameBuffers(int width, int height)
    {
      CreateBackBuffer(width, height);
      CreateFrontBuffer(width, height);
    }

    /// <summary>
    /// Creates the framebuffer object that will be rendered to by the libretro core.
    /// </summary>
    /// <param name="width">Width of the framebuffer.</param>
    /// <param name="height">Height of the framebuffer.</param>
    protected virtual void CreateBackBuffer(int width, int height)
    {
      // Create and bind a new FBO
      _backBuffer = Gl.GenFramebuffer();
      Gl.BindFramebufferEXT(FramebufferTarget.Framebuffer, _backBuffer);

      // Add colour and depth buffers
      _backTexture = CreateTextureBuffer(width, height);
      _backDepthBuffer = CreateDepthBuffer(width, height);

      //FramebufferStatus framebufferStatus = Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
      //if (framebufferStatus != FramebufferStatus.FramebufferComplete)
      //  throw new InvalidOperationException("framebuffer not complete");
      
      Gl.BindFramebufferEXT(FramebufferTarget.Framebuffer, 0);
    }

    /// <summary>
    /// Creates the framebuffer object that will be rendered to by a call to <see cref="Render(int, int)"/>.
    /// </summary>
    /// <param name="width">Width of the framebuffer.</param>
    /// <param name="height">Height of the framebuffer.</param>
    protected virtual void CreateFrontBuffer(int width, int height)
    {
      _frontBuffer = Gl.GenFramebuffer();
      Gl.BindFramebufferEXT(FramebufferTarget.Framebuffer, _frontBuffer);
      _frontTexture = CreateTextureBuffer(width, height);
      Gl.BindFramebufferEXT(FramebufferTarget.Framebuffer, 0);
    }

    /// <summary>
    /// Creates a texture with the specified dimensions and attaches it to the currently bound FBO
    /// </summary>
    /// <param name="width">Width of the texture.</param>
    /// <param name="height">Height of the texture.</param>
    /// <returns>Id of the created texture.</returns>
    protected uint CreateTextureBuffer(int width, int height)
    {
      // Create and bind a new texture
      uint texture = Gl.GenTexture();
      Gl.BindTexture(TextureTarget.Texture2d, texture);
      
      // Set storage, format and size
      Gl.TextureImage2DEXT(texture, TextureTarget.Texture2d, 0, InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);
      Gl.TextureParameter(texture, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Nearest);
      Gl.TextureParameter(texture, TextureParameterName.TextureMinFilter, (float)TextureMagFilter.Nearest);

      //Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.Rgb8, width, height);

      // Attach the texture to the currently bound FBO
      Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, texture, 0);
      Gl.BindTexture(TextureTarget.Texture2d, 0);
      return texture;
    }

    /// <summary>
    /// Creates a depth buffer with the specified dimensions and attaches it to the currently bound FBO
    /// </summary>
    /// <param name="width">Width of the buffer.</param>
    /// <param name="height">Height of the buffer.</param>
    /// <returns>Id of the created depth buffer.</returns>
    protected uint CreateDepthBuffer(int width, int height)
    {
      // Create and bind a new render buffer
      uint depthBuffer = Gl.GenRenderbuffer();
      Gl.BindRenderbufferEXT(RenderbufferTarget.Renderbuffer, depthBuffer);

      // Set storage, format and size
      Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent24, width, height);

      // Attach the buffer to the currently bound FBO
      Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthBuffer);
      Gl.BindRenderbufferEXT(RenderbufferTarget.Renderbuffer, 0);
      return depthBuffer;
    }

    /// <summary>
    /// Destroys all framebuffers and attached buffers.
    /// </summary>
    protected void DestroyFrameBuffers()
    {
      DestroyFrontBuffer();
      DestroyBackBuffer();
    }

    /// <summary>
    /// Destroys the back framebuffer and any attached buffers.
    /// </summary>
    protected virtual void DestroyBackBuffer()
    {
      DestroyTexture(ref _backTexture);
      DestroyRenderBuffer(ref _backDepthBuffer);
      DestroyFrameBuffer(ref _backBuffer);
    }

    /// <summary>
    /// Destroys the front framebuffer and the attached buffers.
    /// </summary>
    protected virtual void DestroyFrontBuffer()
    {
      DestroyTexture(ref _frontTexture);
      DestroyFrameBuffer(ref _frontBuffer);
    }

    protected void DestroyFrameBuffer(ref uint frameBuffer)
    {
      if (frameBuffer == 0)
        return;
      Gl.DeleteFramebuffers(frameBuffer);
      frameBuffer = 0;
    }

    protected void DestroyTexture(ref uint texture)
    {
      if (texture == 0)
        return;
      Gl.DeleteTextures(texture);
      texture = 0;
    }

    protected void DestroyRenderBuffer(ref uint renderBuffer)
    {
      if (renderBuffer == 0)
        return;
      Gl.DeleteRenderbuffers(renderBuffer);
      renderBuffer = 0;
    }

    public virtual void Destroy()
    {
      DestroyFrameBuffers();

      if (_deviceContext == null)
        return;
      _deviceContext.DeleteContext(_glContext);
      _deviceContext.Dispose();
      _deviceContext = null;
    }

    public void Dispose()
    {
      Destroy();
    }
  }
}
