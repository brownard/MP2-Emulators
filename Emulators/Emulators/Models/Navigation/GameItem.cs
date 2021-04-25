using Emulators.Common.Games;
using MediaPortal.Common.General;
using MediaPortal.Common.MediaManagement;
using MediaPortal.UiComponents.Media.Models.Navigation;
using System.Collections.Generic;

namespace Emulators.Models.Navigation
{
  public class GameItem : PlayableMediaItem
  {
    protected AbstractProperty _platformProperty = new WProperty(typeof(string));
    protected AbstractProperty _yearProperty = new WProperty(typeof(int?));
    protected AbstractProperty _descriptionProperty = new WProperty(typeof(string));
    protected AbstractProperty _certificationProperty = new WProperty(typeof(string));
    protected AbstractProperty _developerProperty = new WProperty(typeof(string));
    protected AbstractProperty _ratingProperty = new WProperty(typeof(double?));
    protected AbstractProperty _genresProperty = new WProperty(typeof(IEnumerable<string>));

    public GameItem(MediaItem mediaItem)
      : base(mediaItem)
    { }

    public override void Update(MediaItem mediaItem)
    {
      base.Update(mediaItem);
      SimpleTitle = Title;

      SingleMediaItemAspect aspect;
      if (MediaItemAspect.TryGetAspect(mediaItem.Aspects, GameAspect.Metadata, out aspect))
      {
        Platform = aspect.GetAttributeValue<string>(GameAspect.ATTR_PLATFORM);
        Year = aspect.GetAttributeValue<int?>(GameAspect.ATTR_YEAR);
        Description = aspect.GetAttributeValue<string>(GameAspect.ATTR_DESCRIPTION);
        Certification = aspect.GetAttributeValue<string>(GameAspect.ATTR_CERTIFICATION);
        Developer = aspect.GetAttributeValue<string>(GameAspect.ATTR_DEVELOPER);
        Rating = aspect.GetAttributeValue<double?>(GameAspect.ATTR_RATING);
        Genres = aspect.GetCollectionAttribute<string>(GameAspect.ATTR_GENRES) ?? new string[0];
      }
      else
      {
        Platform = null;
        Year = null;
        Description = null;
        Certification = null;
        Developer = null;
        Rating = null;
        Genres = new string[0];
      }
    }

    public AbstractProperty PlatformProperty
    {
      get { return _platformProperty; }
    }

    public string Platform
    {
      get { return (string)_platformProperty.GetValue(); }
      set { _platformProperty.SetValue(value); }
    }

    public AbstractProperty DeveloperProperty
    {
      get { return _developerProperty; }
    }

    public string Developer
    {
      get { return (string)_developerProperty.GetValue(); }
      set { _developerProperty.SetValue(value); }
    }

    public AbstractProperty YearProperty
    {
      get { return _yearProperty; }
    }

    public int? Year
    {
      get { return (int?)_yearProperty.GetValue(); }
      set { _yearProperty.SetValue(value); }
    }

    public AbstractProperty CertificationProperty
    {
      get { return _certificationProperty; }
    }

    public string Certification
    {
      get { return (string)_certificationProperty.GetValue(); }
      set { _certificationProperty.SetValue(value); }
    }

    public AbstractProperty DescriptionProperty
    {
      get { return _descriptionProperty; }
    }

    public string Description
    {
      get { return (string)_descriptionProperty.GetValue(); }
      set { _descriptionProperty.SetValue(value); }
    }

    public AbstractProperty RatingProperty
    {
      get { return _ratingProperty; }
    }

    public double? Rating
    {
      get { return (double?)_ratingProperty.GetValue(); }
      set { _ratingProperty.SetValue(value); }
    }

    public AbstractProperty GenresProperty
    {
      get { return _genresProperty; }
    }

    public IEnumerable<string> Genres
    {
      get { return (IEnumerable<string>)_genresProperty.GetValue(); }
      set { _genresProperty.SetValue(value); }
    }
  }
}
