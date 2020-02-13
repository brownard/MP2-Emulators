using OpenGL;
using SharpDX.Direct3D9;
using System;

namespace Emulators.LibRetro.VideoProviders.OpenGL
{
  public class DxOpenGLContext : OpenGLContextNew
  {
    protected Device _dxDevice;
    protected IntPtr _glDeviceHandle;
    
    protected SafeTexture _dxTexture;

    protected IntPtr _glTextureHandle;

    public DxOpenGLContext(Device device)
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

    public override void Render(int width, int height, bool bottomLeftOrigin)
    {
      if (!HasDxContext)
      {
        base.Render(width, height, bottomLeftOrigin);
        return;
      }
            
      MakeCurrent(false);
      SafeTexture texture = _dxTexture;
      lock (texture.SyncRoot)
      {
        if (texture.IsDisposing)
        {
          DestroySharedTexture();
          Gl.BindFramebufferEXT(FramebufferTarget.Framebuffer, _frontBuffer);
          CreateSharedTexture(width, height);
          Gl.BindFramebufferEXT(FramebufferTarget.Framebuffer, 0);
        }
      }

      DxGL.DXLockObjectsNV(_glDeviceHandle, new[] { _glTextureHandle });
      RenderToTexture(width, height, bottomLeftOrigin);
      //RenderBackBufferEx(width, height, bottomLeftOrigin);
      DxGL.DXUnlockObjectsNV(_glDeviceHandle, new[] { _glTextureHandle });

      MakeCurrent(true);
    }

    public override void ReadPixels(int width, int height, IntPtr data)
    {
      MakeCurrent(false);
      if (HasDxContext)
        DxGL.DXLockObjectsNV(_glDeviceHandle, new[] { _glTextureHandle });

      base.ReadPixels(width, height, data);

      MakeCurrent(false);
      if (HasDxContext)
        DxGL.DXUnlockObjectsNV(_glDeviceHandle, new[] { _glTextureHandle });
      MakeCurrent(true);
    }

    protected override void CreateFrontBuffer(int width, int height)
    {
      if (!DxGL.HasDXExtensions())
      {
        _glDeviceHandle = IntPtr.Zero;
        base.CreateFrontBuffer(width, height);
        return;
      }

      _glDeviceHandle = DxGL.DXOpenDeviceNV(_dxDevice.NativePointer);

      _frontBuffer = Gl.GenFramebuffer();
      Gl.BindFramebufferEXT(FramebufferTarget.Framebuffer, _frontBuffer);
      CreateSharedTexture(width, height);
      FramebufferStatus framebufferStatus = Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
      Gl.BindFramebufferEXT(FramebufferTarget.Framebuffer, 0);
    }

    protected override void DestroyFrontBuffer()
    {
      if (!HasDxContext)
      {
        base.DestroyFrontBuffer();
        return;
      }

      DestroySharedTexture();
      DestroyFrameBuffer(ref _frontBuffer);

      DxGL.DXCloseDeviceNV(_glDeviceHandle);
      _glDeviceHandle = IntPtr.Zero;
    }

    protected void CreateSharedTexture(int width, int height)
    {
      IntPtr sharedResourceHandle = IntPtr.Zero;
      _dxTexture = new SafeTexture(_dxDevice, width, height, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default, ref sharedResourceHandle);
      bool result = DxGL.DXSetResourceShareHandleNV(_dxTexture.NativePointer, sharedResourceHandle);

      _frontTexture = Gl.GenTexture();
      _glTextureHandle = DxGL.DXRegisterObjectNV(_glDeviceHandle, _dxTexture.NativePointer, _frontTexture, (uint)TextureTarget.Texture2d, DxGL.WGL_ACCESS_READ_WRITE_NV);
      DxGL.DXLockObjectsNV(_glDeviceHandle, new[] { _glTextureHandle });
      Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, _frontTexture, 0);
      DxGL.DXUnlockObjectsNV(_glDeviceHandle, new[] { _glTextureHandle });
    }

    protected void DestroySharedTexture()
    {
      if (_glDeviceHandle == IntPtr.Zero)
        return;

      if (_glTextureHandle != IntPtr.Zero)
      {
        DxGL.DXUnlockObjectsNV(_glDeviceHandle, new[] { _glTextureHandle });
        DxGL.DXUnregisterObjectNV(_glDeviceHandle, _glTextureHandle);
        _glTextureHandle = IntPtr.Zero;
      }

      if (_frontTexture > 0)
      {
        Gl.DeleteTextures(_frontTexture);
        _frontTexture = 0;
      }

      if (_dxTexture != null)
      {
        _dxTexture.Dispose();
        _dxTexture = null;
      }
    }
  }
}
