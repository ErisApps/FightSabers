using FightSabers.Models.Interfaces;

namespace FightSabers.Models.Abstracts
{
    public abstract class RewardItem : IRewardItem
    {
        #region Properties
        public string name        { get; set; }
        public string unlockHint  { get; set; }
        public bool   unlockState { get; set; }
        public string rewardType  { get; set; }
        #endregion

        #region Methods
        public abstract void UnlockItem();
        public abstract void LockItem();
        #endregion

        protected RewardItem()
        {
            rewardType = GetType().ToString();
        }
    }
}
