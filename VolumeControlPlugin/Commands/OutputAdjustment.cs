using System.Linq;

using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace Loupedeck.VolumeControlPlugin.Commands
{
    class OutputAdjustment : PluginDynamicAdjustment
    {
        private VolumeControlPlugin _plugin;
        
        public OutputAdjustment()
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
            
            var inputs = _plugin.NotificationService.Devices
                .Where(device => device.Value.IsPlaybackDevice)
                .ToArray();

            foreach (var input in inputs)
            {
                this.AddParameter(input.Key, input.Value.FullName, "Outputs");
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

            device.ToggleMute();
            base.ActionImageChanged(actionParameter);
        }

        protected override void ApplyAdjustment(string actionParameter, int ticks)
        {
            if (string.IsNullOrWhiteSpace(actionParameter))
                return;

            var device = GetDevice(actionParameter);
            if (IsDisabled(device))
                return;

            device.Volume += ticks;
            base.ActionImageChanged(actionParameter);
        }

        protected override string GetAdjustmentValue(string actionParameter)
        {
            if (string.IsNullOrWhiteSpace(actionParameter))
                return null;

            var device = GetDevice(actionParameter);
            if (IsDisabled(device))
                return null;

            if (device.IsMuted)
                return "muted";

            return device.Volume.ToString("###'%'");
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
                var path = "Loupedeck.VolumeControlPlugin.Resources.VolumeControl.output-active-50.png";
                if (device.IsMuted)
                    path = "Loupedeck.VolumeControlPlugin.Resources.VolumeControl.output-muted-50.png";
                if (IsDisabled(device))
                    path = "Loupedeck.VolumeControlPlugin.Resources.VolumeControl.output-disabled-50.png";

                var background = EmbeddedResources.ReadImage(path);
                bitmapBuilder.SetBackgroundImage(background);
                bitmapBuilder.DrawText(device.Name, 0, 35, 50, 10, BitmapColor.White, 10, 8);

                return bitmapBuilder.ToImage();
            }
        }

        private bool IsDisabled(CoreAudioDevice device)
        {
            if (device is null)
                return true;

            return device.State != DeviceState.Active;
        }

        private CoreAudioDevice GetDevice(string actionParameter)
        {
            if (string.IsNullOrWhiteSpace(actionParameter))
                return null;

            return _plugin.NotificationService.Devices.TryGetValue(actionParameter, out var device)
                ? device
                : null;
        }
    }
}
