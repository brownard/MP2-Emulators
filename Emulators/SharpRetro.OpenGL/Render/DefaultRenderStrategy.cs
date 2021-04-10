using OpenGL;
using SharpRetro.OpenGL.Objects;
using SharpRetro.OpenGL.Shaders;
using System;

namespace SharpRetro.OpenGL.Render
{
  public class DefaultRenderStrategy : IRenderStrategy, IDisposable
  {
    protected static readonly float[] quadVertices = {
        // Positions        // Texture Coords
        -1.0f, 1.0f, 0.0f,  0.0f, 1.0f,
        -1.0f, -1.0f, 0.0f, 0.0f, 0.0f,
        1.0f, 1.0f, 0.0f,   1.0f, 1.0f,
        1.0f, -1.0f, 0.0f,  1.0f, 0.0f,
    };

    protected VertexArray _vertexArrayObject;
    protected ShaderProgram _program;

    public virtual void Create()
    {
      _vertexArrayObject = new VertexArray();
      _vertexArrayObject.Bind();

      _program = new ShaderProgram();
      _program.Link();

      _vertexArrayObject.SetInterleavedBuffer((uint)_program.PositionAttribLocation, 3, (uint)_program.TexCoordsAttribLocation, 2, quadVertices);

      _vertexArrayObject.Unbind();
    }

    public virtual void Destroy()
    {
      GLObject.TryDispose(ref _program);
      GLObject.TryDispose(ref _vertexArrayObject);
    }

    public virtual void Render(int width, int height, bool bottomLeftOrigin, uint texture)
    {
      RenderQuad(width, height, bottomLeftOrigin, texture);
    }

    protected void RenderQuad(int width, int height, bool bottomLeftOrigin, uint texture)
    {
      Gl.Disable(EnableCap.DepthTest);
      Gl.Disable(EnableCap.Lighting);
      Gl.Disable(EnableCap.AlphaTest);

      Gl.Enable(EnableCap.Texture2d);

      _vertexArrayObject.Bind();
      _program.Enable();
      
      Gl.DrawBuffers(Gl.COLOR_ATTACHMENT0);
      CheckError();

      Gl.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
      Gl.Clear(ClearBufferMask.ColorBufferBit);
      Gl.Viewport(0, 0, width, height);

      // Compute MVP
      float scaleY = bottomLeftOrigin ? -1 : 1;
      Matrix4x4f mvp = Matrix4x4f.Scaled(1, scaleY, 1);
      _program.SetMVPMatrix(mvp);

      Gl.ActiveTexture(TextureUnit.Texture0);
      Gl.BindTexture(TextureTarget.Texture2d, texture);
      _program.SetFragTexture(0);
      CheckError();

      Gl.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
      CheckError();

      Gl.BindTexture(TextureTarget.Texture2d, 0);

      Gl.DrawBuffers(0);
      _program.Disable();
      _vertexArrayObject.Unbind();
      Gl.Disable(EnableCap.Texture2d);
    }

    protected void CheckError()
    {
      ErrorCode error = Gl.GetError();
      if (error != ErrorCode.NoError)
        return;
    }

    public void Dispose()
    {
      Destroy();
    }
  }
}
