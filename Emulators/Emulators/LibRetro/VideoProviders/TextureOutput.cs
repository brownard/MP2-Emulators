using SharpDX;
using SharpDX.Direct3D9;
using SharpRetro.LibRetro;
using SharpRetro.Video;
using System;
using System.Drawing;

namespace Emulators.LibRetro.VideoProviders
{
  public class TextureOutput : IVideoOutput, IHardwareRender, IDisposable
  {
    protected readonly object _surfaceLock = new object();

    protected Device _device;
    protected HardwareContext _renderContext;
    protected RETRO_PIXEL_FORMAT _pixelFormat = RETRO_PIXEL_FORMAT.XRGB1555;
    
    protected SafeTexture _renderTexture;
    protected Size _textureSize;
    protected float _displayAspectRatio = -1;

    protected bool _isAllocated = true;

    public object SurfaceLock => _surfaceLock;
    public Size TextureSize => _textureSize;

    public TextureOutput(Device device)
    {
      _device = device;
    }

    public Texture Texture
    {
      get
      {
        lock (_surfaceLock)
          return _renderContext?.Texture ?? _renderTexture;
      }
    }

    public SizeF DisplayAspectRatio
    {
      get
      {
        if(_displayAspectRatio > 0)
          return new SizeF(_displayAspectRatio, 1);
        Size size = _textureSize;
        if (size.Width > 0 && size.Height > 0)
          return new SizeF(size.Width, size.Height);
        return new SizeF(1, 1);
      }
    }

    public void Create()
    {
      _renderContext?.Create();
    }

    public void Destroy()
    {
      _renderContext?.Destroy();
    }

    public void SetPixelFormat(RETRO_PIXEL_FORMAT pixelFormat)
    {
      _pixelFormat = pixelFormat;
    }

    public void SetGeometry(retro_game_geometry geometry)
    {
      _textureSize = new Size((int)geometry.base_width, (int)geometry.base_height);
      _displayAspectRatio = geometry.aspect_ratio;
      _renderContext?.SetGeometry(geometry);
    }

    public bool SetHWRender(ref retro_hw_render_callback hwRenderCallback)
    {
      if (_renderContext != null)
        _renderContext.Dispose();
      _renderContext = new HardwareContext(_device);
      return _renderContext.SetRenderCallback(ref hwRenderCallback);
    }

    public void VideoRefresh(IntPtr data, uint width, uint height, uint pitch)
    {
      // Dupe frame.
      if (data == IntPtr.Zero)
        return;

      lock (_surfaceLock)
      {
        if (!_isAllocated)
          return;

        // update the current size
        if (_textureSize.Width != width || _textureSize.Height != height)
          _textureSize = new Size((int)width, (int)height);

        if (data.ToInt32() == retro_hw_render_callback.RETRO_HW_FRAME_BUFFER_VALID)
          // OpenGl back buffer has been updated, copy it to our render texture
          UpdateTextureFromFramebuffer(_textureSize.Width, _textureSize.Height);
        else
          // data contains a pointer to the pixel data, blit it into our render texture
          UpdateTextureFromData(data, _textureSize.Width, _textureSize.Height, (int)pitch);
      }
    }

    protected void UpdateTextureFromFramebuffer(int width, int height)
    {
      if (_renderContext == null)
        return;

      // Update the render context's front buffer, inverting the image if necessary.
      _renderContext.Render(width, height);

      // If the OpenGl context supports the DirectX extensions
      // then we'll just use the texture directly.
      Texture glTexture = _renderContext.Texture;
      if (glTexture != null)
        return;

      // DirectX extensions not supported, manually copy
      // the OpenGl pixel data to a DirectX texture.

      // Check the size and usage of the render texture
      CheckRenderTexture(width, height, Usage.Dynamic);
      lock (_renderTexture.SyncRoot)
      {
        // The client can dispose our texture when resizing the window
        // check inside the lock that this isn't the case.
        if (_renderTexture.IsDisposing)
          return;

        // Read back the pixel data from the OpenGl context
        // directly into the DirectX texture's data pointer.
        DataRectangle rectangle = _renderTexture.LockRectangle(0, LockFlags.Discard);
        try
        {
          _renderContext.ReadPixels(width, height, rectangle.DataPointer);
        }
        finally
        {
          _renderTexture.UnlockRectangle(0);
        }
      }
    }

    protected void UpdateTextureFromData(IntPtr data, int width, int height, int pitch)
    {
      // Check the current size of our render texture.
      CheckRenderTexture(width, height, Usage.Dynamic);

      lock (_renderTexture.SyncRoot)
      {
        // The client can dispose our texture when resizing the window
        // check inside the lock that this isn't the case.
        if (_renderTexture.IsDisposing)
          return;

        DataRectangle rectangle = _renderTexture.LockRectangle(0, LockFlags.Discard);
        try
        {
          // Convert the pixel data to a 32 bit format and blit it into our render texture.
          VideoBlitter.Blit(_pixelFormat, data, rectangle.DataPointer, width, height, pitch, rectangle.Pitch);
        }
        finally
        {
          _renderTexture.UnlockRectangle(0);
        }
      }
    }

    protected void CheckRenderTexture(int width, int height, Usage usage)
    {
      if (_renderTexture != null)
      {
        lock (_renderTexture.SyncRoot)
        {
          if (!_renderTexture.IsDisposing)
          {
            SurfaceDescription desc = _renderTexture.GetLevelDescription(0);
            if (desc.Width == width && desc.Height == height && desc.Usage == usage)
              return;
            _renderTexture.Dispose();
          }
        }
      }
      _renderTexture = new SafeTexture(_device, width, height, 1, usage, Format.X8R8G8B8, Pool.Default);
    }

    public void Reallocate()
    {
      lock (_surfaceLock)
        _isAllocated = true;
    }

    public void Release()
    {
      lock (_surfaceLock)
      {
        if (_renderTexture != null)
        {
          _renderTexture.Dispose();
          _renderTexture = null;
        }
        _isAllocated = false;
      }
    }

    public void Dispose()
    {
      Release();
      if (_renderContext != null)
      {
        _renderContext.Dispose();
        _renderContext = null;
      }
    }
  }
}
