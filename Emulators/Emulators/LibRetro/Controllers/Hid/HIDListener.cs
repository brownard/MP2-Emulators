using HidInput.Messaging;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.Messaging;
using SharpLib.Hid;
using System;

namespace Emulators.LibRetro.Controllers.Hid
{
  public class StateChangedEventArgs : EventArgs
  {
    public StateChangedEventArgs(Event hidEvent)
    {
      HidEvent = hidEvent;
    }

    public Event HidEvent { get; }
    public bool Handled { get; set; }
  }

  public class HidListener : IDisposable
  {
    #region Logger
    static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
    #endregion

    protected SynchronousMessageQueue _messageQueue;

    public event EventHandler<StateChangedEventArgs> StateChanged;
    protected virtual void OnStateChanged(StateChangedEventArgs e)
    {
      StateChanged?.Invoke(this, e);
    }

    #region Message handling

    protected void SubscribeToMessages()
    {
      _messageQueue = new SynchronousMessageQueue(this, new[] { HidMessaging.PREVIEW_CHANNEL });
      _messageQueue.MessagesAvailable += OnMessageReceived;
      _messageQueue.RegisterAtAllMessageChannels();
    }

    private void OnMessageReceived(SynchronousMessageQueue queue)
    {
      SystemMessage message;
      while ((message = queue.Dequeue()) != null)
      {
        if (message.ChannelName == HidMessaging.PREVIEW_CHANNEL)
        {
          HidMessaging.MessageType messageType = (HidMessaging.MessageType)message.MessageType;
          switch (messageType)
          {
            case HidMessaging.MessageType.HidEvent:
              Event hidEvent = message.MessageData[HidMessaging.EVENT] as Event;
              if (hidEvent != null)
                message.MessageData[HidMessaging.HANDLED] = OnHidEvent(hidEvent);
              break;
          }
        }
      }
    }

    protected virtual void UnsubscribeFromMessages()
    {
      if (_messageQueue == null)
        return;
      _messageQueue.Dispose();
      _messageQueue = null;
    }

    #endregion

    public void Init()
    {
      SubscribeToMessages();
    }

    bool OnHidEvent(Event hidEvent)
    {
      if (!hidEvent.Device?.IsGamePad == true && !hidEvent.IsKeyboard)
        return false;

#if DEBUG
      if (hidEvent.IsRepeat)
      {
        Logger.Debug("HID: Repeat");
      }
#endif

      StateChangedEventArgs e = new StateChangedEventArgs(hidEvent);
      try
      {
        OnStateChanged(e);
      }
      catch (Exception ex)
      {
        Logger.Error($"{nameof(HidListener)}: Exception handling Hid event", ex);
      }
      return e.Handled;
    }

    public void Dispose()
    {
      UnsubscribeFromMessages();
    }
  }
}