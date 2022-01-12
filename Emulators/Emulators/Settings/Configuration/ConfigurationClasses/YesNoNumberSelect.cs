using MediaPortal.Common.Configuration.ConfigurationClasses;

namespace Emulators.Settings.Configuration.ConfigurationClasses
{
  /// <summary>
  /// Number select setting with an additional boolean value for enabling/disabling the setting
  /// </summary>
  public class YesNoNumberSelect : LimitedNumberSelect
  {
    #region Variables

    protected bool _yes = false;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets.
    /// </summary>
    public bool Yes
    {
      get { return _yes; }
      set
      {
        if (_yes != value)
        {
          _yes = value;
          NotifyChange();
        }
      }
    }

    #endregion
  }
}
