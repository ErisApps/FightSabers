using System;
using FightSabers.Core;
using FightSabers.Models.Abstracts;

namespace FightSabers.Models.Quests
{
    public class MonsterKillQuest : Quest
    {
        #region Properties

        public uint currentKillCount;
        public uint toKillCount;

        #endregion

        #region Events

        private void OnMonsterRemoved(object self, MonsterStatus status)
        {
            if (status != MonsterStatus.Killed) return;
            currentKillCount += 1;
            Progress = currentKillCount / (float)toKillCount;
            if (currentKillCount > toKillCount || Math.Abs(Progress - 1) < 0.001f) //Double security here
                Complete();
        }

        #endregion

        #region Methods

        protected override void Refresh()
        {
            progressHint = $"{currentKillCount} / {toKillCount}";
        }

        public override void Complete()
        {
            UnlinkGameEvents();
            base.Complete();
        }

        public override void LinkGameEvents()
        {
            if (isCompleted && !hasGameEventsActivated) return;
            MonsterGenerator.instance.MonsterRemoved += OnMonsterRemoved;
            base.LinkGameEvents();
        }

        public override void UnlinkGameEvents()
        {
            if (isCompleted && hasGameEventsActivated) return;
            MonsterGenerator.instance.MonsterRemoved -= OnMonsterRemoved;
            base.UnlinkGameEvents();
        }

        public void Prepare(uint currentKillCount = 0, uint toKillCount = 10)
        {
            this.currentKillCount = currentKillCount;
            this.toKillCount = toKillCount;
            base.Prepare("Demon slayer", $"Kill a total of <color=#ffa500ff>{toKillCount}</color> monsters!",
                         $"{currentKillCount} / {toKillCount}",
                         GetType().ToString(), toKillCount * 6,
                         currentKillCount                  / (float)toKillCount);
        }

        #endregion
    }
}
