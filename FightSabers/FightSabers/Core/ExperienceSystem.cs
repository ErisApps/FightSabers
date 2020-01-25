using System;
using System.Collections;
using FightSabers.Settings;
using FightSabers.UI;
using FightSabers.UI.Controllers;
using FightSabers.Utilities;
using UnityEngine;

namespace FightSabers.Core
{
    public class ExperienceSystem : PersistentSingleton<ExperienceSystem>
    {
        #region Properties
        public uint  ExperiencePointsWon               { get; private set; }
        public float ExperienceMultiplier              { get; private set; } = 1f;
        public uint  TotalNeededExperienceForNextLevel { get; private set; }
        public bool  IsApplyingExperience              { get; private set; }

        public bool HasOverflowedExperience {
            get { return SaveDataManager.instance.SaveData.currentExp >= TotalNeededExperienceForNextLevel; }
        }
        #endregion

        #region Events
        public delegate void ExperienceHandler(object self);
        public event ExperienceHandler LeveledUp;
        public event ExperienceHandler ApplyExperienceStarted;
        public event ExperienceHandler ApplyExperienceFinished;

        private void OnLeveledUp()
        {
            LeveledUp?.Invoke(this);
        }

        private void OnApplyExperienceStarted()
        {
            IsApplyingExperience = true;
            ApplyExperienceStarted?.Invoke(this);
        }

        private void OnApplyExperienceFinished()
        {
            IsApplyingExperience = false;
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

        public void FixOverflowedExperience()
        {
            if (!HasOverflowedExperience) return;
            while (HasOverflowedExperience)
            {
                SaveDataManager.instance.SaveData.level += 1;
                SaveDataManager.instance.SaveData.currentExp -= TotalNeededExperienceForNextLevel;
                SaveDataManager.instance.SaveData.skillPointRemaining += 1;
                RefreshTotalNeededExperience();
            }
            SaveDataManager.instance.ApplyToFile();
        }

        public void ApplyExperience(float delayApplied = 2.5f)
        {
            if (IsApplyingExperience) return;
            OnApplyExperienceStarted();
            new UnityTask(ApplyExperienceInternal(delayApplied));
        }

        private IEnumerator ApplyExperienceInternal(float delayApplied = 2.5f)
        {
            while (ExperiencePointsWon > 0)
            {
                Logger.log.Debug($"ExperiencePointsWon: {ExperiencePointsWon}");
                if (ExperiencePointsWon >= GetExperienceBeforeLevelUp())
                {
                    new UnityTask(OverlayViewController.instance.FillExperienceBar(SaveDataManager.instance.SaveData.currentExp, TotalNeededExperienceForNextLevel, delayApplied));
                    yield return new WaitUntil(() => !OverlayViewController.instance.CurrentlyAnimated);
                    ExperiencePointsWon -= GetExperienceBeforeLevelUp();
                    LevelUp();
                    yield return new WaitUntil(() => !OverlayViewController.instance.CurrentlyAnimated);
                    delayApplied = 0;
                }
                else
                {
                    var addingExperience = ExperiencePointsWon;
                    new UnityTask(OverlayViewController.instance.FillExperienceBar(SaveDataManager.instance.SaveData.currentExp, SaveDataManager.instance.SaveData.currentExp + addingExperience, delayApplied));
                    yield return new WaitUntil(() => !OverlayViewController.instance.CurrentlyAnimated);
                    SaveDataManager.instance.SaveData.currentExp += addingExperience;
                    ExperiencePointsWon -= addingExperience;
                }
            }
            OnApplyExperienceFinished();
            for (var i = 0; HasOverflowedExperience && i < 5; ++i)
                Logger.log.Error("This should never happen but the experience system is broken for this session. Please DM me if you're seeing this!");
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
            AddFightExperience(1155);
            ApplyExperience();
        }
        #endregion
    }
}