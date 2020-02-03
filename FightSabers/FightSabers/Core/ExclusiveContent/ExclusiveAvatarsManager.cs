using System.Collections.Generic;
using System.IO;
using System.Linq;
using IPA.Loader;
using IPA.Utilities;

namespace FightSabers.Core.ExclusiveContent
{
    public class ExclusiveAvatarsManager : PersistentSingleton<ExclusiveAvatarsManager>
    {
        public static string AvatarsAssetPath      => Path.Combine(BeatSaber.InstallPath, "CustomAvatars");
        public static bool CustomAvatarInstalled { get; private set; }

        public Dictionary<string, string> ExclusiveAvatarData { get; private set; }

        private void Awake()
        {
            CustomAvatarInstalled = PluginManager.AllPlugins.Any(info => info.Metadata.Name == "Custom Avatars");
            ExclusiveAvatarData = new Dictionary<string, string> {
                { "Ooka Miko", "Ooka Miko.avatar" }
            };
        }
    }
}