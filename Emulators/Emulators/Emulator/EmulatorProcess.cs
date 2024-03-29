﻿using Emulators.Common.Emulators;
using Emulators.Input;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.MediaManagement;
using MediaPortal.Common.MediaManagement.DefaultItemAspects;
using MediaPortal.Common.ResourceAccess;
using MediaPortal.Common.SystemCommunication;
using MediaPortal.Common.UserManagement;
using MediaPortal.Common.UserProfileDataManagement;
using MediaPortal.UI.Control.InputManager;
using MediaPortal.UI.ServerCommunication;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Emulators.Emulator
{
  public class EmulatorProcess : IDisposable
  {
    protected MediaItem _mediaItem;
    protected string _gamePath;
    protected EmulatorConfiguration _emulatorConfiguration;
    protected Process _process;
    protected InputHandler _inputHandler;
    protected Key _mappedKey;

    public EmulatorProcess(MediaItem mediaItem, string gamePath, EmulatorConfiguration emulatorConfiguration, Key mappedKey)
    {
      _mediaItem = mediaItem;
      _gamePath = gamePath;
      _emulatorConfiguration = emulatorConfiguration;
      _mappedKey = mappedKey;
    }

    public event EventHandler Exited;
    protected virtual void OnExited(object sender, EventArgs e)
    {
      if (Exited != null)
        Exited(this, e);
      Task.Run(() => NotifyPlayed(_mediaItem));
    }

    public bool TryStart()
    {
      try
      {
        string path = CreatePath(_emulatorConfiguration, _gamePath);
        string arguments = CreateArguments(_emulatorConfiguration, _gamePath);
        ServiceRegistration.Get<ILogger>().Debug("EmulatorProcess: Starting '{0} {1}'", path, arguments);
        _process = new Process();
        _process.StartInfo = new ProcessStartInfo(path, arguments);
        _process.StartInfo.WorkingDirectory = string.IsNullOrEmpty(_emulatorConfiguration.WorkingDirectory) ? DosPathHelper.GetDirectory(_emulatorConfiguration.Path) : _emulatorConfiguration.WorkingDirectory;
        _process.EnableRaisingEvents = true;
        _process.Exited += OnExited;
        bool result = _process.Start();
        if (result && !_emulatorConfiguration.IsNative && _mappedKey != null && _mappedKey != Key.None)
        {
          _inputHandler = new InputHandler(_process.Id, _mappedKey);
          _inputHandler.MappedKeyPressed += OnMappedKeyPressed;
        }
        return result;
      }
      catch (Exception ex)
      {
        ServiceRegistration.Get<ILogger>().Error("EmulatorProcess: Error starting process", ex);
        return false;
      }
    }

    protected void OnMappedKeyPressed(object sender, EventArgs e)
    {
      Process process = _process;
      if (process != null)
      {
        uint? sendKey = _emulatorConfiguration.ExitsOnEscapeKey ? KeyboardHook.VK_ESCAPE : (uint?)null;
        _inputHandler.CloseWindow(process.MainWindowHandle, sendKey);
      }
    }

    protected string CreatePath(EmulatorConfiguration emulatorConfiguration, string gamePath)
    {
      if (emulatorConfiguration.IsNative)
        return gamePath;
      return _emulatorConfiguration.Path;
    }

    protected string CreateArguments(EmulatorConfiguration emulatorConfiguration, string gamePath)
    {
      if (emulatorConfiguration.IsNative)
        return emulatorConfiguration.Arguments;

      string format = emulatorConfiguration.UseQuotes ? "\"{0}\"" : "{0}";
      string arguments = emulatorConfiguration.Arguments ?? "";

      arguments = arguments.Replace(EmulatorConfiguration.WILDCARD_GAME_DIRECTORY, string.Format(format, DosPathHelper.GetDirectory(gamePath)));
      if (!arguments.Contains(EmulatorConfiguration.WILDCARD_GAME_PATH) && !arguments.Contains(EmulatorConfiguration.WILDCARD_GAME_PATH_NO_EXT))
        return string.Format("{0} {1}", arguments.TrimEnd(), string.Format(format, gamePath));
      return arguments.Replace(EmulatorConfiguration.WILDCARD_GAME_PATH, string.Format(format, gamePath))
        .Replace(EmulatorConfiguration.WILDCARD_GAME_PATH_NO_EXT, string.Format(format, DosPathHelper.GetFileNameWithoutExtension(gamePath)));
    }

    protected static async Task NotifyPlayed(MediaItem mediaItem)
    {
      IServerConnectionManager scm = ServiceRegistration.Get<IServerConnectionManager>();
      IContentDirectory cd = scm.ContentDirectory;
      if (cd != null)
      {
        IUserManagement userProfileDataManagement = ServiceRegistration.Get<IUserManagement>();
        if (userProfileDataManagement.IsValidUser)
          await cd.NotifyUserPlaybackAsync(userProfileDataManagement.CurrentUser.ProfileId, mediaItem.MediaItemId, 100, true);
        else
          await cd.NotifyPlaybackAsync(mediaItem.MediaItemId, true);
      }

      // Update loaded item also, so changes will be visible in GUI without reloading
      mediaItem.UserData[UserDataKeysKnown.KEY_PLAY_PERCENTAGE] = UserDataKeysKnown.GetSortablePlayPercentageString(100);
      if (MediaItemAspect.TryGetAttribute(mediaItem.Aspects, MediaAspect.ATTR_PLAYCOUNT, 0, out int currentPlayCount))
        MediaItemAspect.SetAttribute(mediaItem.Aspects, MediaAspect.ATTR_PLAYCOUNT, ++currentPlayCount);

      int userPlayCount = mediaItem.UserData.ContainsKey(UserDataKeysKnown.KEY_PLAY_COUNT) ? Convert.ToInt32(mediaItem.UserData[UserDataKeysKnown.KEY_PLAY_COUNT]) : 0;
      mediaItem.UserData[UserDataKeysKnown.KEY_PLAY_COUNT] = UserDataKeysKnown.GetSortablePlayCountString(++userPlayCount);
      ContentDirectoryMessaging.SendMediaItemChangedMessage(mediaItem, ContentDirectoryMessaging.MediaItemChangeType.Updated);
    }

    public void Dispose()
    {
      if (_inputHandler != null)
      {
        _inputHandler.Dispose();
        _inputHandler = null;
      }
      if (_process != null)
      {
        _process.Exited -= OnExited;
        _process.Dispose();
        _process = null;
      }
    }
  }
}