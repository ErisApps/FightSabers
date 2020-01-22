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
            if (currentLevelUpCount >= toLevelUpCount)
                Complete();
        }

        #endregion

        protected override void Refresh()
        {
            progressHint = $"{currentLevelUpCount} / {toLevelUpCount}";
        }

        public override void Activate()
        {
            if (!isInitialized || isActivated) return;
            ExperienceSystem.instance.LeveledUp += OnLeveledUp;
            base.Activate();
        }

        public void Prepare(uint currentLevelUpCount = 0, uint toLevelUpCount = 1)
        {
            this.currentLevelUpCount = currentLevelUpCount;
            this.toLevelUpCount = toLevelUpCount;
            base.Prepare($"Raise up", $"Level up <color=#ffa500ff>{toLevelUpCount}</color> level{(toLevelUpCount != 1 ? "s" : "")}!", 
                         $"{currentLevelUpCount} / {toLevelUpCount}",
                         GetType().ToString(), 15 * toLevelUpCount,
                         currentLevelUpCount / (float)toLevelUpCount);
        }
    }
}
