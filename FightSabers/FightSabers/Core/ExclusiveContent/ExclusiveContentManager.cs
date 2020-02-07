using System.Collections.Generic;
using System.Linq;
using FightSabers.Models.Abstracts;
using FightSabers.Models.Interfaces;
using FightSabers.Models.RewardItems;
using FightSabers.Settings;

namespace FightSabers.Core.ExclusiveContent
{
    public class ExclusiveContentManager : PersistentSingleton<ExclusiveContentManager>
    {
        public List<IRewardItem> BaseRewardItems           { get; private set; }
        public bool              HasLoadedExclusiveContent { get; private set; }

        private void Awake()
        {
            BaseRewardItems = new List<IRewardItem> {
                new SaberReward("Skeleton Arm", "Spooky skeletons, send shivers done your spine!", "Unlock it at the shop!", RewardItem.UnlockType.Coins, 10),
                new PlatformReward("Light Disc", "Disco light!", "Unlock it at the shop!", RewardItem.UnlockType.Coins, 10),
                new AvatarReward("Ooka Miko", "A super cute anime avatar because we're all weebs after all, aren't we?", "Unlock it at the shop!", RewardItem.UnlockType.Coins, 10),
                new NoteReward("Minecraft", "Slice that dirt!", "Unlock it at the shop!", RewardItem.UnlockType.Coins, 10),
                new WallReward("AYAYA", "A-YA-YA!", "Unlock it at the shop!", RewardItem.UnlockType.Coins, 10)
            };
            if (SaveDataManager.instance.SaveData.rewardableItems       == null ||
                SaveDataManager.instance.SaveData.rewardableItems.Count != BaseRewardItems.Count)
                InitializeBaseData();
            if (SaveDataManager.instance.SaveData.rewardableItems != null)
            {
                foreach (var rewardItem in SaveDataManager.instance.SaveData.rewardableItems.OfType<RewardItem>())
                    rewardItem.Enable();
            }
        }

        private void InitializeBaseData()
        {
            if (SaveDataManager.instance.SaveData.rewardableItems == null || SaveDataManager.instance.SaveData.rewardableItems.Count == 0)
                SaveDataManager.instance.SaveData.rewardableItems = BaseRewardItems;
            else
            {
                foreach (var baseRewardItem in BaseRewardItems)
                {
                    if (SaveDataManager.instance.SaveData.rewardableItems.All(item => item.name != baseRewardItem.name))
                        SaveDataManager.instance.SaveData.rewardableItems.Add(baseRewardItem);
                }
            }
            SaveDataManager.instance.ApplyToFile();
        }

        public void LoadExclusiveContent()
        {
            ExclusiveSabersManager.TouchInstance();
            ExclusivePlatformsManager.TouchInstance();
            ExclusiveAvatarsManager.TouchInstance();
            ExclusiveNotesManager.TouchInstance();
            ExclusiveWallsManager.TouchInstance();
            HasLoadedExclusiveContent = true;
        }
    }
}