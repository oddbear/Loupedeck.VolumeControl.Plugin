using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi;

namespace Loupedeck.VolumeControlPlugin.Helpers
{
    internal class DeviceHelper
    {
        private readonly VolumeControlPlugin _plugin;

        public DeviceHelper(VolumeControlPlugin plugin)
        {
            _plugin = plugin;
        }

        internal bool IsDisabled(CoreAudioDevice device)
        {
            if (device is null)
                return true;

            return device.State != DeviceState.Active;
        }

        internal bool IsActive(CoreAudioDevice device, bool isCommunication)
        {
            if (device is null)
                return false;
         
            return isCommunication ? device.IsDefaultCommunicationsDevice : device.IsDefaultDevice;

        }

        internal CoreAudioDevice GetDevice(string actionParameter)
        {
            if (string.IsNullOrWhiteSpace(actionParameter))
                return null;

            return _plugin.NotificationService.Devices.TryGetValue(actionParameter, out var device)
                ? device
                : null;
        }
    }
}
