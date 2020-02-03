using System.IO;
using FightSabers.Core.ExclusiveContent;
using FightSabers.Models.Abstracts;
using FightSabers.Settings;
using FightSabers.Utilities;

namespace FightSabers.Models.RewardItems
{
    public class AvatarReward : RewardItem
    {
        public override void UnlockItem()
        {
            if (!ExclusiveContentManager.instance.HasLoadedExclusiveContent || !ExclusiveAvatarsManager.CustomAvatarInstalled || unlockState) return;
            FileUtils.WriteResourceToFile($"FightSabers.Rewards.{name}.avatar", Path.Combine(ExclusiveAvatarsManager.AvatarsAssetPath, $"{name}.avatar"));
            unlockState = true;
            SaveDataManager.instance.ApplyToFile();
        }

        public override void LockItem()
        {
            if (!ExclusiveContentManager.instance.HasLoadedExclusiveContent || !ExclusiveAvatarsManager.CustomAvatarInstalled || !unlockState) return;
            var filePath = Path.Combine(ExclusiveAvatarsManager.AvatarsAssetPath, $"{name}.avatar");
            if (File.Exists(filePath))
                File.Delete(filePath);
            unlockState = false;
            SaveDataManager.instance.ApplyToFile();
        }
    }
}