using Emulators.Settings.Configuration.ConfigurationClasses;
using MediaPortal.Common.General;
using MediaPortal.UiComponents.Configuration.ConfigurationControllers;
using System;

namespace Emulators.Settings.Configuration.ConfigurationControllers
{
  /// <summary>
  /// Custom controller for a number select setting with an associated boolean value for enabling and disabling the setting.
  /// </summary>
  public class YesNoNumberSelectController : NumberSelectController
  {
    protected AbstractProperty _yesProperty;

    protected bool _isUpdatingSetting = false;

    public YesNoNumberSelectController()
    {
      _yesProperty = new WProperty(typeof(bool), false);
    }

    public override Type ConfigSettingType
    {
      get { return typeof(YesNoNumberSelect); }
    }

    protected override string DialogScreen
    {
      get { return "dialog_configuration_yesnonumberselect"; }
    }

    public AbstractProperty YesProperty
    {
      get { return _yesProperty; }
    }

    public bool Yes
    {
      get { return (bool)_yesProperty.GetValue(); }
      set { _yesProperty.SetValue(value); }
    }

    protected override void SettingChanged()
    {
      // Don't reload the settings if the change is caused by this model
      // updating them as they may still be in a dirty state and contain
      // old valuse. See related comment in UpdateSetting.
      if (_isUpdatingSetting)
        return;
      base.SettingChanged();
      if (_setting == null)
        return;
      YesNoNumberSelect yesNo = (YesNoNumberSelect)_setting;
      Yes = yesNo.Yes;
    }

    protected override void UpdateSetting()
    {
      // Prevent SettingChanged from being handled whilst updating the setting
      // as setting the Yes property fires the handler before the numeric value
      // property has been updated, causing the old numeric value to be reloaded.
      _isUpdatingSetting = true;
      try
      {
        YesNoNumberSelect yesNo = (YesNoNumberSelect)_setting;
        yesNo.Yes = Yes;
        base.UpdateSetting();
      }
      finally
      {
        _isUpdatingSetting = false;
      }
    }
  }
}
