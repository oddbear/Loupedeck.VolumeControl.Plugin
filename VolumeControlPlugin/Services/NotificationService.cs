using System;
using System.Collections.Generic;

using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace Loupedeck.VolumeControlPlugin.Services
{
    public class NotificationService : MarshalByRefObject
    {
        public readonly Dictionary<string, CoreAudioDevice> Devices = new Dictionary<string, CoreAudioDevice>();
        private readonly CoreAudioController _controller;

        public NotificationService()
        {
            _controller = new CoreAudioController();

            var devices = _controller.GetDevices(AudioSwitcher.AudioApi.DeviceType.All, DeviceState.All);

            foreach (var device in devices)
            {
                if (device.Name == "Unknown")
                    continue;

                Devices[device.RealId] = device;
            }
        }
    }
}
