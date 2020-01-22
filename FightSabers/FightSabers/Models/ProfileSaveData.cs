using System.Collections.Generic;
using FightSabers.Models.Interfaces;

namespace FightSabers.Models
{
    public class ProfileSaveData
    {
        public uint level = 1;
        public uint currentExp;
        public uint skillPointRemaining;
        public uint killMonsterCount;
        public uint flownMonsterCount;
        public List<IQuest> currentQuests = new List<IQuest>();
    }
}
