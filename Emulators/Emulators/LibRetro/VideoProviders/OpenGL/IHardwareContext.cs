using System;

namespace Emulators.LibRetro.VideoProviders.OpenGL
{
  public interface IHardwareContext : IDisposable
  {
    void Create(int width, int height);
    void Destroy();
    uint GetCurrentFramebuffer();
    IntPtr GetProcAddress(IntPtr sym);
    void SetDimensions(int width, int height);
  }
}