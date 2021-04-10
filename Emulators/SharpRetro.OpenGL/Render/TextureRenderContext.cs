using OpenGL;
using SharpRetro.OpenGL.Objects;
using System;

namespace SharpRetro.OpenGL.Render
{
  /// <summary>
  /// Implementation of <see cref="IRenderContext"/> that renders to an OpenGL texture.
  /// </summary>
  public class TextureRenderContext : RenderContext
  {
    protected Texture2D _textureBuffer;

    /// <summary>
    /// The texture that will be rendered to.
    /// </summary>
    public Texture2D TextureBuffer
    {
      get { return _textureBuffer; }
    }

    protected override void CreateBuffersOverride(int width, int height)
    {
      CreateTextureBuffer(width, height);
    }

    protected override void DestroyBuffersOverride()
    {
      DestroyTextureBuffer();
    }

    protected virtual void CreateTextureBuffer(int width, int height)
    {
      _textureBuffer = new Texture2D();
      _textureBuffer.Bind();
      _textureBuffer.SetStorage(width, height);
      _framebuffer.AttachTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _textureBuffer, 0);
      _textureBuffer.Unbind();
    }

    protected virtual void DestroyTextureBuffer()
    {
      GLObject.TryDispose(ref _textureBuffer);
    }
  }
}
