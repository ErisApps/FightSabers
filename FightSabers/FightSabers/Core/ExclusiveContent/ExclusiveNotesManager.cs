using System.Collections.Generic;
using System.IO;
using System.Linq;
using IPA.Loader;
using IPA.Utilities;

namespace FightSabers.Core.ExclusiveContent
{
    public class ExclusiveNotesManager : PersistentSingleton<ExclusiveNotesManager>
    {
        public static string NotesAssetPath      => Path.Combine(BeatSaber.InstallPath, "CustomNotes");
        public static bool CustomNoteInstalled { get; private set; }

        public Dictionary<string, string> ExclusiveNoteData { get; private set; }

        private void Awake()
        {
            CustomNoteInstalled = PluginManager.AllPlugins.Any(info => info.Metadata.Name == "CustomNotes");
            ExclusiveNoteData = new Dictionary<string, string> {
                { "Minecraft", "FightSabers.Rewards.Minecraft.bloq" }
            };
        }
    }
}