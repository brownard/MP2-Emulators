using Emulators.Game;
using Emulators.Models.Navigation;
using MediaPortal.Common;
using MediaPortal.Common.Commands;
using MediaPortal.UiComponents.Media.MediaLists;

namespace Emulators.MediaExtensions.MediaLists
{
  public class LatestGameMediaListProvider : BaseLatestMediaListProvider
  {
    public LatestGameMediaListProvider()
    {
      _necessaryMias = EmulatorsConsts.NECESSARY_GAME_MIAS;
      _optionalMias = EmulatorsConsts.OPTIONAL_GAME_MIAS;
      _playableConverterAction = item => new GameItem(item)
      {
        Command = new MethodDelegateCommand(() => ServiceRegistration.Get<IGameLauncher>().LaunchGame(item))
      };
      _navigationInitializerType = typeof(GamesLibrary);
    }
  }
}
