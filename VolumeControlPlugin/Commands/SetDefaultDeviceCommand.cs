using System.Linq;
using System.Threading.Tasks;

using CoreDeviceType = AudioSwitcher.AudioApi.DeviceType;
using Loupedeck.VolumeControlPlugin.Helpers;
using AudioSwitcher.AudioApi.CoreAudio;
using System;

namespace Loupedeck.VolumeControlPlugin.Commands
{
    abstract class SetDefaultDeviceCommand : PluginMultistateDynamicCommand
    {
        private readonly bool _isCommunication;
        private readonly CoreDeviceType _deviceType;
        private readonly string _groupName;

        private DeviceHelper _deviceHelper;

        private VolumeControlPlugin _plugin;

        protected SetDefaultDeviceCommand(bool isCommunication, CoreDeviceType deviceType, string groupName)
        {
            PluginLog.Info("Initializing SetDefaultDeviceCommand: "+ groupName);
            
            this.AddState("Not default device", "Not selected as default");
            this.AddState("Default device", "Selected as default");
            this.AddState("Device disabled", "Not found or disabled"); // Displayed also, when device is disconnected

            _isCommunication = isCommunication;
            _deviceType = deviceType;
            _groupName = groupName;
        }

        protected override bool OnLoad()
        {
            _plugin = base.Plugin as VolumeControlPlugin;

            if (_plugin is null)
                return false;

            _deviceHelper = new DeviceHelper(_plugin);

            if (_plugin.NotificationService is null)
                return false;

            var devices = _plugin.NotificationService.Devices
                .Where(device => device.Value.DeviceType == _deviceType)
                .ToArray();

            foreach (var device in devices)
            {
                this.AddParameter(device.Key, device.Value.FullName, _groupName);
                this.SetCurrentState(device.Key, this.GetDeviceCommandStateIndex(device.Value)); // Set initial state
            }

            return true;
        }

        protected override void RunCommand(string actionParameter)
        {
            if (string.IsNullOrWhiteSpace(actionParameter))
                return;

            var device = _deviceHelper.GetDevice(actionParameter);
            if (_deviceHelper.IsDisabled(device))
                return;

            PluginLog.Info("Setting as default: " + device.FullName);
            if (_isCommunication)
                Task.Run(() => device.SetAsDefaultCommunicationsAsync()).GetAwaiter().OnCompleted(this.setDefaultCompleted);
            else
                Task.Run(() => device.SetAsDefaultAsync()).GetAwaiter().OnCompleted(this.setDefaultCompleted);
         }

        internal void setDefaultCompleted()
        {
            foreach (var deviceKey in this.GetParameters())
            {
                var deviceKeyValue = deviceKey.Value;

                var device = _deviceHelper.GetDevice(deviceKeyValue);
                this.SetCurrentState(deviceKeyValue, this.GetDeviceCommandStateIndex(device));
            }

        }

        internal Int32 GetDeviceCommandStateIndex(CoreAudioDevice device)
        {
 
            if (device is null || _deviceHelper.IsDisabled(device))
                return 2;   // Disabled
            

            if (_deviceHelper.IsActive(device, _isCommunication))
                return 1;   // Selected/default device
         

            return 0;       // Not default (available) device
            
        }

    }

    class SetDefaultPlaybackDevice : SetDefaultDeviceCommand
    {
        public SetDefaultPlaybackDevice()
            : base(false, CoreDeviceType.Playback, "Set Default Playback Device")
        {
            //
        }
    }

    class SetDefaultCommunicationPlaybackDevice : SetDefaultDeviceCommand
    {
        public SetDefaultCommunicationPlaybackDevice()
            : base(true, CoreDeviceType.Playback, "Set Default Playback Communication Device")
        {
            //
        }
    }

    class SetDefaultRecordingDevice : SetDefaultDeviceCommand
    {
        public SetDefaultRecordingDevice()
            : base(false, CoreDeviceType.Capture, "Set Default Recording Device")
        {
            //
        }
    }

    class SetDefaultCommunicationRecordingDevice : SetDefaultDeviceCommand
    {
        public SetDefaultCommunicationRecordingDevice()
            : base(true, CoreDeviceType.Capture, "Set Default Recording Communication Device")
        {
            //
        }
    }
}
