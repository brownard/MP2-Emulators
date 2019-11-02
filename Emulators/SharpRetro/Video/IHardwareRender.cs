using SharpRetro.LibRetro;

namespace SharpRetro.Video
{
  public interface IHardwareRender
  {
    bool SetHWRender(ref retro_hw_render_callback hwRenderCallback);
    void Create();
    void Destroy();
  }
}
