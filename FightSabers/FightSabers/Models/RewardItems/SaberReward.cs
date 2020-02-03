using System.IO;
using FightSabers.Core.ExclusiveContent;
using FightSabers.Models.Abstracts;
using FightSabers.Settings;
using FightSabers.Utilities;

namespace FightSabers.Models.RewardItems
{
    public class SaberReward : RewardItem
    {
        public override void UnlockItem()
        {
            if (unlockState) return;
            FileUtils.WriteResourceToFile($"FightSabers.Rewards.{name}.saber", Path.Combine(ExclusiveSabersManager.SabersAssetPath, $"{name}.saber"));
            unlockState = true;
            SaveDataManager.instance.ApplyToFile();
        }

        public override void LockItem()
        {
            if (!unlockState) return;
            var filePath = Path.Combine(ExclusiveSabersManager.SabersAssetPath, $"{name}.saber");
            if (File.Exists(filePath))
                File.Delete(filePath);
            unlockState = false;
            SaveDataManager.instance.ApplyToFile();
        }
    }
}