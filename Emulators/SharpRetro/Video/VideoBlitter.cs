using SharpRetro.LibRetro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRetro.Video
{
  public static unsafe class VideoBlitter
  {
    public static void Blit(RETRO_PIXEL_FORMAT pixelFormat, IntPtr src, int[] dst, int width, int height, int srcPitch, int dstPitch)
    {
      fixed (int* i = &dst[0])
        Blit(pixelFormat, src, (IntPtr)i, width, height, srcPitch, dstPitch);
    }

    public static void Blit(RETRO_PIXEL_FORMAT pixelFormat, IntPtr src, IntPtr dst, int width, int height, int srcPitch, int dstPitch)
    {
      switch (pixelFormat)
      {
        case RETRO_PIXEL_FORMAT.XRGB8888:
          Blit888((int*)src, (int*)dst, width, height, srcPitch / 4, dstPitch / 4);
          break;
        case RETRO_PIXEL_FORMAT.RGB565:
          Blit565((short*)src, (int*)dst, width, height, srcPitch / 2, dstPitch / 4);
          break;
        case RETRO_PIXEL_FORMAT.XRGB1555:
          Blit555((short*)src, (int*)dst, width, height, srcPitch / 2, dstPitch / 4);
          break;
      }
    }

    public static void Blit555(short* src, int* dst, int width, int height, int srcPitch, int dstPitch)
    {
      for (int j = 0; j < height; j++)
      {
        short* srcRow = src;
        int* dstRow = dst;
        for (int i = 0; i < width; i++)
        {
          short ci = *srcRow;
          int r = ci & 0x001f;
          int g = ci & 0x03e0;
          int b = ci & 0x7c00;

          r = (r << 3) | (r >> 2);
          g = (g >> 2) | (g >> 7);
          b = (b >> 7) | (b >> 12);
          int co = (b << 16) | (g << 8) | r;

          *dstRow = co;
          dstRow++;
          srcRow++;
        }
        src += srcPitch;
        dst += dstPitch;
      }
    }

    public static void Blit565(short* src, int* dst, int width, int height, int srcPitch, int dstPitch)
    {
      for (int j = 0; j < height; j++)
      {
        short* srcRow = src;
        int* dstRow = dst;
        for (int i = 0; i < width; i++)
        {
          short ci = *srcRow;
          int r = ci & 0x001f;
          int g = (ci & 0x07e0) >> 5;
          int b = (ci & 0xf800) >> 11;

          r = (r << 3) | (r >> 2);
          g = (g << 2) | (g >> 4);
          b = (b << 3) | (b >> 2);
          int co = (b << 16) | (g << 8) | r;

          *dstRow = co;
          dstRow++;
          srcRow++;
        }
        src += srcPitch;
        dst += dstPitch;
      }
    }

    public static void Blit888(int* src, int* dst, int width, int height, int srcPitch, int dstPitch)
    {
      for (int j = 0; j < height; j++)
      {
        int* srcRow = src;
        int* dstRow = dst;
        for (int i = 0; i < width; i++)
        {
          int ci = *srcRow;
          int co = ci | unchecked((int)0xff000000);

          *dstRow = co;
          dstRow++;
          srcRow++;
        }
        src += srcPitch;
        dst += dstPitch;
      }
    }
  }
}
