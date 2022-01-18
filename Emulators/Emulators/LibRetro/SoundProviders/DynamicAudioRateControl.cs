using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulators.LibRetro.SoundProviders
{
  /// <summary>
  /// Class that calculates the audio playback rate, up to a maximum delta, required to maintain a consistent amount of audio buffered.
  /// </summary>
  /// <remarks>
  /// Old games consoles, and hence the libretro cores emulating them, tend to be highly synchronous and usually push exactly one frame
  /// of audio per one frame of video, which worked great when they were attached to an old CRT TV with a very precise fixed framerate.
  /// However emulated systems relying on the V blank of a modern graphics card for timing will usually have a framerate that differs
  /// slightly, e.g. running at 59.97hz instead of an expected 60hz, which will cause the audio buffer to over/underrun over time because video
  /// frames are pushed slightly faster/slower than the time it takes to playback the accompanying frame of audio at the original frequency.
  /// 
  /// This class attempts to compensate for that by monitoring the current fill of the audio buffer and calculating the audio playback rate
  /// needed to maintain a specified amount of audio buffered. However adjusting the audio playback speed will also change the pitch of the
  /// audio so a maximum delta is also specified so that the playback rate change is never more than this delta to avoid an audible pitch change.
  /// For this reason the playback rate can only compensate so much and it's therefore still necessary for the framerate to closely match the original source.
  /// </remarks>
  public class DynamicAudioRateControl
  {
    protected int _bufferSize;
    protected int _desiredBufferFill;
    protected double _rateControlDelta;

    public DynamicAudioRateControl(int bufferSize, double rateControlDelta)
    {
      _bufferSize = bufferSize;
      _desiredBufferFill = bufferSize / 2; // Aim to fill the buffer halfway
      _rateControlDelta = rateControlDelta;
    }

    public double Update(int buffered)
    {
      int delta = buffered - _desiredBufferFill;
      double direction = (double)delta / _desiredBufferFill;
      return 1.0 + _rateControlDelta * direction;
    }
  }
}
