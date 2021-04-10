using NUnit.Framework;
using OpenGL;
using SharpRetro.OpenGL.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
  [TestFixture]
  public class OpenGlTests
  {
    [Test]
    public void TryCreateContext()
    {
      int major = 2;
      int minor = 1;

      Gl.Initialize();
      var deviceContext = DeviceContext.Create();

      GlContext glContext = GlContext.TryCreate(deviceContext, major, minor, SharpRetro.LibRetro.retro_hw_context_type.RETRO_HW_CONTEXT_OPENGL, false);
      Assert.NotNull(glContext);
      glContext.Dispose();
    }
  }
}
