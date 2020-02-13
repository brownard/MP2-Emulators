using Emulators.LibRetro.VideoProviders.OpenGL.Objects;
using OpenGL;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulators.LibRetro.VideoProviders.OpenGL
{
  public class DxFrontendRenderContext : FrontendRenderContext
  {
    protected Device _dxDevice;
    protected IntPtr _glDeviceHandle;
    protected IntPtr _glTextureHandle;
    protected SafeTexture _dxTexture;

    public DxFrontendRenderContext(Device device)
    {
      _dxDevice = device;
    }

    public SafeTexture Texture
    {
      get { return _dxTexture; }
    }

    public bool HasDxContext
    {
      get { return _glDeviceHandle != IntPtr.Zero; }
    }

    public override void Init()
    {
      if (_dxDevice == null || !Wgl.CurrentExtensions.DXInterop_NV)
      {
        _glDeviceHandle = IntPtr.Zero;
        base.Init();
        return;
      }

      _glDeviceHandle = Wgl.DXOpenDeviceNV(_dxDevice.NativePointer);
    }

    public override void ReadPixels(int width, int height, IntPtr data)
    {
      LockTexture();
      base.ReadPixels(width, height, data);
      UnlockTexture();
    }

    public override void BeginRender(int width, int height)
    {
      base.BeginRender(width, height);
      if (!HasDxContext)
        return;

      SafeTexture texture = _dxTexture;
      lock (texture.SyncRoot)
      {
        if (texture.IsDisposing)
        {
          DestroyTextureBuffer();
          CreateTextureBuffer(width, height);
        }
      }
      LockTexture();
    }

    public override void EndRender()
    {
      UnlockTexture();
      base.EndRender();
    }

    protected override void CreateTextureBuffer(int width, int height)
    {
      if (!HasDxContext)
      {
        base.CreateTextureBuffer(width, height);
        return;
      }

      IntPtr sharedResourceHandle = IntPtr.Zero;
      _dxTexture = new SafeTexture(_dxDevice, width, height, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default, ref sharedResourceHandle);
      _textureBuffer = new Texture2D();
      bool result = RegisterDxTexture(_dxTexture, _textureBuffer, sharedResourceHandle);

      LockTexture();
      _framebuffer.AttachTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _textureBuffer, 0);
      UnlockTexture();
    }

    protected override void DestroyTextureBuffer()
    {
      if (!HasDxContext)
      {
        base.DestroyTextureBuffer();
        return;
      }

      UnlockTexture();
      UnregisterDxTexture();
      OpenGlUtils.TryDispose(ref _textureBuffer);
      OpenGlUtils.TryDispose(ref _dxTexture);
    }

    protected bool RegisterDxTexture(Texture dxTexture, Texture2D glTexture, IntPtr dxSharedResourceHandle)
    {
      bool result = Wgl.DXSetResourceShareHandleNV(_dxTexture.NativePointer, dxSharedResourceHandle);
      if (result)
        _glTextureHandle = Wgl.DXRegisterObjectNV(_glDeviceHandle, _dxTexture.NativePointer, _textureBuffer.Id, (int)TextureTarget.Texture2d, Wgl.ACCESS_READ_WRITE_NV);
      return result;
    }

    protected void UnregisterDxTexture()
    {
      if (_glDeviceHandle != IntPtr.Zero && _glTextureHandle != IntPtr.Zero)
        Wgl.DXUnregisterObjectNV(_glDeviceHandle, _glTextureHandle);
    }

    protected void LockTexture()
    {
      if (_glDeviceHandle != IntPtr.Zero && _glTextureHandle != IntPtr.Zero)
        Wgl.DXLockObjectNV(_glDeviceHandle, 1, new[] { _glTextureHandle });
    }

    protected void UnlockTexture()
    {
      if (_glDeviceHandle != IntPtr.Zero && _glTextureHandle != IntPtr.Zero)
        Wgl.DXUnlockObjectsNV(_glDeviceHandle, 1, new[] { _glTextureHandle });
    }

    public override void Dispose()
    {
      base.Dispose();
      if (!HasDxContext)
        return;
      Wgl.DXCloseDeviceNV(_glDeviceHandle);
      _glDeviceHandle = IntPtr.Zero;
    }
  }
}
