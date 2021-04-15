using MediaPortal.Common.MediaManagement;
using MediaPortal.Common.MediaManagement.DefaultItemAspects;

namespace Emulators.LibRetro
{
  public class LibRetroMediaItem : MediaItem
  {
    public LibRetroMediaItem(string libRetroPath, MediaItem originalMediaItem)
      : base(originalMediaItem.MediaItemId, originalMediaItem.Aspects, originalMediaItem.UserData)
    {
      LibRetroPath = libRetroPath;
      //otherwise MP2's player manager won't try and find a player 
      MediaItemAspect.GetOrCreateAspect(_aspects, VideoAspect.Metadata);
    }

    public string LibRetroPath { get; set; }
    public string ExtractedPath { get; set; }
  }
}
