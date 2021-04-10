using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRetro.OpenGL.Objects
{
  public class VertexArray : GLObject
  {
    protected uint _vertexBuffer;

    public uint VertexBuffer
    {
      get { return _vertexBuffer; }
    }

    public void Bind()
    {
      Gl.BindVertexArray(_id);
    }

    public void Unbind()
    {
      Gl.BindVertexArray(0);
    }

    /// <summary>
    /// Create a new vertex buffer from interleaved position and texture vertices,
    /// defines the vertex attribute data for the given locations and enables them
    /// on the currently bound vertex array object.
    /// </summary>
    /// <param name="positionLocation">The location of the position vertex attribute.</param>
    /// <param name="positionSize">The number of floats in each position vertex.</param>
    /// <param name="texLocaton">The location of the texture vertex attribute.</param>
    /// <param name="texSize">The number of floats in each texture vertex.</param>
    /// <param name="buffer">Interleved buffer of position and texture vertices.</param>
    public void SetInterleavedBuffer(uint positionLocation, int positionSize, uint texLocaton, int texSize, float[] buffer)
    {
      // Store the raw vertices
      _vertexBuffer = Gl.GenBuffer();
      Gl.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
      Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(buffer.Length * sizeof(float)), buffer, BufferUsage.StaticDraw);

      if (positionSize > 0)
      {
        // Define the position attribute data, stride is the sum of the position and texture vertex sizes
        Gl.VertexAttribPointer(positionLocation, positionSize, VertexAttribType.Float, false, (positionSize + texSize) * sizeof(float), IntPtr.Zero);
        Gl.EnableVertexAttribArray(positionLocation);
      }

      if (texSize > 0)
      {
        // Define the texture attribute data, stride is the sum of the position and texture vertex sizes and offset is the position size
        Gl.VertexAttribPointer(texLocaton, texSize, VertexAttribType.Float, false, (positionSize + texSize) * sizeof(float), new IntPtr(positionSize * sizeof(float)));
        Gl.EnableVertexAttribArray(texLocaton);
      }

      Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    protected override uint Generate()
    {
      return Gl.GenVertexArray();
    }

    protected override void Delete(uint id)
    {
      Gl.DeleteVertexArrays(id);
    }

    public override void Dispose()
    {
      if (_vertexBuffer != 0)
        Gl.DeleteBuffers(_vertexBuffer);
      _vertexBuffer = 0;
      base.Dispose();
    }
  }
}
