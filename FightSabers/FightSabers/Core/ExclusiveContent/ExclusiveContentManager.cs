﻿using System.Collections.Generic;
using System.Linq;
using FightSabers.Models.Interfaces;
using FightSabers.Models.RewardItems;
using FightSabers.Settings;

namespace FightSabers.Core.ExclusiveContent
{
    public class ExclusiveContentManager : PersistentSingleton<ExclusiveContentManager>
    {
        public List<IRewardItem> BaseRewardItems { get; private set; }
        public bool HasLoadedExclusiveContent { get; private set; }

        private void Awake()
        {
            BaseRewardItems = new List<IRewardItem> {
                new SaberReward { name = "Skeleton Arm", unlockHint = "" },
                new PlatformReward { name = "Light Disc" , unlockHint = "" },
                new AvatarReward { name = "Ooka Miko" , unlockHint = "" },
                new NoteReward { name = "Minecraft" , unlockHint = "" },
                new WallReward { name = "AYAYA" , unlockHint = "" }
            };
            if (SaveDataManager.instance.SaveData.rewardableItems       == null ||
                SaveDataManager.instance.SaveData.rewardableItems.Count != BaseRewardItems.Count)
                InitializeBaseData();
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
