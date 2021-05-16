using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.Messaging;
using MediaPortal.Plugins.InputDeviceManager.Messaging;
using SharpLib.Hid;
using System;

namespace Emulators.LibRetro.Controllers.Hid
{
  public class HidListener : IDisposable
  {
    #region Logger
    static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
    #endregion

    protected AsynchronousMessageQueue _messageQueue;

    public event EventHandler<Event> StateChanged;
    protected virtual void OnStateChanged(Event hidEvent)
    {
      StateChanged?.Invoke(this, hidEvent);
    }

    #region Message handling
    protected void SubscribeToMessages()
    {
      _messageQueue = new AsynchronousMessageQueue(this, new[] { InputDeviceMessaging.CHANNEL });
      _messageQueue.MessageReceived += OnMessageReceived;
      _messageQueue.Start();
    }

    protected virtual void UnsubscribeFromMessages()
    {
      if (_messageQueue == null)
        return;
      _messageQueue.Shutdown();
      _messageQueue = null;
    }

    void OnMessageReceived(AsynchronousMessageQueue queue, SystemMessage message)
    {
      if (message.ChannelName == InputDeviceMessaging.CHANNEL)
      {
        InputDeviceMessaging.MessageType messageType = (InputDeviceMessaging.MessageType)message.MessageType;
        switch (messageType)
        {
          case InputDeviceMessaging.MessageType.HidBroadcast:
            Event hidEvent = message.MessageData[InputDeviceMessaging.HID_EVENT] as Event;
            if (hidEvent != null)
              OnHidEvent(hidEvent);
            break;
        }
      }
    }
    #endregion

    public void Init()
    {
      SubscribeToMessages();
    }

    void OnHidEvent(Event hidEvent)
    {
      if (!hidEvent.Device.IsGamePad && !hidEvent.IsKeyboard)
        return;

#if DEBUG
      if (hidEvent.IsRepeat)
      {
        Logger.Debug("HID: Repeat");
      }
#endif
      
      OnStateChanged(hidEvent);
    }

    public void Dispose()
    {
      UnsubscribeFromMessages();
    }
  }
}