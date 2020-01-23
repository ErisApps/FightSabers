using System.Collections;
using System.Collections.Generic;
using FightSabers.Models.Abstracts;
using FightSabers.Models.Interfaces;
using FightSabers.Models.Quests;
using FightSabers.Settings;
using FightSabers.Utilities;
using UnityEngine;

namespace FightSabers.Core
{
    public class QuestManager : PersistentSingleton<QuestManager>
    {
        #region Properties
        public List<IQuest> CurrentQuests {
            get { return SaveDataManager.instance.SaveData.currentQuests; }
        }

        public List<IQuest> PickableQuests { get; private set; }

        public bool CanPickQuest {
            get { return SaveDataManager.instance.SaveData.currentQuests.Count < 3; }
        }
        #endregion

        #region Events

        public delegate void QuestHandler(object self);
        public delegate void QuestArgsHandler(object self, Quest quest);
        public event QuestHandler QuestPicked;
        public event QuestArgsHandler QuestCompleted;

        private void OnQuestPicked()
        {
            QuestPicked?.Invoke(this);
        }

        private void OnQuestCompleted(Quest quest)
        {
            QuestCompleted?.Invoke(this, quest);
        }

        #endregion

        #region Unity methods
        private void Awake()
        {
            PickableQuests = new List<IQuest>();
        }
        #endregion

        #region Methods
        public void LoadQuests()
        {
            //TODO: Load stored quests from a file
            for (var i = 0; i < 3; ++i)
            {
                var quest = new LevelUpQuest();
                quest.Prepare(0, (uint)Random.Range(1, 4));
                quest.QuestCompleted += OnQuestCompleted;
                PickableQuests.Add(quest);
            }
        }

        private void OnQuestCompleted(object self)
        {
            if (self is Quest quest)
            {
                quest.QuestCompleted -= OnQuestCompleted;
                SaveDataManager.instance.SaveData.currentQuests.Remove(quest);
                SaveDataManager.instance.ApplyToFile();
                OnQuestCompleted(quest);
            }
        }

        public void PickQuest(int idx)
        {
            if (idx < 0 || PickableQuests == null || idx >= PickableQuests.Count || !(PickableQuests[idx] is IQuest quest)) return;
            PickQuest(quest);
        }

        public void PickQuest(IQuest quest)
        {
            if (quest == null) return;
            //PickableQuests.Remove(quest);
            var idx = PickableQuests.FindIndex(q => q == quest);
            Logger.log.Debug(idx.ToString());
            if (idx >= 0)
            {
                PickableQuests[idx] = null;
                quest.Activate();
                SaveDataManager.instance.SaveData.currentQuests.Add(quest);
                SaveDataManager.instance.ApplyToFile();
                OnQuestPicked();
                //new UnityTask(TestComplete(quest, 2f));
            }
        }

        private IEnumerator TestComplete(IQuest quest, float delay)
        {
            yield return new WaitForSeconds(delay);
            quest.Complete();
        }
        #endregion
    }
}