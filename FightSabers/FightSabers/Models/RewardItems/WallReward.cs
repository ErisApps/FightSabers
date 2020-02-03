using System.IO;
using FightSabers.Core.ExclusiveContent;
using FightSabers.Models.Abstracts;
using FightSabers.Settings;
using FightSabers.Utilities;

namespace FightSabers.Models.RewardItems
{
    public class WallReward : RewardItem
    {
        public override void UnlockItem()
        {
            if (!ExclusiveContentManager.instance.HasLoadedExclusiveContent || !ExclusiveWallsManager.CustomWallInstalled || unlockState) return;
            FileUtils.WriteResourceToFile($"FightSabers.Rewards.{name}.pixie", Path.Combine(ExclusiveWallsManager.WallsAssetPath, $"{name}.pixie"));
            unlockState = true;
            SaveDataManager.instance.ApplyToFile();
        }

        public override void LockItem()
        {
            if (!ExclusiveContentManager.instance.HasLoadedExclusiveContent || !ExclusiveWallsManager.CustomWallInstalled || !unlockState) return;
            var filePath = Path.Combine(ExclusiveWallsManager.WallsAssetPath, $"{name}.pixie");
            if (File.Exists(filePath))
                File.Delete(filePath);
            unlockState = false;
            SaveDataManager.instance.ApplyToFile();
        }
    }
}