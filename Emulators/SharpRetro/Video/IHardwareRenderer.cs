using SharpRetro.LibRetro;

namespace SharpRetro.Video
{
  /// <summary>
  /// Interface for video outputs that support hardware rendering using the <see cref="retro_hw_render_callback"/> interface.
  /// </summary>
  public interface IHardwareRenderer
  {
    /// <summary>
    /// Called when a libretro core requests a hardware render context.
    /// Implementations of this method should try and create the requested hardware context
    /// and set the <see cref="retro_hw_render_callback.get_current_framebuffer"/> and
    /// <see cref="retro_hw_render_callback.get_proc_address"/> callbacks. 
    /// </summary>
    /// <param name="hwRenderCallback">Callback interface from the libretro core.</param>
    /// <returns><c>true</c> if the requested context could be created.</returns>
    bool SetHWRender(ref retro_hw_render_callback hwRenderCallback);
    
    /// <summary>
    /// Called after a call to <see cref="SetHWRender(ref retro_hw_render_callback)"/> succeeds
    /// and any callbacks have been passed back to the core. Implementations should typically
    /// create any buffers here and call the <see cref="retro_hw_render_callback.context_reset"/>
    /// callback.
    /// </summary>
    void Create();

    /// <summary>
    /// Called when the libretro core no longer needs the hardware context.
    /// Implementations should release any resources and destroy the context here.
    /// </summary>
    void Destroy();
  }
}
