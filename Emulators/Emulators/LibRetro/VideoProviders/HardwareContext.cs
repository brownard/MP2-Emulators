using Emulators.LibRetro.VideoProviders.OpenGL;
using SharpDX.Direct3D9;
using SharpRetro.LibRetro;
using System;
using System.Runtime.InteropServices;

namespace Emulators.LibRetro.VideoProviders
{
  public class HardwareContext : IDisposable
  {
    protected retro_hw_get_proc_address_t _getProcAddressDlgt;
    protected retro_hw_get_current_framebuffer_t _getCurrentFramebufferDlgt;
    protected retro_hw_context_reset_t _contextReset;
    protected retro_hw_context_reset_t _contextDestroy;

    protected Device _device;
    protected DxOpenGLContext _contextProvider;
    protected bool _depth;
    protected bool _stencil;
    protected bool _bottomLeftOrigin;

    protected int _maxWidth;
    protected int _maxHeight;

    public HardwareContext(Device device)
    {
      _device = device;
    }

    public Texture Texture
    {
      get { return _contextProvider?.Texture; }
    }

    public void Render(int width, int height)
    {
      _contextProvider?.Render(width, height, _bottomLeftOrigin);
    }

    public void ReadPixels(int width, int height, IntPtr data)
    {
      _contextProvider?.ReadPixels(width, height, data);
    }

    /// <summary>
    /// Configures an OpenGl context using the specified <paramref name="hwRenderCallback"/> properties and sets it's
    /// <see cref="retro_hw_render_callback.get_proc_address"/> and <see cref="retro_hw_render_callback.get_current_framebuffer"/> callbacks.
    /// </summary>
    /// <param name="hwRenderCallback"></param>
    /// <returns></returns>
    public bool SetRenderCallback(ref retro_hw_render_callback hwRenderCallback)
    {
      _contextProvider = new DxOpenGLContext(_device);

      _depth = hwRenderCallback.depth;
      _stencil = hwRenderCallback.stencil;
      _bottomLeftOrigin = hwRenderCallback.bottom_left_origin;

      // Get the callbacks used to notify the libretro core when our render context changes 
      if (hwRenderCallback.context_reset != IntPtr.Zero)
        _contextReset = Marshal.GetDelegateForFunctionPointer<retro_hw_context_reset_t>(hwRenderCallback.context_reset);
      if(hwRenderCallback.context_destroy != IntPtr.Zero)
        _contextDestroy = Marshal.GetDelegateForFunctionPointer<retro_hw_context_reset_t>(hwRenderCallback.context_destroy);

      // Set the callbacks that the libretro core uses to call our render context
      _getProcAddressDlgt = new retro_hw_get_proc_address_t(_contextProvider.GetProcAddress);
      hwRenderCallback.get_proc_address = Marshal.GetFunctionPointerForDelegate(_getProcAddressDlgt);
      
      _getCurrentFramebufferDlgt = new retro_hw_get_current_framebuffer_t(_contextProvider.GetCurrentFramebuffer);
      hwRenderCallback.get_current_framebuffer = Marshal.GetFunctionPointerForDelegate(_getCurrentFramebufferDlgt);

      return true;
    }

    public void Create()
    {
      if (_contextProvider != null && _maxWidth > 0 && _maxHeight > 0)
        CreateContext();
    }

    public void Destroy()
    {
      if (_contextProvider != null)
      {
        _contextDestroy?.Invoke();
        _contextProvider.Destroy();
        _contextProvider = null;
      }
    }

    public void SetGeometry(retro_game_geometry geometry)
    {
      _maxWidth = (int)geometry.max_width;
      _maxHeight = (int)geometry.max_height;
      if (_contextProvider != null)
      {
        _contextProvider.SetDimensions(_maxWidth, _maxHeight);
        _contextReset?.Invoke();
      }
    }

    protected void CreateContext()
    {
      _contextProvider.Create(_maxWidth, _maxHeight);
      _contextReset?.Invoke();
    }

    public void Dispose()
    {
      Destroy();
    }
  }
}