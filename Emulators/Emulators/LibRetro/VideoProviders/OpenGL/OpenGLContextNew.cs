using Emulators.LibRetro.VideoProviders.OpenGL.Objects;
using Emulators.LibRetro.VideoProviders.OpenGL.Shaders;
using OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Emulators.LibRetro.VideoProviders.OpenGL
{
  /// <summary>
  /// Represents a libretro compatible OpenGl context that can be used with a <see cref="SharpRetro.Native.retro_hw_render_callback"/>.
  /// </summary>
  public class OpenGLContextNew : IDisposable, IHardwareContext
  {
    [DllImport("opengl32", EntryPoint = "wglGetProcAddress", ExactSpelling = true)]
    private static extern IntPtr wglGetProcAddress(IntPtr function_name);

    protected DeviceContext _deviceContext;
    protected IntPtr _glContext;
    protected IntPtr _sharedContext;

    protected VertexArrayObject _vertexArrayObject;
    protected ShaderProgram _program;

    protected uint _backBuffer;
    protected uint _backTexture;
    protected uint _backColourBuffer;
    protected uint _backDepthBuffer;

    protected uint _frontBuffer;
    protected uint _frontTexture;

    float[] quadVertices = {
        // Positions       // Texture Coords
        -1.0f, 1.0f, 0.0f, 0.0f, 1.0f,
        -1.0f, -1.0f, 0.0f, 0.0f, 0.0f,
        1.0f, 1.0f, 0.0f, 1.0f, 1.0f,
        1.0f, -1.0f, 0.0f, 1.0f, 0.0f,
    };

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
      _glContext = _deviceContext.CreateContext(IntPtr.Zero);//CreateGlContext();

      if (_glContext == IntPtr.Zero)
        throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

      //_sharedContext = _deviceContext.CreateContextAttrib(IntPtr.Zero, null, new Khronos.KhronosVersion(2, 1, "gl"));//_glContext);

      _deviceContext.MakeCurrent(_glContext);

      CreateFrontBuffer(width, height);
      CreateProgram();

      MakeCurrent(true);
      CreateBackBuffer(width, height);
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

      MakeCurrent(false);
      Gl.BindFramebufferEXT(FramebufferTarget.Framebuffer, _frontBuffer);
      Gl.ReadBuffer(ReadBufferMode.ColorAttachment0);
      Gl.ReadPixels(0, 0, width, height, PixelFormat.Bgra, PixelType.UnsignedByte, data);
      Gl.BindFramebufferEXT(FramebufferTarget.Framebuffer, 0);
      MakeCurrent(true);
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
      MakeCurrent(false);
      //RenderBackBuffer(width, height, bottomLeftOrigin);
      RenderToTexture(width, height, bottomLeftOrigin);
      MakeCurrent(true);
    }

    protected void RenderToTexture(int width, int height, bool bottomLeftOrigin)
    {
      Gl.Disable(EnableCap.DepthTest);
      Gl.Disable(EnableCap.Lighting);
      Gl.Disable(EnableCap.AlphaTest);

      Gl.Enable(EnableCap.Texture2d);

      _vertexArrayObject.Bind();
      _program.Enable();

      Gl.BindFramebuffer(FramebufferTarget.Framebuffer, _frontBuffer);
      Gl.DrawBuffers(Gl.COLOR_ATTACHMENT0);
      CheckError();

      Gl.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
      Gl.Clear(ClearBufferMask.ColorBufferBit);
      Gl.Viewport(0, 0, width, height);

      // Compute MVP
      float scaleY = bottomLeftOrigin ? -1 : 1;
      Matrix4x4f mvp = Matrix4x4f.Scaled(1, scaleY, 1);
      _program.SetMVPMatrix(mvp);

      Gl.ActiveTexture(TextureUnit.Texture0);
      Gl.BindTexture(TextureTarget.Texture2d, _backTexture);
      _program.SetFragTexture(0);
      CheckError();

      Gl.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
      CheckError();

      Gl.BindTexture(TextureTarget.Texture2d, 0);

      Gl.DrawBuffers(0);
      Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
      _program.Disable();
      _vertexArrayObject.Unbind();
      Gl.Disable(EnableCap.Texture2d);
    }

    protected void MakeCurrent(bool sharedContext)
    {
      if (_sharedContext != IntPtr.Zero && _deviceContext != null)
        _deviceContext.MakeCurrent(sharedContext ? _sharedContext : _glContext);
    }

    protected IntPtr CreateGlContext()
    {
      int major = 2;
      int minor = 1;

      List<int> attributes = new List<int>();

      attributes.AddRange(new int[] {
            Wgl.CONTEXT_MAJOR_VERSION_ARB, major,
            Wgl.CONTEXT_MINOR_VERSION_ARB, minor
          });

      uint contextFlags = 0;
      contextFlags |= Wgl.CONTEXT_DEBUG_BIT_ARB;
      attributes.AddRange(new int[] { Wgl.CONTEXT_FLAGS_ARB, unchecked((int)contextFlags) });

      uint contextProfile = 0;
      contextProfile |= Wgl.CONTEXT_COMPATIBILITY_PROFILE_BIT_ARB;
      attributes.AddRange(new int[] { Wgl.CONTEXT_PROFILE_MASK_ARB, unchecked((int)contextProfile) });

      attributes.Add(0);

      return _deviceContext.CreateContextAttrib(IntPtr.Zero, attributes.ToArray());
    }

    protected void CreateProgram()
    {
      _vertexArrayObject = new VertexArrayObject();
      _vertexArrayObject.Bind();

      _program = new ShaderProgram();
      _program.Link();

      _vertexArrayObject.SetInterleavedBuffer((uint)_program.PositionAttribLocation, 3, (uint)_program.TexCoordsAttribLocation, 2, quadVertices);

      _vertexArrayObject.Unbind();
    }

    /// <summary>
    /// Creates the front and back framebuffer objects.
    /// </summary>
    /// <param name="width">Width of the framebuffers.</param>
    /// <param name="height">Height of the framebuffers.</param>
    protected void CreateFrameBuffers(int width, int height)
    {
      CreateBackBuffer(width, height);
      MakeCurrent(false);
      CreateFrontBuffer(width, height);
      MakeCurrent(true);
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
      //_backColourBuffer = CreateColourBuffer(width, height);
      _backDepthBuffer = CreateDepthBuffer(width, height);
      FramebufferStatus framebufferStatus = Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
      
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
      //_backColourBuffer = CreateColourBuffer(width, height);
      FramebufferStatus framebufferStatus = Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
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
      Gl.TextureParameter(texture, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
      Gl.TextureParameter(texture, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
      Gl.TextureParameter(texture, TextureParameterName.TextureBaseLevel, 0);
      Gl.TextureParameter(texture, TextureParameterName.TextureMaxLevel, 0);
      var error = Gl.GetError();

      //Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.Rgb8, width, height);

      // Attach the texture to the currently bound FBO
      Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, texture, 0);
      Gl.DrawBuffers(Gl.COLOR_ATTACHMENT0);
      Gl.BindTexture(TextureTarget.Texture2d, 0);
      return texture;
    }

    /// <summary>
    /// Creates a colour buffer with the specified dimensions and attaches it to the currently bound FBO
    /// </summary>
    /// <param name="width">Width of the buffer.</param>
    /// <param name="height">Height of the buffer.</param>
    /// <returns>Id of the created depth buffer.</returns>
    protected uint CreateColourBuffer(int width, int height)
    {
      // Create and bind a new render buffer
      uint colourBuffer = Gl.GenRenderbuffer();
      Gl.BindRenderbufferEXT(RenderbufferTarget.Renderbuffer, colourBuffer);

      // Set storage, format and size
      Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.Rgb8, width, height);

      // Attach the buffer to the currently bound FBO
      Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, RenderbufferTarget.Renderbuffer, colourBuffer);
      Gl.BindRenderbufferEXT(RenderbufferTarget.Renderbuffer, 0);
      return colourBuffer;
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
      MakeCurrent(false);
      DestroyFrontBuffer();
      if (_program != null)
      {
        _program.Dispose();
        _program = null;
      }
      if (_vertexArrayObject != null)
      {
        _vertexArrayObject.Dispose();
        _vertexArrayObject = null;
      }

      MakeCurrent(true);
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

    protected void CheckError()
    {
      ErrorCode error = Gl.GetError();
      if (error != ErrorCode.NoError)
        return;
    }

    public virtual void Destroy()
    {
      DestroyFrameBuffers();
           
      if (_deviceContext == null)
        return;
      _deviceContext.MakeCurrent(IntPtr.Zero);
      if (_sharedContext != IntPtr.Zero)
        _deviceContext.DeleteContext(_sharedContext);
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
