using OpenGL;
using SharpRetro.LibRetro;
using System;
using System.Collections.Generic;

namespace SharpRetro.OpenGL.Contexts
{
  public class GlContext : IDisposable
  {
    protected int _versionMajor;
    protected int _versionMinor;
    protected bool _coreContext;
    protected bool _debug;

    protected DeviceContext _deviceContext;
    protected IntPtr _glContext;

    public static GlContext TryCreate(DeviceContext deviceContext, int versionMajor, int versionMinor,
      retro_hw_context_type contextType, bool debug)
    {
      bool coreContext;
      if (contextType == retro_hw_context_type.RETRO_HW_CONTEXT_OPENGL_CORE)
        coreContext = true;
      else if (contextType == retro_hw_context_type.RETRO_HW_CONTEXT_OPENGL)
        coreContext = false;
      else // Unsupported context type
        return null;

      GlContext context = new GlContext(deviceContext, versionMajor, versionMinor, coreContext, debug);
      if (context.Create(IntPtr.Zero))
        return context;

      context.Dispose();
      return null;
    }

    public GlContext(DeviceContext deviceContext, int versionMajor, int versionMinor, bool coreContext, bool debug)
    {
      _deviceContext = deviceContext;
      _versionMajor = versionMajor;
      _versionMinor = versionMinor;
      _coreContext = coreContext;
      _debug = debug;
    }

    public bool Create(IntPtr sharedContext)
    {
      if (_deviceContext == null)
        return false;
      List<int> attributes = GetContextAttributes(_versionMajor, _versionMinor, _coreContext, _debug);
      _glContext = _deviceContext.CreateContextAttrib(sharedContext, attributes.ToArray());
      return _glContext != IntPtr.Zero;
    }

    public GlContext CreateSharedContext()
    {
      GlContext sharedContext = new GlContext(_deviceContext, _versionMajor, _versionMinor, _coreContext, _debug);
      if (!sharedContext.Create(_glContext))
        return null;
      return sharedContext;
    }

    public bool MakeCurrent()
    {
      return _deviceContext.MakeCurrent(_glContext);
    }

    public IntPtr Context
    {
      get { return _glContext; }
    }

    protected static List<int> GetContextAttributes(int versionMajor, int versionMinor, bool coreContext, bool debug)
    {
      List<int> attributes = new List<int>();

      // Version
      attributes.AddRange(new int[] {
        Wgl.CONTEXT_MAJOR_VERSION_ARB, versionMajor,
        Wgl.CONTEXT_MINOR_VERSION_ARB, versionMinor,
        Wgl.CONTEXT_RELEASE_BEHAVIOR_ARB, Wgl.CONTEXT_RELEASE_BEHAVIOR_NONE_ARB
      });

      // Debug context
      if (debug)
      {
        uint contextFlags = Wgl.CONTEXT_DEBUG_BIT_ARB;
        attributes.AddRange(new int[] {
          Wgl.CONTEXT_FLAGS_ARB, unchecked((int)contextFlags)
        });
      }

      // Core or compatibility profile
      uint contextProfile = coreContext ? Wgl.CONTEXT_CORE_PROFILE_BIT_ARB : Wgl.CONTEXT_COMPATIBILITY_PROFILE_BIT_ARB;
      attributes.AddRange(new int[] {
        Wgl.CONTEXT_PROFILE_MASK_ARB, unchecked((int)contextProfile)
      });

      // Attributes have to be 0 terminated
      attributes.Add(0);
      return attributes;
    }

    public void Dispose()
    {
      if (_deviceContext == null)
        return;
      _deviceContext.MakeCurrent(IntPtr.Zero);
      if (_glContext == IntPtr.Zero)
        return;
      _deviceContext.DeleteContext(_glContext);
      _glContext = IntPtr.Zero;

      // Don't dispose _deviceContext as we don't own it
    }
  }
}
