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
    /// Process a single audio frame.
    /// </summary>
    /// <param name="left">The left sample.</param>
    /// <param name="right">The right sample.</param>
    void RetroAudioSample(short left, short right);

    /// <summary>
    /// Process a batch of audio frames.
    /// </summary>
    /// <param name="data">Pointer to the audio data.</param>
    /// <param name="frames">The number of frames.</param>
    /// <returns>The number of frames actually processed.</returns>
    uint RetroAudioSampleBatch(IntPtr data, uint frames);

    /// <summary>
    /// Sets the <see cref="TimingInfo"/> to use to determine the sample rate of the audio.
    /// </summary>
    /// <param name="timingInfo">The timing info to use.</param>
    void SetTimingInfo(TimingInfo timingInfo);
  }
}
