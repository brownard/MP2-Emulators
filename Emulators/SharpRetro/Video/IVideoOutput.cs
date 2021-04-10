using SharpRetro.LibRetro;
using System;

namespace SharpRetro.Video
{
  public interface IVideoOutput
  {
    void VideoRefresh(IntPtr data, uint width, uint height, uint pitch);
    void SetGeometry(retro_game_geometry geometry);
    void SetPixelFormat(RETRO_PIXEL_FORMAT pixelFormat);
  }
}