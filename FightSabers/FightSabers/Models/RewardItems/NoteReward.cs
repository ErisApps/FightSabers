using System.IO;
using FightSabers.Core.ExclusiveContent;
using FightSabers.Models.Abstracts;
using FightSabers.Settings;
using FightSabers.Utilities;

namespace FightSabers.Models.RewardItems
{
    public class NoteReward : RewardItem
    {
        public override void UnlockItem()
        {
            if (unlockState) return;
            FileUtils.WriteResourceToFile($"FightSabers.Rewards.{name}.bloq", Path.Combine(ExclusiveNotesManager.NotesAssetPath, $"{name}.bloq"));
            unlockState = true;
            SaveDataManager.instance.ApplyToFile();
        }

        public override void LockItem()
        {
            if (!unlockState) return;
            var filePath = Path.Combine(ExclusiveNotesManager.NotesAssetPath, $"{name}.bloq");
            if (File.Exists(filePath))
                File.Delete(filePath);
            unlockState = false;
            SaveDataManager.instance.ApplyToFile();
        }
    }
}