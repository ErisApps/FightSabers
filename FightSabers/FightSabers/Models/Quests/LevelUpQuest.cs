using System;
using FightSabers.Core;
using FightSabers.Models.Abstracts;

namespace FightSabers.Models.Quests
{
    public class LevelUpQuest : Quest
    {
        #region Properties

        public uint currentLevelUpCount;
        public uint toLevelUpCount;

        #endregion

        #region Events

        private void OnLeveledUp(object self)
        {
            currentLevelUpCount += 1;
            Progress = currentLevelUpCount / (float)toLevelUpCount;
            if (currentLevelUpCount >= toLevelUpCount || Math.Abs(Progress - 1) < 0.001f) //Double security here
                Complete();
        }

        #endregion

        protected override void Refresh()
        {
            progressHint = $"{currentLevelUpCount} / {toLevelUpCount}";
        }

        public override void Activate(bool forceInitialize = false)
        {
            if (!isInitialized && forceInitialize)
                isInitialized = true;
            if (!isInitialized || isActivated) return;
            Logger.log.Debug(">>> Specific quest activated!");
            ExperienceSystem.instance.LeveledUp += OnLeveledUp;
        }

        public override void Deactivate()
        {
            if (!isInitialized || !isActivated) return;
            ExperienceSystem.instance.LeveledUp -= OnLeveledUp;
            base.Deactivate();
        }

        public override void Complete()
        {
            ExperienceSystem.instance.LeveledUp -= OnLeveledUp;
            base.Complete();
            ExperienceSystem.instance.ApplyExperience();
        }

        public void Prepare(uint currentLevelUpCount = 0, uint toLevelUpCount = 1)
        {
            this.currentLevelUpCount = currentLevelUpCount;
            this.toLevelUpCount = toLevelUpCount;
            base.Prepare("Raise up", $"Level up <color=#ffa500ff>{toLevelUpCount}</color> level{(toLevelUpCount != 1 ? "s" : "")}!", 
                         $"{currentLevelUpCount} / {toLevelUpCount}",
                         GetType().ToString(), 15 * toLevelUpCount,
                         currentLevelUpCount / (float)toLevelUpCount);
        }
    }
}
