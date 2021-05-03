using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.Messaging;
using MediaPortal.Plugins.InputDeviceManager;
using MediaPortal.Plugins.InputDeviceManager.Messaging;
using SharpLib.Hid;
using SharpLib.Hid.Usage;
using SharpLib.Win32;
using System;
using System.Collections.Generic;

namespace Emulators.LibRetro.Controllers.Hid
{
  public class HidStateEventArgs : EventArgs
  {
    public HidStateEventArgs(HidState state)
    {
      State = state;
    }
    public HidState State { get; private set; }
  }

  public class HidListener : IDisposable
  {
    #region Logger
    static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
    #endregion

    protected AsynchronousMessageQueue _messageQueue;

    public event EventHandler<HidStateEventArgs> StateChanged;
    protected virtual void OnStateChanged(HidStateEventArgs e)
    {
      var handler = StateChanged;
      if (handler != null)
        handler(this, e);
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

    void OnHidEvent(Event aHidEvent)
    {
      if (!aHidEvent.Device.IsGamePad)
        return;

#if DEBUG
      if (aHidEvent.IsRepeat)
      {
        Logger.Debug("HID: Repeat");
      }
#endif
      HashSet<ushort> buttons = new HashSet<ushort>();
      foreach (ushort usage in aHidEvent.Usages)
        buttons.Add(usage);

      //For each axis
      Dictionary<ushort, HidAxisState> axisStates = new Dictionary<ushort, HidAxisState>();
      foreach (KeyValuePair<HIDP_VALUE_CAPS, uint> entry in aHidEvent.UsageValues)
      {
        //HatSwitch is handled separately as direction pad state
        if (entry.Key.IsRange || entry.Key.NotRange.Usage == (ushort)GenericDesktop.HatSwitch)
          continue;
        //Get our usage type
        Type usageType = HidUtils.UsageType((UsagePage)entry.Key.UsagePage);
        if (usageType == null)
          continue;
        //Get the name of our axis
        string name = Enum.GetName(usageType, entry.Key.NotRange.Usage);
        ushort index = entry.Key.NotRange.DataIndex;
        axisStates[index] = new HidAxisState(name, index, entry.Value, entry.Key.BitSize);
      }

      DirectionPadState directionPadState = aHidEvent.GetDirectionPadState();

      HidState state = new HidState
      {
        VendorId = aHidEvent.Device.VendorId,
        ProductId = aHidEvent.Device.ProductId,
        Name = aHidEvent.Device.Name,
        FriendlyName = aHidEvent.Device.FriendlyName,
        Buttons = buttons,
        AxisStates = axisStates,
        DirectionPadState = directionPadState
      };
      OnStateChanged(new HidStateEventArgs(state));
    }

    public void Dispose()
    {
      UnsubscribeFromMessages();
    }
  }
}