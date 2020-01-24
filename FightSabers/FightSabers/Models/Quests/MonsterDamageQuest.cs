using System;
using FightSabers.Core;
using FightSabers.Models.Abstracts;

namespace FightSabers.Models.Quests
{
    public class MonsterDamageQuest : Quest
    {
        #region Properties

        public uint currentDamageCount;
        public uint toDamageCount;

        #endregion

        #region Events

        private void OnMonsterHurt(object self, int damage)
        {
            currentDamageCount += (uint)damage;
            Progress = currentDamageCount / (float)toDamageCount;
            if (currentDamageCount > toDamageCount || Math.Abs(Progress - 1) < 0.001f) //Double security here
                Complete();
        }

        #endregion

        #region Methods

        protected override void Refresh()
        {
            progressHint = $"{currentDamageCount} / {toDamageCount}";
        }

        public override void Complete()
        {
            UnlinkGameEvents();
            base.Complete();
        }

        public override void LinkGameEvents()
        {
            if (isCompleted && !hasGameEventsActivated) return;
            MonsterGenerator.instance.MonsterHurt += OnMonsterHurt;
            base.LinkGameEvents();
        }

        public override void UnlinkGameEvents()
        {
            if (isCompleted && hasGameEventsActivated) return;
            MonsterGenerator.instance.MonsterHurt -= OnMonsterHurt;
            base.UnlinkGameEvents();
        }

        public void Prepare(uint currentDamageCount = 0, uint toDamageCount = 35000)
        {
            this.currentDamageCount = currentDamageCount;
            this.toDamageCount = toDamageCount;
            base.Prepare("Frenzy slicer", $"Make a total of <color=#ffa500ff>{toDamageCount}</color> damage to a monster!",
                         $"{currentDamageCount} / {toDamageCount}",
                         GetType().ToString(), toDamageCount / 1000,
                         currentDamageCount / (float)toDamageCount);
        }

        #endregion
    }
}
