using System;
using System.Collections.Generic;
using System.Linq;

using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace Loupedeck.VolumeControlPlugin.Services
{
    public class NotificationService : IMMNotificationClient
    {
        private readonly Dictionary<string, MMDevice> Devices = new Dictionary<string, MMDevice>();
        private readonly MMDeviceEnumerator _deviceEnumerator;

        public MMDevice GetDevice(string deviceId)
        {
            try
            {
                var device = Devices[deviceId];
                if (device.InstanceId == "Unknown")
                    return null;

                return device;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public MMDevice[] GetDevices(DataFlow dataFlow)
        {
            return Devices.Values
                .Where(device =>
                {
                    try
                    {
                        if (device.InstanceId == "Unknown")
                            return false;

                        return device.DataFlow == dataFlow;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                })
                .ToArray();
        }

        public NotificationService()
        {
            _deviceEnumerator = new MMDeviceEnumerator();
            _deviceEnumerator.RegisterEndpointNotificationCallback(this);

            var devices = _deviceEnumerator
                .EnumerateAudioEndPoints(DataFlow.All, DeviceState.All);

            foreach (var device in devices)
            {
                try
                {
                    var deviceFriendlyName = device.DeviceFriendlyName;
                    var friendlyName = device.FriendlyName;
                    var dataFlow = device.DataFlow;

                    Devices[device.ID] = device;
                }
                catch (Exception)
                {

                }
            }
        }
        
        /// <summary>
        /// Gets the newly set default device, and it's Flow and Role.
        /// Used to search for expected default device, and set if the new is not the correct one.
        /// </summary>
        /// <param name="flow">Render or Capture</param>
        /// <param name="role">Multimedia or Communications</param>
        /// <param name="defaultDeviceId">The new default deviceId.</param>
        void IMMNotificationClient.OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
        {
            Console.WriteLine($"{DateTime.Now} [OnDefaultDeviceChanged]: {flow}, {role}, {defaultDeviceId}");
        }

        void IMMNotificationClient.OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            Console.WriteLine($"{DateTime.Now} [OnDeviceStateChanged]: {deviceId}, {newState}");
        }

        void IMMNotificationClient.OnDeviceAdded(string pwstrDeviceId)
        {
            //This does not seem to do anything, if I unplug the device or replug it, I get:
            //OnDeviceStateChanged: Active -> NotPresent or NotPresent -> Active
            //Might have something to do if the drivers are installed or uninstalled?
            Console.WriteLine($"{DateTime.Now} [OnDeviceAdded]: {pwstrDeviceId}");
        }

        void IMMNotificationClient.OnDeviceRemoved(string deviceId)
        {
            //See OnDeviceAdded.
            Console.WriteLine($"{DateTime.Now} [OnDeviceRemoved]: {deviceId}");
        }

        void IMMNotificationClient.OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
        {
            //Events triggered on change of different properties:
            //Ex. Volume change and mute, triggers one event.
            //Other like 16/24bit 48K HZ, triggers a lot of events.
            Console.WriteLine($"{DateTime.Now} [OnPropertyValueChanged]: {pwstrDeviceId} || {key.formatId} || {key.propertyId}");
        }
    }
}
