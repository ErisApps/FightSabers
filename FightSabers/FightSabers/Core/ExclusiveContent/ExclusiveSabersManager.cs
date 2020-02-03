using System.Collections.Generic;
using System.IO;
using System.Linq;
using FightSabers.Models;
using IPA.Loader;
using IPA.Utilities;

namespace FightSabers.Core.ExclusiveContent
{
    public class ExclusiveSabersManager : PersistentSingleton<ExclusiveSabersManager>
    {
        public static string SabersAssetPath      => Path.Combine(BeatSaber.InstallPath, "CustomSabers");
        public static bool CustomSaberInstalled { get; private set; }

        public Dictionary<string, CustomSaberData> ExclusiveSaberData { get; private set; }

        private void Awake()
        {
            CustomSaberInstalled = PluginManager.AllPlugins.Any(info => info.Metadata.Name == "Custom Sabers");
            ExclusiveSaberData = new Dictionary<string, CustomSaberData> {
                { "Skeleton Arm", new CustomSaberData("FightSabers.Rewards.Skeleton Arm.saber") }
            };
        }
    }
}