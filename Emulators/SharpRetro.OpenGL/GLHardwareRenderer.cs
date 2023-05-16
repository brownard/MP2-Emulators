using OpenGL;
using SharpRetro.LibRetro;
using SharpRetro.OpenGL.Contexts;
using SharpRetro.OpenGL.Render;
using SharpRetro.Video;
using System;
using System.Runtime.InteropServices;

namespace SharpRetro.OpenGL
{
  public class GLHardwareRenderer : IHardwareRenderer
  {
    /// <summary>
    /// Entry point for the Windows OpenGl extensions, passed back to the core via the <see cref="_getProcAddressDelegate"/>.
    /// </summary>
    /// <param name="function_name"></param>
    /// <returns></returns>
    [DllImport("opengl32", EntryPoint = "wglGetProcAddress", ExactSpelling = true)]
    public static extern IntPtr wglGetProcAddress(IntPtr function_name);

    /// <summary>
    /// Passed to the libretro core in a call to SetRenderCallback,
    /// will be used by the core to load the OpenGL symbols.
    /// </summary>
    private static readonly retro_hw_get_proc_address_t _getProcAddressDelegate = new retro_hw_get_proc_address_t(wglGetProcAddress);

    /// <summary>
    /// Passed to the libretro core in a call to SetRenderCallback,
    /// will be used by the core to get the id of the framebuffer to render to.
    /// </summary>
    private retro_hw_get_current_framebuffer_t _getCurrentFramebufferDlgt;

    /// <summary>
    /// Set by the libretro core in a call to SetRenderCallback,
    /// will be used to notify the core when the context has changed.
    /// </summary>
    private retro_hw_context_reset_t _contextResetDlgt;

    /// <summary>
    /// Optionally set by the libretro core in a call to SetRenderCallback,
    /// will be used to notify the core when the context has been destroyed.
    /// </summary>
    private retro_hw_context_reset_t _contextDestroyDlgt;

    protected bool _haveSharedContext = false;

    protected DeviceContext _deviceContext;
    protected GlContext _glContext;
    protected GlContext _sharedGlContext;

    protected LibretroRenderContext _libretroContext;
    protected IRenderStrategy _renderStrategy;
    protected IRenderContext _frontendContext;

    protected int _currentWidth;
    protected int _currentHeight;
    protected bool _created;

    /// <summary>
    /// Creates a new <see cref="GLHardwareRenderer"/> that will use the specified <see cref="IRenderStrategy"/>
    /// to render the contents of the libretro core's framebuffer to the specified <see cref="IRenderContext"/>.
    /// </summary>
    /// <param name="renderStrategy">The render strategy to use to render to the <paramref name="frontendContext"/>.</param>
    /// <param name="frontendContext">The render context to render to.</param>
    public GLHardwareRenderer(IRenderStrategy renderStrategy, IRenderContext frontendContext)
    {
      // We can't reference the GetCurrentFramebuffer method from a field initializer, so need to set it here
      _getCurrentFramebufferDlgt = new retro_hw_get_current_framebuffer_t(GetCurrentFramebuffer);

      _renderStrategy = renderStrategy;
      _frontendContext = frontendContext;
    }
    
    public void Create()
    {
      if (_created || _libretroContext == null)
        return;

      _created = true;
      _glContext.MakeCurrent();
      _renderStrategy.Create();

      // The dimensions might not have been set yet by a call to SetDimensions,
      // we'll defer creating the buffers in this case until the dimensions have been set.
      if (_currentWidth < 1 && _currentHeight < 1)
        return;

      CreateBuffers(_currentWidth, _currentHeight);
      if (_sharedGlContext != null)
        _sharedGlContext.MakeCurrent();
      OnContextReset();
    }

    public void SetDimensions(int width, int height)
    {
      // Might be called multiple times, check whether anything has actually changed
      if (width == _currentWidth && height == _currentHeight)
        return;

      _currentWidth = width;
      _currentHeight = height;

      // We can't create the buffers until Create has been
      // called and the dimensions are valid
      if (!_created || width < 1 || height < 1)
        return;

      _glContext.MakeCurrent();
      DestroyBuffers();
      CreateBuffers(width, height);
      if(_sharedGlContext != null)
        _sharedGlContext.MakeCurrent();
      OnContextReset();
    }

    public void Render(int width, int height)
    {
      if (!_created || _currentWidth < width || _currentHeight < height)
        return;

      _glContext.MakeCurrent();
      _frontendContext.BeginRender(width, height);
      _renderStrategy.Render(width, height, _libretroContext.BottomLeftOrigin, _libretroContext.TextureBuffer.Id);
      _frontendContext.EndRender();
      if(_sharedGlContext != null)
        _sharedGlContext.MakeCurrent();
    }

    public void Destroy()
    {
      if (_libretroContext == null)
        return;
      _created = false;
      //OnContextDestroy();
      DestroyBuffers();
      _renderStrategy.Destroy();
      _sharedGlContext?.Dispose();
      _glContext.Dispose();
      _deviceContext.Dispose();
    }

    public bool SetHWRender(ref retro_hw_render_callback hwRenderCallback)
    {
      if (_deviceContext != null)
        return false;

      _deviceContext = DeviceContext.Create();

      // Try and create an openGl context using the attributes specified in the callback
      _glContext = GlContext.TryCreate(_deviceContext, (int)hwRenderCallback.version_major, (int)hwRenderCallback.version_minor,
        (retro_hw_context_type)hwRenderCallback.context_type, hwRenderCallback.debug_context);

      // Creation might fail if the system doesn't
      // support the requested attributes
      if (_glContext == null)
        return false;

      // ToDo: Some cores seem to need a separate shared context
      // but can't get it to work yet...
      if (_haveSharedContext)
        _sharedGlContext = _glContext.CreateSharedContext();
      
      // Render context that the libretro core will render to, contains a texture buffer and an optional depth/stencil buffer.
      _libretroContext = new LibretroRenderContext(hwRenderCallback.depth, hwRenderCallback.stencil, hwRenderCallback.bottom_left_origin);
      
      // Get the libretro core callbacks
      _contextResetDlgt = hwRenderCallback.context_reset != IntPtr.Zero ?
        Marshal.GetDelegateForFunctionPointer<retro_hw_context_reset_t>(hwRenderCallback.context_reset) : null;

      _contextDestroyDlgt = hwRenderCallback.context_destroy != IntPtr.Zero ?
        Marshal.GetDelegateForFunctionPointer<retro_hw_context_reset_t>(hwRenderCallback.context_destroy) : null;

      // Pass the opengl callbacks back to the core
      hwRenderCallback.get_proc_address = Marshal.GetFunctionPointerForDelegate(_getProcAddressDelegate);
      hwRenderCallback.get_current_framebuffer = Marshal.GetFunctionPointerForDelegate(_getCurrentFramebufferDlgt);

      return true;
    }

    protected void CreateBuffers(int width, int height)
    {
      _libretroContext.CreateBuffers(width, height);
      _frontendContext.CreateBuffers(width, height);
    }

    protected void DestroyBuffers()
    {
      _libretroContext.DestroyBuffers();
      _frontendContext.DestroyBuffers();
    }

    protected uint GetCurrentFramebuffer()
    {
      return _libretroContext?.Framebuffer?.Id ?? 0;
    }

    protected void OnContextReset()
    {
      _contextResetDlgt?.Invoke();
    }

    protected void OnContextDestroy()
    {
      _contextDestroyDlgt?.Invoke();
    }
  }
}
