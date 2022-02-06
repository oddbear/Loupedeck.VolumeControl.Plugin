using System.Linq;

using NAudio.CoreAudioApi;

namespace Loupedeck.VolumeControlPlugin.Commands
{
    class InputAdjustment : PluginDynamicAdjustment
    {
        private VolumeControlPlugin _plugin;
        
        public InputAdjustment()
            : base(true)
        {
            //
        }

        protected override bool OnLoad()
        {
            _plugin = base.Plugin as VolumeControlPlugin;

            if (_plugin is null)
                return false;

            if (_plugin.NotificationService is null)
                return false;
            
            var inputs = _plugin.NotificationService.GetDevices(DataFlow.Capture);

            foreach (var input in inputs)
            {
                //Friendly name can be custom name:
                //FriendlyName: Playback (InterfaceName)
                //DeviceFriendlyName: InterfaceName
                //ID: {0.0.0.00000000}.{00000000-0000-0000-0000-000123456789}
                this.AddParameter(input.ID, input.DeviceFriendlyName, "Inputs");
            }

            return true;
        }

        protected override void RunCommand(string actionParameter)
        {
            if (string.IsNullOrWhiteSpace(actionParameter))//Always null issue.
                return;

            var device = GetDevice(actionParameter);
            if (IsDisabled(device))
                return;

            device.AudioEndpointVolume.Mute = device.AudioEndpointVolume.Mute;
            base.ActionImageChanged(actionParameter);
        }

        protected override void ApplyAdjustment(string actionParameter, int ticks)
        {
            if (string.IsNullOrWhiteSpace(actionParameter))
                return;

            var device = GetDevice(actionParameter);
            if (IsDisabled(device))
                return;

            device.AudioEndpointVolume.MasterVolumeLevelScalar += ticks;
            base.ActionImageChanged(actionParameter);
        }

        protected override string GetAdjustmentValue(string actionParameter)
        {
            if (string.IsNullOrWhiteSpace(actionParameter))
                return null;

            var device = GetDevice(actionParameter);
            if (IsDisabled(device))
                return null;

            return device.AudioEndpointVolume.MasterVolumeLevelScalar.ToString();
        }
        
        protected override BitmapImage GetCommandImage(string actionParameter, PluginImageSize imageSize)
        {
            if (string.IsNullOrWhiteSpace(actionParameter))
                return null;

            var device = GetDevice(actionParameter);
            if (device is null)
                return null;
            
            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                var path = "Loupedeck.VolumeControlPlugin.Resources.VolumeControl.input-active-50.png";
                if (device.AudioEndpointVolume.Mute)
                    path = "Loupedeck.VolumeControlPlugin.Resources.VolumeControl.input-muted-50.png";
                if (IsDisabled(device))
                    path = "Loupedeck.VolumeControlPlugin.Resources.VolumeControl.input-disabled-50.png";

                var background = EmbeddedResources.ReadImage(path);
                bitmapBuilder.Translate(6, 0);
                bitmapBuilder.Scale(0.8f, 0.8f);
                bitmapBuilder.SetBackgroundImage(background);
                bitmapBuilder.Scale(1.25f, 1.25f);
                bitmapBuilder.Translate(-6, 0);

                bitmapBuilder.Translate(0, 18);
                bitmapBuilder.DrawText(device.DeviceFriendlyName, BitmapColor.White, 10); //TODO: Name and alignment.

                return bitmapBuilder.ToImage();
            }
        }

        private bool IsDisabled(MMDevice device)
        {
            if (device is null)
                return true;

            if (device.InstanceId == "Unknown")
                return true;

            return device.State != DeviceState.Active;
        }

        private MMDevice GetDevice(string actionParameter)
        {
            if (string.IsNullOrWhiteSpace(actionParameter))
                return null;

            return _plugin.NotificationService.GetDevice(actionParameter);
        }
    }
}
