using System.Collections.Generic;
using FightSabers.Models.Interfaces;
using FightSabers.Models.Quests;
using FightSabers.Settings;
using UnityEngine;

namespace FightSabers.Core
{
    public class QuestManager : PersistentSingleton<QuestManager>
    {
        public List<IQuest> CurrentQuests { get; private set; }
        public List<IQuest> PickableQuests { get; private set; }

        private void Awake()
        {
            CurrentQuests = new List<IQuest>();
            PickableQuests = new List<IQuest>();
        }

        public void LoadQuests()
        {
            //TODO: Load stored quests from a file
            for (var i = 0; i < 3; ++i)
            {
                var quest = new LevelUpQuest();
                quest.Prepare(0, (uint)Random.Range(1, 4));
                PickableQuests.Add(quest);
            }
        }

        public void PickQuest(int idx)
        {
            if (idx <= 0 || PickableQuests == null || idx >= PickableQuests.Count || !(PickableQuests[idx] is IQuest quest)) return;
            PickQuest(quest);
        }

        public void PickQuest(IQuest quest)
        {
            if (quest == null) return;
            CurrentQuests.Add(quest);
            PickableQuests.Remove(quest);
            quest.Activate();
            SaveDataManager.instance.SaveData.currentQuests.Add(quest);
            SaveDataManager.instance.ApplyToFile();
        }
    }
}
