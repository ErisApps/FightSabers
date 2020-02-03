using System.Collections.Generic;
using System.IO;
using System.Linq;
using FightSabers.Models;
using FightSabers.Models.Interfaces;
using FightSabers.Models.RewardItems;
using FightSabers.Settings;
using IPA.Loader;
using IPA.Utilities;

namespace FightSabers.Core.ExclusiveContent
{
    public class ExclusiveSabersManager : PersistentSingleton<ExclusiveSabersManager>
    {
        public static string SabersAssetPath      => Path.Combine(BeatSaber.InstallPath, "CustomSabers");
        public static bool   HasFoundCustomSabers { get; private set; }
        public static bool CustomSaberInstalled { get; private set; }

        public Dictionary<string, CustomSaberData> ExclusiveSaberData { get; private set; }

        private List<IRewardItem> _baseRewardItems;

        private void Awake()
        {
            CustomSaberInstalled = PluginManager.AllPlugins.Any(info => info.Metadata.Name == "Custom Sabers");
            if (!CustomSaberInstalled) return;
            _baseRewardItems = new List<IRewardItem> {
                new SaberReward { name = "Skeleton Arm", rewardType = "FightSabers.Models.RewardItems.SaberReward", unlockHint = "Reach level 5!", unlockState = false }
            };
            if (SaveDataManager.instance.SaveData.rewardableItems == null ||
                SaveDataManager.instance.SaveData.rewardableItems.Count != _baseRewardItems.Count)
                InitializeSabersData();
            ExclusiveSaberData = new Dictionary<string, CustomSaberData> {
                { "Skeleton Arm", new CustomSaberData("FightSabers.Rewards.Skeleton Arm.saber") }
            };
        }

        private void InitializeSabersData()
        {
            if (SaveDataManager.instance.SaveData.rewardableItems == null || SaveDataManager.instance.SaveData.rewardableItems.Count == 0)
                SaveDataManager.instance.SaveData.rewardableItems = _baseRewardItems;
            else
            {
                foreach (var baseRewardItem in _baseRewardItems)
                {
                    if (SaveDataManager.instance.SaveData.rewardableItems.All(item => item.name != baseRewardItem.name))
                        SaveDataManager.instance.SaveData.rewardableItems.Add(baseRewardItem);
                }
            }
            SaveDataManager.instance.ApplyToFile();
        }

        public void LoadUnlockableSabers()
        {
            if (!CustomSaberInstalled || !Directory.Exists(SabersAssetPath)) return; // Most likely doesn't have CustomSabers
            HasFoundCustomSabers = true;
        }
    }
}