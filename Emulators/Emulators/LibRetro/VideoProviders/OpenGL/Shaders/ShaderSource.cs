using OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Emulators.LibRetro.VideoProviders.OpenGL.Shaders
{
  public class ShaderSource : IDisposable
  {
    public const string VERTEX_SHADER_RESOURCE = "Emulators.LibRetro.VideoProviders.OpenGL.Shaders.DefaultVertexShader.glsl";
    public const string FRAGMENT_SHADER_RESOURCE = "Emulators.LibRetro.VideoProviders.OpenGL.Shaders.DefaultFragmentShader.glsl";

    protected string _vertexShaderString;
    protected string _fragmentShaderString;

    protected uint _vertexShader;
    protected uint _fragmentShader;

    public ShaderSource(string vertexShaderResource, string fragmentShaderResource)
    {
      _vertexShaderString = GetEmbeddedResourceString(vertexShaderResource);
      _fragmentShaderString = GetEmbeddedResourceString(fragmentShaderResource);
    }

    public uint VertexShader
    {
      get { return _vertexShader; }
    }

    public uint FragmentShader
    {
      get { return _fragmentShader; }
    }

    public bool Compile()
    {
      int infologLength;
      StringBuilder infolog = new StringBuilder(1024);
      infolog.EnsureCapacity(1024);

      // Vertex shader
      int vertexShaderCompiled;
      _vertexShader = CompileShader(new[] { _vertexShaderString }, ShaderType.VertexShader, out vertexShaderCompiled);
      if (vertexShaderCompiled == 0)
        Gl.GetShaderInfoLog(_vertexShader, 1024, out infologLength, infolog);

      // Fragment shader
      int fragmentShaderCompiled;
      _fragmentShader = CompileShader(new[] { _fragmentShaderString }, ShaderType.FragmentShader, out fragmentShaderCompiled);
      if (fragmentShaderCompiled == 0)
        Gl.GetShaderInfoLog(_fragmentShader, 1024, out infologLength, infolog);

      return vertexShaderCompiled > 0 && fragmentShaderCompiled > 0;      
    }

    protected uint CompileShader(string[] shaderSource, ShaderType shaderType, out int compiled)
    {
      uint shader = Gl.CreateShader(shaderType);
      Gl.ShaderSource(shader, shaderSource);
      Gl.CompileShader(shader);
      Gl.GetShader(shader, ShaderParameterName.CompileStatus, out compiled);
      return shader;
    }

    protected static string GetEmbeddedResourceString(string resourceName)
    {
      using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
      using (var streamReader = new StreamReader(resourceStream))
        return streamReader.ReadToEnd();
    }

    public void Dispose()
    {
      Gl.DeleteShader(_vertexShader);
      Gl.DeleteShader(_fragmentShader);
    }
  }
}
