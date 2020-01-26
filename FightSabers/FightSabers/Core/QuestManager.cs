using System;
using System.Collections;
using System.Collections.Generic;
using FightSabers.Models.Abstracts;
using FightSabers.Models.Interfaces;
using FightSabers.Models.Quests;
using FightSabers.Settings;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FightSabers.Core
{
    public class QuestManager : PersistentSingleton<QuestManager>
    {
        #region Properties
        public List<IQuest> CurrentQuests {
            get { return SaveDataManager.instance.SaveData.currentQuests; }
        }

        public List<IQuest> PickableQuests {
            get { return SaveDataManager.instance.SaveData.pickableQuests; }
        }

        public bool CanPickQuest {
            get { return SaveDataManager.instance.SaveData.currentQuests.Count < 3; }
        }

        public Type[] PossibleQuestTypes { get; private set; }
        #endregion

        #region Events
        public delegate void QuestHandler(object     self);
        public delegate void QuestArgsHandler(object self, Quest quest);
        public event QuestHandler     QuestPicked;
        public event QuestArgsHandler QuestCanceled;
        public event QuestArgsHandler QuestCompleted;
        public event QuestArgsHandler QuestProgressChanged;

        private void OnQuestPicked()
        {
            QuestPicked?.Invoke(this);
        }

        private void OnQuestCanceled(object self)
        {
            if (self is Quest quest)
            {
                quest.QuestCanceled -= OnQuestCanceled;
                SaveDataManager.instance.SaveData.currentQuests.Remove(quest);
                SaveDataManager.instance.ApplyToFile();
                QuestCanceled?.Invoke(this, quest);
            }
        }

        private void OnQuestCompleted(object self)
        {
            if (self is Quest quest)
            {
                quest.QuestCompleted -= OnQuestCompleted;
                SaveDataManager.instance.SaveData.currentQuests.Remove(quest);
                SaveDataManager.instance.ApplyToFile();
                QuestCompleted?.Invoke(this, quest);
            }
        }

        private void OnQuestProgressChanged(object self)
        {
            if (self is Quest quest)
                QuestProgressChanged?.Invoke(this, quest);
        }
        #endregion

        #region Unity methods
        private void Awake()
        {
            PossibleQuestTypes = new[] { typeof(LevelUpQuest), typeof(MonsterDamageQuest), typeof(MonsterKillQuest) };
        }
        #endregion

        #region Methods
        public void LoadQuests()
        {
            Random.InitState((int)DateTime.Now.Ticks);
            foreach (var currentQuest in CurrentQuests)
            {
                if (!(currentQuest is Quest quest)) continue;
                quest.QuestCanceled += OnQuestCanceled;
                quest.QuestCompleted += OnQuestCompleted;
                quest.ProgressChanged += OnQuestProgressChanged;
                quest.Activate(true);
            }
            foreach (var pickableQuest in PickableQuests)
            {
                if (!(pickableQuest is Quest quest)) continue;
                quest.ForceInitialize();
                quest.QuestCompleted += OnQuestCompleted;
                quest.ProgressChanged += OnQuestProgressChanged;
            }
        }

        public void AddNewPickableQuest()
        {
            if (PickableQuests.Count >= 3) return;
            var type = PossibleQuestTypes[Random.Range(0, PossibleQuestTypes.Length)];
            AddNewPickableQuest(type);
        }

        public void AddNewPickableQuest(Type type)
        {
            if (PickableQuests.Count >= 3) return;
            if (Activator.CreateInstance(type) is Quest quest)
            {
                switch (quest)
                {
                    case LevelUpQuest levelUpQuest:
                        levelUpQuest.Prepare(0, (uint)Random.Range(1, 4));
                        break;
                    case MonsterDamageQuest monsterDamageQuest:
                        monsterDamageQuest.Prepare(0, (uint)(Random.Range(250, 500) * 100));
                        break;
                    case MonsterKillQuest monsterKillQuest:
                        monsterKillQuest.Prepare(0, (uint)Random.Range(5, 10));
                        break;
                }
                quest.QuestCompleted += OnQuestCompleted;
                quest.ProgressChanged += OnQuestProgressChanged;
                PickableQuests.Add(quest);
                SaveDataManager.instance.ApplyToFile();
            }
        }

        public void LinkGameEventsForActivatedQuests()
        {
            foreach (var currentQuest in CurrentQuests)
                currentQuest.LinkGameEvents();
        }

        public void UnlinkGameEventsForActivatedQuests()
        {
            foreach (var currentQuest in CurrentQuests)
                currentQuest.UnlinkGameEvents();
        }

        #region Quest managing
        public void CancelQuest(int idx)
        {
            if (idx < 0 || CurrentQuests == null || idx >= CurrentQuests.Count || !(CurrentQuests[idx] is IQuest quest)) return;
            CancelQuest(quest);
        }

        public void CancelQuest([NotNull] IQuest quest)
        {
            if (quest == null) throw new ArgumentNullException(nameof(quest));
            var idx = CurrentQuests.FindIndex(q => q == quest);
            if (idx < 0) return;
            quest.Deactivate();
            SaveDataManager.instance.SaveData.currentQuests.Remove(quest);
            SaveDataManager.instance.ApplyToFile();
            OnQuestCanceled(quest);
        }

        public void PickQuest(int idx)
        {
            if (idx < 0 || PickableQuests == null || idx >= PickableQuests.Count || !(PickableQuests[idx] is IQuest quest)) return;
            PickQuest(quest);
        }

        public void PickQuest([NotNull] IQuest quest)
        {
            if (quest == null) throw new ArgumentNullException(nameof(quest));
            var idx = PickableQuests.FindIndex(q => q == quest);
            if (idx < 0) return;
            PickableQuests.Remove(quest);
            quest.Activate();
            SaveDataManager.instance.SaveData.currentQuests.Add(quest);
            SaveDataManager.instance.ApplyToFile();
            OnQuestPicked();
            //new UnityTask(TestComplete(quest, 2f));
        }

        private IEnumerator TestComplete(IQuest quest, float delay)
        {
            yield return new WaitForSeconds(delay);
            quest.Complete();
        }
        #endregion
        #endregion
    }
}