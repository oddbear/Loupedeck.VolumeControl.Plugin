namespace Loupedeck.VolumeControlPlugin
{
    using System;

    using Services;

    // This class contains the plugin-level logic of the Loupedeck plugin.

    public class VolumeControlPlugin : Plugin
    {
        public NotificationService NotificationService = new NotificationService();

        // Gets a value indicating whether this is an API-only plugin.
        public override Boolean UsesApplicationApiOnly => true;

        // Gets a value indicating whether this is a Universal plugin or an Application plugin.
        public override Boolean HasNoApplication => true;

        // Initializes a new instance of the plugin class.
        public VolumeControlPlugin()
        {
            // Initialize the plugin log.
            PluginLog.Init(this.Log);

            // Initialize the plugin resources.
            PluginResources.Init(this.Assembly);
        }

        // This method is called when the plugin is loaded.
        public override void Load()
        {
            LoadPluginIcons();
        }

        // This method is called when the plugin is unloaded.
        public override void Unload()
        {
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
