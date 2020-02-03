using System.Collections.Generic;
using System.IO;
using System.Linq;
using IPA.Loader;
using IPA.Utilities;

namespace FightSabers.Core.ExclusiveContent
{
    public class ExclusivePlatformsManager : PersistentSingleton<ExclusivePlatformsManager>
    {
        public static string PlatformsAssetPath      => Path.Combine(BeatSaber.InstallPath, "CustomPlatforms");
        public static bool CustomPlatformInstalled { get; private set; }

        public Dictionary<string, string> ExclusivePlatformData { get; private set; }

        private void Awake()
        {
            CustomPlatformInstalled = PluginManager.AllPlugins.Any(info => info.Metadata.Name == "Custom Platforms");
            ExclusivePlatformData = new Dictionary<string, string> {
                { "Light Disc", "FightSabers.Rewards.Light Disc.plat" }
            };
        }
    }
}