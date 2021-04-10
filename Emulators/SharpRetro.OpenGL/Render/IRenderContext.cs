using System;

namespace SharpRetro.OpenGL.Render
{
  public interface IRenderContext
  {
    /// <summary>
    /// Called before this render context is rendered to.
    /// </summary>
    /// <param name="width">The width of the render surface to render to.</param>
    /// <param name="height">The height of the render surface to render to.</param>
    void BeginRender(int width, int height);

    /// <summary>
    /// Called after this render context has been rendered to.
    /// </summary>
    void EndRender();

    /// <summary>
    /// Creates buffers of the specified size for rendering to.
    /// Can be called multiple times, implementations should
    /// destroy and recreate the buffers if necessary.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    void CreateBuffers(int width, int height);

    /// <summary>
    /// Destroy any created buffers.
    /// </summary>
    void DestroyBuffers();

    /// <summary>
    /// Reads the contents of the first color attachment to the provided IntPtr. Callers
    /// should ensure that the IntPtr points to writable memory of at least the required size.
    /// </summary>
    /// <param name="width">The width of the pixel data to read.</param>
    /// <param name="height">The height of the pixel data to read.</param>
    /// <param name="data">POinter to write the pixel data to.</param>
    void ReadPixels(int width, int height, IntPtr data);
  }
}