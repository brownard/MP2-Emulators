using OpenGL;
using SharpRetro.LibRetro;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Emulators.LibRetro.VideoProviders.OpenGL
{
  /// <summary>
  /// Provides an OpenGl hardware context and framebuffer object for supported cores and provides
  /// methods for rendering the core's framebuffer to a specified <see cref="FrontendRenderContext"/>. 
  /// </summary>
  /// <typeparam name="T">The type of the frontend </typeparam>
  public class RetroGlRenderer<T> : IDisposable where T : FrontendRenderContext
  {
    /// <summary>
    /// Set by the libretro core in a call to SetRenderCallback,
    /// will be used to notify the core when the context has changed.
    /// </summary>
    protected retro_hw_context_reset_t _contextResetDlgt;

    /// <summary>
    /// Optionally set by the libretro core in a call to SetRenderCallback,
    /// will be used to notify the core when the context has been destroyed.
    /// </summary>
    protected retro_hw_context_reset_t _contextDestroyDlgt;

    protected DeviceContext _deviceContext;
    protected GlContext _glContext;
    protected GlContext _sharedGlContext;
    protected RetroGlRenderContext _retroGlRenderContext;
    protected T _frontendRenderContext;
    protected IRenderStrategy _renderStrategy;
    protected bool _haveSharedContext = false;

    protected bool _created = false;
    protected int _width;
    protected int _height;

    public T FrontendRenderContext
    {
      get { return _frontendRenderContext; }
    }

    public bool Create()
    {
      if (_created)
        return true;
      _created = true;
      if (_retroGlRenderContext == null || _width <= 0 || _height <= 0)
        return false;
      CreateBuffers();
      OnContextReset();
      return true;
    }

    /// <summary>
    /// Configures an OpenGl context using the specified <paramref name="hwRenderCallback"/> properties and sets it's
    /// <see cref="retro_hw_render_callback.get_proc_address"/> and <see cref="retro_hw_render_callback.get_current_framebuffer"/> callbacks.
    /// </summary>
    /// <param name="hwRenderCallback"></param>
    /// <returns></returns>
    public bool SetRenderCallback(ref retro_hw_render_callback hwRenderCallback, Func<T> frontendContextFactory)
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

      _glContext.MakeCurrent();

      _renderStrategy = new DefaultRenderStrategy();
      _renderStrategy.Init();

      _frontendRenderContext = frontendContextFactory.Invoke();
      _frontendRenderContext.Init();
            
      _retroGlRenderContext = new RetroGlRenderContext(hwRenderCallback.depth, hwRenderCallback.stencil, hwRenderCallback.bottom_left_origin);

      _contextResetDlgt = hwRenderCallback.context_reset != IntPtr.Zero ? Marshal.GetDelegateForFunctionPointer<retro_hw_context_reset_t>(hwRenderCallback.context_reset) : null;
      _contextDestroyDlgt = hwRenderCallback.context_destroy != IntPtr.Zero ? Marshal.GetDelegateForFunctionPointer<retro_hw_context_reset_t>(hwRenderCallback.context_destroy) : null;

      hwRenderCallback.get_proc_address = Marshal.GetFunctionPointerForDelegate(_retroGlRenderContext.GetProcAddressDelegate);
      hwRenderCallback.get_current_framebuffer = Marshal.GetFunctionPointerForDelegate(_retroGlRenderContext.GetCurrentFramebufferDlgt);
      bool result = MakeCurrent(true);
      return true;
    }

    public void SetDimensions(int width, int height)
    {
      if (_width == width && _height == height)
        return;
      _width = width;
      _height = height;
      if (!_created)
        return;
      RecreateBuffers();
      OnContextReset();
    }

    public void Render(int width, int height)
    {
      if (!_created)
        return;

      MakeCurrent(false);
      _frontendRenderContext.BeginRender(width, height);
      _renderStrategy.Render(width, height, _retroGlRenderContext.BottomLeftOrigin, _retroGlRenderContext.TextureBuffer.Id);
      _frontendRenderContext.EndRender();
      MakeCurrent(true);
    }

    protected void OnContextReset()
    {
      _contextResetDlgt?.Invoke();
    }

    protected void OnContextDestroy()
    {
      _contextDestroyDlgt?.Invoke();
    }

    protected bool MakeCurrent(bool sharedContext)
    {
      if (_sharedGlContext == null)
        return true;

      GlContext context = sharedContext ? _sharedGlContext : _glContext;
      bool result = context.MakeCurrent();
      IntPtr current = DeviceContext.GetCurrentContext();
      Debug.Assert(current == context.Context);
      return result;
    }

    protected void RecreateBuffers()
    {
      DestroyBuffers();
      CreateBuffers();
    }

    protected void CreateBuffers()
    {
      if (_retroGlRenderContext == null)
        throw new InvalidOperationException("Render context has not been created yet");

      if (_width < 1 || _height < 1)
        return;

      _retroGlRenderContext.CreateBuffers(_width, _height);
      MakeCurrent(false);
      _frontendRenderContext.CreateBuffers(_width, _height);
      MakeCurrent(true);
    }

    protected void DestroyBuffers()
    {
      if (_retroGlRenderContext != null)
        _retroGlRenderContext.DestroyBuffers();
      MakeCurrent(false);
      if (_frontendRenderContext != null)
        _frontendRenderContext.DestroyBuffers();
      MakeCurrent(true);
    }

    public void Destroy()
    {
      OnContextDestroy();
      _contextResetDlgt = null;
      _contextDestroyDlgt = null;
      OpenGlUtils.TryDispose(ref _retroGlRenderContext);
      OpenGlUtils.TryDispose(ref _frontendRenderContext);
      OpenGlUtils.TryDispose(ref _renderStrategy);
      OpenGlUtils.TryDispose(ref _glContext);
      OpenGlUtils.TryDispose(ref _deviceContext);
      _created = false;
    }

    public void Dispose()
    {
      Destroy();
    }
  }
}
