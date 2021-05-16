using SharpLib.Hid;
using SharpLib.Hid.Usage;
using SharpLib.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Emulators.LibRetro.Controllers.Hid
{
  static class HidUtils
  {
    public static List<Device> GetHidDevices()
    {
      List<Device> devices = new List<Device>();
      //Get our list of devices
      RAWINPUTDEVICELIST[] ridList = null;
      uint deviceCount = 0;
      int res = Function.GetRawInputDeviceList(ridList, ref deviceCount, (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICELIST)));
      if (res == -1)
      {
        //Just give up then
        return devices;
      }

      ridList = new RAWINPUTDEVICELIST[deviceCount];
      res = Function.GetRawInputDeviceList(ridList, ref deviceCount, (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICELIST)));
      if (res != deviceCount)
      {
        //Just give up then
        return devices;
      }

      foreach (RAWINPUTDEVICELIST device in ridList)
      {
        Device hidDevice;
        //Try create our HID device.
        try
        {
          hidDevice = new Device(device.hDevice);
        }
        catch /*(System.Exception ex)*/
        {
          //Just skip that device then
          continue;
        }
        devices.Add(hidDevice);
      }
      return devices;
    }

    /// <summary>
    /// Provide the type for the usage corresponding to the given usage page.
    /// </summary>
    /// <param name="aUsagePage"></param>
    /// <returns></returns>
    public static Type UsageType(SharpLib.Hid.UsagePage aUsagePage)
    {
      switch (aUsagePage)
      {
        case SharpLib.Hid.UsagePage.GenericDesktopControls:
          return typeof(SharpLib.Hid.Usage.GenericDesktop);

        case SharpLib.Hid.UsagePage.Consumer:
          return typeof(SharpLib.Hid.Usage.ConsumerControl);

        case SharpLib.Hid.UsagePage.WindowsMediaCenterRemoteControl:
          return typeof(SharpLib.Hid.Usage.WindowsMediaCenterRemoteControl);

        case SharpLib.Hid.UsagePage.Telephony:
          return typeof(SharpLib.Hid.Usage.TelephonyDevice);

        case SharpLib.Hid.UsagePage.SimulationControls:
          return typeof(SharpLib.Hid.Usage.SimulationControl);

        case SharpLib.Hid.UsagePage.GameControls:
          return typeof(SharpLib.Hid.Usage.GameControl);

        case SharpLib.Hid.UsagePage.GenericDeviceControls:
          return typeof(SharpLib.Hid.Usage.GenericDevice);

        default:
          return null;
      }
    }

    /// <summary>
    /// Returns whether the specified HID device name belongs to an XInput device.
    /// </summary>
    /// <remarks>
    /// If the device name contains "IG_", e.g. "\\\\?\\HID#VID_045E&PID_02A1&IG_00#7&a222c90&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}", then it's an XInput device.
    /// See https://docs.microsoft.com/en-us/windows/win32/xinput/xinput-and-directinput.
    /// </remarks>
    /// <param name="deviceName">The device name of the HID device.</param>
    /// <returns><c>True</c> if the device name belongs to an XInput device.</returns>
    public static bool IsXInputDevice(string deviceName)
    {
      return !string.IsNullOrEmpty(deviceName) && deviceName.Contains("IG_");
    }

    public static HidState GetGamepadState(Event aHidEvent)
    {
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
        Type usageType = UsageType((UsagePage)entry.Key.UsagePage);
        if (usageType == null)
          continue;
        //Get the name of our axis
        string name = Enum.GetName(usageType, entry.Key.NotRange.Usage);
        ushort index = entry.Key.NotRange.DataIndex;
        axisStates[index] = new HidAxisState(name, index, entry.Value, entry.Key.BitSize);
      }

      DirectionPadState directionPadState;
      try
      {
        directionPadState = aHidEvent.GetDirectionPadState();
      }
      catch
      {
        directionPadState = DirectionPadState.Rest;
      }

      return new HidState
      {
        VendorId = aHidEvent.Device?.VendorId ?? 0,
        ProductId = aHidEvent.Device?.ProductId ?? 0,
        Name = aHidEvent.Device?.Name,
        FriendlyName = aHidEvent.Device?.FriendlyName,
        Buttons = buttons,
        AxisStates = axisStates,
        DirectionPadState = directionPadState
      };
    }
  }
}
