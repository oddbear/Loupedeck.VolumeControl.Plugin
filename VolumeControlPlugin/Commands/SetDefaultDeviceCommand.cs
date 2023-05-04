using System.Linq;
using System.Threading.Tasks;

using CoreDeviceType = AudioSwitcher.AudioApi.DeviceType;
using Loupedeck.VolumeControlPlugin.Helpers;

namespace Loupedeck.VolumeControlPlugin.Commands
{
    abstract class SetDefaultDeviceCommand : PluginDynamicCommand
    {
        private readonly bool _isCommunication;
        private readonly CoreDeviceType _deviceType;
        private readonly string _groupName;

        private DeviceHelper _deviceHelper;

        private VolumeControlPlugin _plugin;

        protected SetDefaultDeviceCommand(bool isCommunication, CoreDeviceType deviceType, string groupName)
        {
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

            if (_isCommunication)
                Task.Run(() => device.SetAsDefaultCommunicationsAsync()).GetAwaiter().GetResult();
            else
                Task.Run(() => device.SetAsDefaultAsync()).GetAwaiter().GetResult();
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
