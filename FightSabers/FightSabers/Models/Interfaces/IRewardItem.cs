﻿using FightSabers.Models.Abstracts;

namespace FightSabers.Models.Interfaces
{
    public interface IRewardItem
    {
        #region Properties

        /// <summary>
        /// Name of the rewardable item
        /// </summary>
        string name { get; set; }

        /// <summary>
        /// Description of the rewardable item
        /// </summary>
        string description { get; set; }

        /// <summary>
        /// Description how to unlock the item
        /// </summary>
        string unlockHint { get; set; }

        /// <summary>
        /// Does the item has been rewarded?
        /// </summary>
        bool unlockState { get; set; }

        /// <summary>
        /// Unlock type of the reward
        /// </summary>
        RewardItem.UnlockType unlockType { get; set; }

        /// <summary>
        /// Unlock value needed for the unlock type
        /// </summary>
        object unlockValue { get; set; }

        /// <summary>
        /// Type of the reward
        /// </summary>
        string rewardType { get; set; }

        #endregion
    }
}