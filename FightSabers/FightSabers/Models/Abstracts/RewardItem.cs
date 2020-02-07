using System;
using FightSabers.Models.Interfaces;

namespace FightSabers.Models.Abstracts
{
    public abstract class RewardItem : IRewardItem
    {
        public enum UnlockType
        {
            Coins,
            Level
        }

        #region Properties
        public string name        { get; set; }
        public string description { get; set; }
        public string unlockHint  { get; set; }
        public bool   unlockState { get; set; }
        public UnlockType unlockType { get; set; }
        public object unlockValue { get; set; }
        public string rewardType  { get; set; }
        #endregion

        #region Methods
        public abstract void UnlockItem();
        public abstract void LockItem();
        #endregion

        protected RewardItem(string name = "", string description = "", string unlockHint = "", UnlockType unlockType = UnlockType.Coins, object unlockValue = null)
        {
            rewardType = GetType().ToString();
            this.name = name;
            this.description = description;
            this.unlockHint = unlockHint;
            this.unlockType = unlockType;
            this.unlockValue = unlockValue;
        }

        public void Enable()
        {
            switch (unlockType)
            {
                case UnlockType.Coins:
                    Logger.log.Debug($"[{name}]: Coins");
                    break;
                case UnlockType.Level:
                    Logger.log.Debug($"[{name}]: Level");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
