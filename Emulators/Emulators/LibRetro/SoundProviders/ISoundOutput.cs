using Emulators.LibRetro.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulators.LibRetro.SoundProviders
{
  public interface ISoundOutput : IDisposable
  {
    bool Play();
    void Pause();
    void UnPause();
    void SetVolume(int volume);
    void SetSynchronizationStrategy(SynchronizationStrategy strategy);
    bool HasAudio { get; set; }
  }
}
