using System.Collections.Generic;
using System.IO;
using System.Linq;
using IPA.Loader;
using IPA.Utilities;

namespace FightSabers.Core.ExclusiveContent
{
    public class ExclusiveWallsManager : PersistentSingleton<ExclusiveWallsManager>
    {
        public static string WallsAssetPath      => Path.Combine(BeatSaber.InstallPath, "CustomWalls");
        public static bool CustomWallInstalled { get; private set; }

        public Dictionary<string, string> ExclusiveWallData { get; private set; }

        private void Awake()
        {
            CustomWallInstalled = PluginManager.AllPlugins.Any(info => info.Metadata.Name == "CustomWalls");
            ExclusiveWallData = new Dictionary<string, string> {
                { "AYAYA", "FightSabers.Rewards.AYAYA.pixie" }
            };
        }
    }
}