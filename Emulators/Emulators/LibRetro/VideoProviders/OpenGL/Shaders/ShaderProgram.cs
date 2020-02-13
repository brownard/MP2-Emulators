using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulators.LibRetro.VideoProviders.OpenGL.Shaders
{
  /// <summary>
  /// Wrapper for an OpenGl shader program that renders
  /// a texture to the currently bound frame buffer.
  /// </summary>
  public class ShaderProgram : IDisposable
  {
    public const string MVP_UNIFORM_NAME = "modelViewProjection";
    public const string POSITION_ATTRIB_NAME = "position";
    public const string TEXCOORDS_ATTRIB_NAME = "texCoords";
    public const string FRAGTEX_UNIFORM_NAME = "fragTex";

    protected uint _program;

    protected int _mvpLocation = -1;
    protected int _positionLocation = -1;
    protected int _texCoordsLocation = -1;
    protected int _fragTexLocation = -1;

    /// <summary>
    /// Id of the OpengGl program
    /// </summary>
    public uint Program
    {
      get { return _program; }
    }

    /// <summary>
    /// Location of the MVP matrix uniform.
    /// </summary>
    public int MvpUniformLocation
    {
      get { return _mvpLocation; }
    }

    /// <summary>
    /// Location of the fragment texture uniform.
    /// </summary>
    public int FragTexUniformLocation
    {
      get { return _fragTexLocation; }
    }

    /// <summary>
    /// Location of the vertex position attribute.
    /// </summary>
    public int PositionAttribLocation
    {
      get { return _positionLocation; }
    }

    /// <summary>
    /// Location of the texture coordinate attribute.
    /// </summary>
    public int TexCoordsAttribLocation
    {
      get { return _texCoordsLocation; }
    }

    /// <summary>
    /// Compiles the default vertex and fragment shaders then creates
    /// a new program and attaches and links the compiled shaders.
    /// </summary>
    /// <returns></returns>
    public bool Link()
    {
      if (_program != 0)
        throw new InvalidOperationException("Program has already been linked");

      int linked = 0;
      using (ShaderSource shader = new ShaderSource(ShaderSource.VERTEX_SHADER_RESOURCE, ShaderSource.FRAGMENT_SHADER_RESOURCE))
        if (shader.Compile())
          _program = CreateProgram(shader.VertexShader, shader.FragmentShader, out linked);

      if (linked == 0)
      {
        Dispose();
        return false;
      }

      // Now we're linked we can get the location of the uniforms and attributes.
      _mvpLocation = Gl.GetUniformLocation(_program, MVP_UNIFORM_NAME);
      _positionLocation = Gl.GetAttribLocation(_program, POSITION_ATTRIB_NAME);
      _texCoordsLocation = Gl.GetAttribLocation(_program, TEXCOORDS_ATTRIB_NAME);
      _fragTexLocation = Gl.GetUniformLocation(_program, FRAGTEX_UNIFORM_NAME);
      return true;
    }

    /// <summary>
    /// Use this program.
    /// </summary>
    public void Enable()
    {
      Gl.UseProgram(_program);
    }

    /// <summary>
    /// Stop using this program.
    /// </summary>
    public void Disable()
    {
      Gl.UseProgram(0);
    }

    /// <summary>
    /// Sets the value of the MVP uniform to the specified <paramref name="matrix"/>.
    /// </summary>
    /// <param name="matrix">The matrix to set.</param>
    public void SetMVPMatrix(Matrix4x4f matrix)
    {
      Gl.UniformMatrix4f(_mvpLocation, 1, false, matrix);
    }

    /// <summary>
    /// Sets the value of the frag texture uniform to the specified <paramref name="textureUnitIndex"/>.
    /// </summary>
    /// <param name="textureUnitIndex">The index of the texture unit that is bound to the texture to sample.</param>
    public void SetFragTexture(uint textureUnitIndex)
    {
      Gl.Uniform1(_fragTexLocation, textureUnitIndex);
    }

    /// <summary>
    /// Gets the location of the uniform with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the uniform.</param>
    /// <returns>The location of the uniform or a negative value if the uniform could not be found.</returns>
    public int GetUniformLocation(string name)
    {
      return Gl.GetUniformLocation(_program, name);
    }

    /// <summary>
    /// Gets the location of the attribute with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the attribute.</param>
    /// <returns>The location of the attribute or a negative value if the attribute could not be found.</returns>
    public int GetAttribLocation(string name)
    {
      return Gl.GetAttribLocation(_program, name);
    }

    /// <summary>
    /// Sets the value of the uniform with the specified <paramref name="name"/>
    /// to the specified <paramref name="matrix"/>.
    /// </summary>
    /// <param name="name">The name of the uniform to set.</param>
    /// <param name="matrix">The matrix to set.</param>
    public void SetMatrix4x4f(string name, Matrix4x4f matrix)
    {
      int location = Gl.GetUniformLocation(_program, name);
      if (location > -1)
        Gl.UniformMatrix4f(location, 1, false, matrix);
    }

    /// <summary>
    /// Sets the value of the uniform with the specified <paramref name="name"/>
    /// to the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="name">The name of the uniform to set.</param>
    /// <param name="value">The value to set.></param>
    public void SetInt(string name, uint value)
    {
      int location = Gl.GetUniformLocation(_program, name);
      if (location > -1)
        Gl.Uniform1(location, value);
    }

    protected uint CreateProgram(uint vertexShader, uint fragmentShader, out int linked)
    {
      int infologLength;
      StringBuilder infolog = new StringBuilder(1024);
      infolog.EnsureCapacity(1024);

      uint program = Gl.CreateProgram();
      Gl.AttachShader(program, vertexShader);
      Gl.AttachShader(program, fragmentShader);
      Gl.LinkProgram(program);
      Gl.GetProgram(program, ProgramProperty.LinkStatus, out linked);

      if (linked == 0)
        Gl.GetProgramInfoLog(_program, 1024, out infologLength, infolog);

      return program;
    }

    public void Dispose()
    {
      if (_program == 0)
        return;
      Gl.DeleteProgram(_program);
      _program = 0;
      _mvpLocation = -1;
      _positionLocation = -1;
      _texCoordsLocation = -1;
      _fragTexLocation = -1;
    }
  }
}
