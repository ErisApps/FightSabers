using System.IO;
using FightSabers.Core.ExclusiveContent;
using FightSabers.Models.Abstracts;
using FightSabers.Settings;
using FightSabers.Utilities;

namespace FightSabers.Models.RewardItems
{
    public class PlatformReward : RewardItem
    {
        public override void UnlockItem()
        {
            if (!ExclusiveContentManager.instance.HasLoadedExclusiveContent || !ExclusivePlatformsManager.CustomPlatformInstalled || unlockState) return;
            FileUtils.WriteResourceToFile($"FightSabers.Rewards.{name}.plat", Path.Combine(ExclusivePlatformsManager.PlatformsAssetPath, $"{name}.plat"));
            unlockState = true;
            SaveDataManager.instance.ApplyToFile();
        }

        public override void LockItem()
        {
            if (!ExclusiveContentManager.instance.HasLoadedExclusiveContent || !ExclusivePlatformsManager.CustomPlatformInstalled || !unlockState) return;
            var filePath = Path.Combine(ExclusivePlatformsManager.PlatformsAssetPath, $"{name}.plat");
            if (File.Exists(filePath))
                File.Delete(filePath);
            unlockState = false;
            SaveDataManager.instance.ApplyToFile();
        }
    }
}