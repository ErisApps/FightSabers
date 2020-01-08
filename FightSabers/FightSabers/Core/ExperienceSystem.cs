using System;
using System.Collections;
using FightSabers.Settings;
using FightSabers.UI;
using FightSabers.Utilities;
using UnityEngine;

namespace FightSabers.Core
{
    public class ExperienceSystem : PersistentSingleton<ExperienceSystem>
    {
        #region Properties

        public uint  ExperiencePointsWon          { get; private set; }
        public float ExperienceMultiplier         { get; private set; } = 1f;
        public uint  TotalNeededExperienceForNextLevel { get; private set; }

        #endregion

        #region Events

        public delegate void ExperienceHandler(object self);
        public event ExperienceHandler LeveledUp;
        public event ExperienceHandler ApplyExperienceFinished;

        private void OnLeveledUp()
        {
            LeveledUp?.Invoke(this);
        }

        private void OnApplyExperienceFinished()
        {
            ApplyExperienceFinished?.Invoke(this);
        }

        #endregion

        #region Methods

        public void Setup()
        {
            RefreshTotalNeededExperience();
        }

        public void RefreshTotalNeededExperience()
        {
            TotalNeededExperienceForNextLevel = GetNeededExperience(SaveDataManager.instance.SaveData.level + 1) - GetNeededExperience(SaveDataManager.instance.SaveData.level);
        }

        public uint GetExperienceBeforeLevelUp()
        {
            return TotalNeededExperienceForNextLevel - SaveDataManager.instance.SaveData.currentExp;
        }

        public uint GetNeededExperience(uint level)
        {
            return (uint)((level + 25) * Math.Pow(14, Math.Log(Math.Sqrt(level))) / 2);
        }

        public float GetPercentageForExperience(uint exp)
        {
            return (exp - 0f) / (TotalNeededExperienceForNextLevel - 0f);
        }

        public float GetPercentageForExperiencePrecise(float exp)
        {
            return (exp - 0f) / (TotalNeededExperienceForNextLevel - 0f);
        }

        public float GetCurrentExperiencePercentage()
        {
            return GetPercentageForExperience(SaveDataManager.instance.SaveData.currentExp);
        }

        public void AddFightExperience(uint exp)
        {
            ExperiencePointsWon += (uint)(exp * ExperienceMultiplier);
        }

        public IEnumerator ApplyExperience()
        {
            var delayApplied = 2.5f;
            while (ExperiencePointsWon > 0)
            {
                if (ExperiencePointsWon >= GetExperienceBeforeLevelUp())
                {
                    new UnityTask(FightSabersProgress.instance.FillExperienceBar(SaveDataManager.instance.SaveData.currentExp, TotalNeededExperienceForNextLevel, delayApplied));
                    yield return new WaitUntil(() => !FightSabersProgress.instance.CurrentlyAnimated);
                    ExperiencePointsWon -= GetExperienceBeforeLevelUp();
                    LevelUp();
                    yield return new WaitUntil(() => !FightSabersProgress.instance.CurrentlyAnimated);
                    delayApplied = 0;
                }
                else
                {
                    new UnityTask(FightSabersProgress.instance.FillExperienceBar(SaveDataManager.instance.SaveData.currentExp, SaveDataManager.instance.SaveData.currentExp + ExperiencePointsWon, delayApplied));
                    yield return new WaitUntil(() => !FightSabersProgress.instance.CurrentlyAnimated);
                    SaveDataManager.instance.SaveData.currentExp += ExperiencePointsWon;
                    ExperiencePointsWon = 0;
                }
            }
            OnApplyExperienceFinished();
        }

        public void LevelUp()
        {
            SaveDataManager.instance.SaveData.level += 1;
            SaveDataManager.instance.SaveData.currentExp = 0;
            SaveDataManager.instance.SaveData.skillPointRemaining += 1;
            RefreshTotalNeededExperience();
            OnLeveledUp();
        }

        private void TestLevel() //TODO: Remove later, FPFC testing
        {
            ExperienceSystem.instance.AddFightExperience(1155);
            new UnityTask(ExperienceSystem.instance.ApplyExperience());
        }

        #endregion
    }
}
