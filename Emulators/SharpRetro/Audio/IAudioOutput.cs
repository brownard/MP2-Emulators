using SharpRetro.LibRetro;
using System;

namespace SharpRetro.Audio
{
  /// <summary>
  /// Interface for handling the audio callbacks from a libretro core.
  /// </summary>
  public interface IAudioOutput
  {
    /// <summary>
    /// Sets the <see cref="retro_system_timing"/> to use to determine the sample rate of the audio.
    /// </summary>
    /// <param name="timing">The timing info to use.</param>
    void SetTimingInfo(retro_system_timing timing);

    /// <summary>
    /// Process a single audio frame.
    /// </summary>
    /// <param name="left">The left sample.</param>
    /// <param name="right">The right sample.</param>
    void AudioSample(short left, short right);

    /// <summary>
    /// Process a batch of audio frames.
    /// </summary>
    /// <param name="data">Pointer to the audio data.</param>
    /// <param name="frames">The number of frames.</param>
    /// <returns>The number of frames actually processed.</returns>
    uint AudioSampleBatch(IntPtr data, uint frames);
  }
}
