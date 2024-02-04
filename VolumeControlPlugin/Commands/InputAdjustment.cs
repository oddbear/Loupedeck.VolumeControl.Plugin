using System.Linq;
using System.Threading.Tasks;

using Loupedeck.VolumeControlPlugin.Helpers;

namespace Loupedeck.VolumeControlPlugin.Commands
{
    class InputAdjustment : PluginDynamicAdjustment
    {
        private VolumeControlPlugin _plugin;
        private DeviceHelper _deviceHelper;
        
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

            _deviceHelper = new DeviceHelper(_plugin);

            if (_plugin.NotificationService is null)
                return false;
            
            var inputs = _plugin.NotificationService.Devices
                .Where(device => device.Value.IsCaptureDevice)
                .ToArray();

            foreach (var input in inputs)
            {
                this.AddParameter(input.Key, input.Value.FullName, "Adjust Volume Inputs");
            }

            return true;
        }

        protected override void RunCommand(string actionParameter)
        {
            if (string.IsNullOrWhiteSpace(actionParameter))//Always null issue.
                return;

            var device = _deviceHelper.GetDevice(actionParameter);
            if (_deviceHelper.IsDisabled(device))
{ 
                //Update image in case the device was disconnected/disabled
                base.ActionImageChanged(actionParameter);
                return;
}

            Task.Run(() => device.SetMuteAsync(!device.IsMuted)).GetAwaiter().GetResult();
            base.ActionImageChanged(actionParameter);
        }

        protected override void ApplyAdjustment(string actionParameter, int ticks)
        {
            if (string.IsNullOrWhiteSpace(actionParameter))
                return;

            var device = _deviceHelper.GetDevice(actionParameter);
            if (_deviceHelper.IsDisabled(device))
{
                //Update image in case the device was disconnected/disabled
                base.ActionImageChanged(actionParameter);
                return;
}
                
            
            var volume = device.Volume + ticks;
            if (volume > 100)
                volume = 100;
            if (volume < 0)
                volume = 0;

            Task.Run(() => device.SetVolumeAsync(volume)).GetAwaiter().GetResult();
            base.ActionImageChanged(actionParameter);
        }

        protected override string GetAdjustmentValue(string actionParameter)
        {
            if (string.IsNullOrWhiteSpace(actionParameter))
                return null;
            
            var device = _deviceHelper.GetDevice(actionParameter);
            if (_deviceHelper.IsDisabled(device))
                return "disabled";

            if (device.IsMuted)
                return "muted";

            return device.Volume.ToString("###'%'");
        }
        
        protected override BitmapImage GetCommandImage(string actionParameter, PluginImageSize imageSize)
        {
            if (string.IsNullOrWhiteSpace(actionParameter))
                return null;

            var device = _deviceHelper.GetDevice(actionParameter);
            if (device is null)
                return null;
            
            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                var path = "Loupedeck.VolumeControlPlugin.Resources.VolumeControl.input-active-50.png";
                if (device.IsMuted)
                    path = "Loupedeck.VolumeControlPlugin.Resources.VolumeControl.input-muted-50.png";
                if (_deviceHelper.IsDisabled(device))
                    path = "Loupedeck.VolumeControlPlugin.Resources.VolumeControl.input-disabled-50.png";

                var background = EmbeddedResources.ReadImage(path);
                bitmapBuilder.SetBackgroundImage(background);
                bitmapBuilder.DrawText(device.Name, 0, 35, 50, 10, BitmapColor.White, 10, 8);

                return bitmapBuilder.ToImage();
            }
        }
    }
}
