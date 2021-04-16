using Emulators.Common.Games;
using Emulators.Game;
using Emulators.Models.Navigation;
using MediaPortal.Common;
using MediaPortal.Common.Commands;
using MediaPortal.UiComponents.Media.MediaLists;

namespace Emulators.MediaExtensions.MediaLists
{
  public class FavoriteGameMediaListProvider : BaseFavoriteMediaListProvider
  {
    public FavoriteGameMediaListProvider()
    {
      _changeAspectId = GameAspect.ASPECT_ID;
      _necessaryMias = EmulatorsConsts.NECESSARY_GAME_MIAS;
      _playableConverterAction = item => new GameItem(item)
      {
        Command = new MethodDelegateCommand(() => ServiceRegistration.Get<IGameLauncher>().LaunchGame(item))
      };
      _navigationInitializerType = typeof(GamesLibrary);
    }
  }
}
