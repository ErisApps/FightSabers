using System;
using System.Collections;
using System.Collections.Generic;
using FightSabers.Models.Abstracts;
using FightSabers.Models.Interfaces;
using FightSabers.Models.Quests;
using FightSabers.Settings;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace FightSabers.Core
{
	internal class QuestManager : IInitializable
	{
		private readonly SaveDataManager _saveDataManager;
		private readonly DiContainer _diContainer;

		public QuestManager(SaveDataManager saveDataManager, DiContainer diContainer)
		{
			_saveDataManager = saveDataManager;
			_diContainer = diContainer;

			PossibleQuestTypes = new[]
			{
				typeof(LevelUpQuest),
				typeof(MonsterDamageQuest),
				typeof(MonsterKillQuest)
			};
		}

		public void Initialize()
		{
			LoadQuests();
		}

		private void LoadQuests()
		{
			Random.InitState((int) DateTime.Now.Ticks);
			foreach (var currentQuest in CurrentQuests)
			{
				if (currentQuest is not Quest quest)
				{
					continue;
				}

				_diContainer.Inject(quest);

				quest.QuestCanceled += OnQuestCanceled;
				quest.QuestCompleted += OnQuestCompleted;
				quest.ProgressChanged += OnQuestProgressChanged;
				quest.Activate(true);
			}

			foreach (var pickableQuest in PickableQuests)
			{
				if (pickableQuest is not Quest quest)
				{
					continue;
				}

				_diContainer.Inject(quest);

				quest.ForceInitialize();
				quest.QuestCompleted += OnQuestCompleted;
				quest.ProgressChanged += OnQuestProgressChanged;
			}
		}

		public List<IQuest> CurrentQuests => _saveDataManager.SaveData.currentQuests;

		public List<IQuest> PickableQuests => _saveDataManager.SaveData.pickableQuests;

		public bool CanPickQuest => _saveDataManager.SaveData.currentQuests.Count < 3;

		public Type[] PossibleQuestTypes { get; }

		public delegate void QuestHandler(object self);

		public delegate void QuestArgsHandler(object self, Quest quest);

		public event QuestHandler? QuestPicked;

		public event QuestArgsHandler? QuestCanceled;

		public event QuestArgsHandler? QuestCompleted;

		public event QuestArgsHandler? QuestProgressChanged;

		private void OnQuestPicked()
		{
			QuestPicked?.Invoke(this);
		}

		private void OnQuestCanceled(object self)
		{
			if (self is Quest quest)
			{
				quest.QuestCanceled -= OnQuestCanceled;
				_saveDataManager.SaveData.currentQuests.Remove(quest);
				_saveDataManager.ApplyToFile();
				QuestCanceled?.Invoke(this, quest);
			}
		}

		private void OnQuestCompleted(object self)
		{
			if (self is Quest quest)
			{
				quest.QuestCompleted -= OnQuestCompleted;
				_saveDataManager.SaveData.currentQuests.Remove(quest);
				_saveDataManager.ApplyToFile();
				QuestCompleted?.Invoke(this, quest);
			}
		}

		private void OnQuestProgressChanged(object self)
		{
			if (self is Quest quest)
			{
				QuestProgressChanged?.Invoke(this, quest);
			}
		}

		public void AddNewPickableQuest()
		{
			if (PickableQuests.Count >= 3)
			{
				return;
			}

			var type = PossibleQuestTypes[Random.Range(0, PossibleQuestTypes.Length)];
			AddNewPickableQuest(type);
		}

		public void AddNewPickableQuest(Type type)
		{
			if (PickableQuests.Count >= 3)
			{
				return;
			}

			if (Activator.CreateInstance(type) is not Quest quest)
			{
				return;
			}

			_diContainer.Inject(quest);

			switch (quest)
			{
				case LevelUpQuest levelUpQuest:
					levelUpQuest.Prepare(0, (uint) Random.Range(1, 4));
					break;
				case MonsterDamageQuest monsterDamageQuest:
					monsterDamageQuest.Prepare(0, (uint) (Random.Range(250, 500) * 100));
					break;
				case MonsterKillQuest monsterKillQuest:
					monsterKillQuest.Prepare(0, (uint) Random.Range(5, 10));
					break;
			}

			quest.QuestCompleted += OnQuestCompleted;
			quest.ProgressChanged += OnQuestProgressChanged;
			PickableQuests.Add(quest);
			_saveDataManager.ApplyToFile();
		}

		public void LinkGameEventsForActivatedQuests()
		{
			foreach (var currentQuest in CurrentQuests)
			{
				currentQuest.LinkGameEvents();
			}
		}

		public void UnlinkGameEventsForActivatedQuests()
		{
			foreach (var currentQuest in CurrentQuests)
			{
				currentQuest.UnlinkGameEvents();
			}
		}

		public void CancelQuest(int idx)
		{
			if (idx < 0 || CurrentQuests == null || idx >= CurrentQuests.Count || !(CurrentQuests[idx] is IQuest quest))
			{
				return;
			}

			CancelQuest(quest);
		}

		public void CancelQuest(IQuest quest)
		{
			if (quest == null)
			{
				throw new ArgumentNullException(nameof(quest));
			}

			var idx = CurrentQuests.FindIndex(q => q == quest);
			if (idx < 0)
			{
				return;
			}

			quest.Deactivate();
			_saveDataManager.SaveData.currentQuests.Remove(quest);
			_saveDataManager.ApplyToFile();
			OnQuestCanceled(quest);
		}

		public void PickQuest(int idx)
		{
			if (idx < 0 || PickableQuests == null || idx >= PickableQuests.Count || !(PickableQuests[idx] is IQuest quest))
			{
				return;
			}

			PickQuest(quest);
		}

		public void PickQuest(IQuest quest)
		{
			if (quest == null)
			{
				throw new ArgumentNullException(nameof(quest));
			}

			var idx = PickableQuests.FindIndex(q => q == quest);
			if (idx < 0)
			{
				return;
			}

			PickableQuests.Remove(quest);
			quest.Activate();
			_saveDataManager.SaveData.currentQuests.Add(quest);
			_saveDataManager.ApplyToFile();
			OnQuestPicked();
			//new UnityTask(TestComplete(quest, 2f));
		}

		private IEnumerator TestComplete(IQuest quest, float delay)
		{
			yield return new WaitForSeconds(delay);
			quest.Complete();
		}
	}
}