using Loupedeck.VolumeControlPlugin.Services;

namespace Loupedeck.VolumeControlPlugin
{
    public class VolumeControlPlugin : Plugin
    {
        public NotificationService NotificationService = new NotificationService();

        public override bool HasNoApplication => true;
        public override bool UsesApplicationApiOnly => true;

        public override void Load()
        {
            LoadPluginIcons();
        }
        
        public override void RunCommand(string commandName, string parameter)
        {
            //
        }

        public override void ApplyAdjustment(string adjustmentName, string parameter, int diff)
        {
            //
        }

        private void LoadPluginIcons()
        {
            //var resources = this.Assembly.GetManifestResourceNames();
            Info.Icon16x16 = EmbeddedResources.ReadImage("Loupedeck.VolumeControlPlugin.Resources.Icons.icon-16.png");
            Info.Icon32x32 = EmbeddedResources.ReadImage("Loupedeck.VolumeControlPlugin.Resources.Icons.icon-32.png");
            Info.Icon48x48 = EmbeddedResources.ReadImage("Loupedeck.VolumeControlPlugin.Resources.Icons.icon-48.png");
            Info.Icon256x256 = EmbeddedResources.ReadImage("Loupedeck.VolumeControlPlugin.Resources.Icons.icon-256.png");
        }
    }
}
