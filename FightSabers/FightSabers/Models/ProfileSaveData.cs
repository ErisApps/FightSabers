﻿using System.Collections.Generic;
using FightSabers.Models.Interfaces;

namespace FightSabers.Models
{
    public class ProfileSaveData
    {
        public uint level = 1;
        public uint currentExp;
        public int fightCoinsAmount;
        public uint skillPointRemaining;
        public uint killMonsterCount;
        public uint flownMonsterCount;
        public List<IQuest> currentQuests = new List<IQuest>();
        public List<IQuest> pickableQuests = new List<IQuest>();
        public List<IRewardItem> rewardableItems = new List<IRewardItem>();
    }
}
