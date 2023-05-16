using OpenGL;
using SharpRetro.OpenGL.Contexts;
using SharpRetro.OpenGL.Objects;
using SharpRetro.OpenGL.Shaders;
using System;
using System.Diagnostics;
using System.Text;

namespace SharpRetro.OpenGL.Render
{
  public class DefaultRenderStrategy : IRenderStrategy, IDisposable
  {
    protected static readonly float[] quadVertices = {
        // Positions    // Texture Coords
        -1.0f,  1.0f,   0.0f, 1.0f,
        -1.0f, -1.0f,   0.0f, 0.0f,
         1.0f, -1.0f,   1.0f, 0.0f,
         1.0f,  1.0f,   1.0f, 1.0f,
    };

    protected static readonly uint[] indices =
    {
      0, 1, 2, 0, 2, 3
    };

    protected VertexArray _vertexArrayObject;
    protected ShaderProgram _program;

    public virtual void Create()
    {
      _program = new ShaderProgram();
      _program.Link();

      _vertexArrayObject = new VertexArray();
      _vertexArrayObject.SetInterleavedBuffer((uint)_program.PositionAttribLocation, 2, (uint)_program.TexCoordsAttribLocation, 2, quadVertices, indices);
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
      CheckError();

      _program.Enable();
      CheckError();

      _vertexArrayObject.Bind();
      CheckError();

      Gl.DrawBuffers(Gl.COLOR_ATTACHMENT0);
      CheckError();

      Gl.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
      Gl.Clear(ClearBufferMask.ColorBufferBit);
      Gl.Viewport(0, 0, width, height);

      // Compute MVP
      float scaleY = bottomLeftOrigin ? -1 : 1;
      Matrix4x4f mvp = Matrix4x4f.Scaled(1, scaleY, 1);
      _program.SetMVPMatrix(mvp);
      CheckError();

      Gl.ActiveTexture(TextureUnit.Texture0);
      CheckError();

      Gl.BindTexture(TextureTarget.Texture2d, texture);
      CheckError();

      _program.SetFragTexture(0);
      CheckError();

      Gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);
      CheckError();

      Gl.BindTexture(TextureTarget.Texture2d, 0);
      CheckError();

      Gl.DrawBuffers(0);
      CheckError();

      _program.Disable();
      CheckError();

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
