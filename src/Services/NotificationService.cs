using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace Loupedeck.VolumeControlPlugin.Services
{
    public class NotificationService : MarshalByRefObject
    {
        public readonly Dictionary<string, CoreAudioDevice> Devices = new Dictionary<string, CoreAudioDevice>();

        public NotificationService()
        {
            var controller = new CoreAudioController();

            var devices = controller.GetDevices(AudioSwitcher.AudioApi.DeviceType.All, DeviceState.All);

            foreach (var device in devices)
            {
                try
                {
                    if (device.Name == "Unknown")
                        continue;

                    Devices[device.RealId] = device;
                }
                catch
                {
                    //Ignore failing devices.
                }
            }
        }
    }
}
