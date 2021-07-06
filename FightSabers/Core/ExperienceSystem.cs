using System;
using System.Collections;
using FightSabers.Settings;
using FightSabers.UI.Controllers;
using FightSabers.Utilities;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;

namespace FightSabers.Core
{
	internal class ExperienceSystem : IInitializable
	{
		private readonly SiraLog _logger;

		private readonly SaveDataManager _saveDataManager;

		public ExperienceSystem(SiraLog logger, SaveDataManager saveDataManager)
		{
			_logger = logger;
			_saveDataManager = saveDataManager;

			RefreshTotalNeededExperience();
		}

		public void Initialize()
		{
			if (!HasOverflowedExperience)
			{
				return;
			}

			while (HasOverflowedExperience)
			{
				_saveDataManager.SaveData.level += 1;
				_saveDataManager.SaveData.currentExp -= TotalNeededExperienceForNextLevel;
				_saveDataManager.SaveData.skillPointRemaining += 1;
				RefreshTotalNeededExperience();
			}

			_saveDataManager.ApplyToFile();
		}

		public uint ExperiencePointsWon { get; private set; }

		public float ExperienceMultiplier { get; private set; } = 1f;

		public uint TotalNeededExperienceForNextLevel { get; private set; }

		public bool IsApplyingExperience { get; private set; }

		public bool HasOverflowedExperience => _saveDataManager.SaveData.currentExp >= TotalNeededExperienceForNextLevel;

		public delegate void ExperienceHandler(object self);

		public event ExperienceHandler? LeveledUp;

		public event ExperienceHandler? ApplyExperienceStarted;

		public event ExperienceHandler? ApplyExperienceFinished;

		private void OnLeveledUp()
		{
			LeveledUp?.Invoke(this);
		}

		private void OnApplyExperienceStarted()
		{
			IsApplyingExperience = true;
			ApplyExperienceStarted?.Invoke(this);
		}

		private void OnApplyExperienceFinished()
		{
			IsApplyingExperience = false;
			ApplyExperienceFinished?.Invoke(this);
		}

		public void RefreshTotalNeededExperience()
		{
			TotalNeededExperienceForNextLevel = GetNeededExperience(_saveDataManager.SaveData.level + 1) - GetNeededExperience(_saveDataManager.SaveData.level);
		}

		public uint GetExperienceBeforeLevelUp()
		{
			return TotalNeededExperienceForNextLevel - _saveDataManager.SaveData.currentExp;
		}

		public uint GetNeededExperience(uint level)
		{
			return (uint) ((level + 25) * Math.Pow(14, Math.Log(Math.Sqrt(level))) / 2);
		}

		public float GetPercentageForExperience(uint exp)
		{
			return (exp - 0f) / (TotalNeededExperienceForNextLevel - 0f);
		}

		public float GetPercentageForExperiencePrecise(float exp)
		{
			return (exp - 0f) / (TotalNeededExperienceForNextLevel - 0f);
		}

		public float GetCurrentExperiencePercentage()
		{
			return GetPercentageForExperience(_saveDataManager.SaveData.currentExp);
		}

		public void AddFightExperience(uint exp)
		{
			ExperiencePointsWon += (uint) (exp * ExperienceMultiplier);
		}

		public void ApplyExperience(float delayApplied = 2.5f)
		{
			if (IsApplyingExperience)
			{
				return;
			}

			OnApplyExperienceStarted();
			new UnityTask(ApplyExperienceInternal(delayApplied));
		}

		private IEnumerator ApplyExperienceInternal(float delayApplied = 2.5f)
		{
			while (ExperiencePointsWon > 0)
			{
				_logger.Debug($"ExperiencePointsWon: {ExperiencePointsWon}");
				if (ExperiencePointsWon >= GetExperienceBeforeLevelUp())
				{
					new UnityTask(OverlayViewController.instance.FillExperienceBar(_saveDataManager.SaveData.currentExp, TotalNeededExperienceForNextLevel, delayApplied));
					yield return new WaitUntil(() => !OverlayViewController.instance.CurrentlyAnimated);
					ExperiencePointsWon -= GetExperienceBeforeLevelUp();
					LevelUp();
					yield return new WaitUntil(() => !OverlayViewController.instance.CurrentlyAnimated);
					delayApplied = 0;
				}
				else
				{
					var addingExperience = ExperiencePointsWon;
					new UnityTask(OverlayViewController.instance.FillExperienceBar(_saveDataManager.SaveData.currentExp, _saveDataManager.SaveData.currentExp + addingExperience,
						delayApplied));
					yield return new WaitUntil(() => !OverlayViewController.instance.CurrentlyAnimated);
					_saveDataManager.SaveData.currentExp += addingExperience;
					ExperiencePointsWon -= addingExperience;
				}
			}

			OnApplyExperienceFinished();
			for (var i = 0; HasOverflowedExperience && i < 5; ++i)
			{
				_logger.Error("This should never happen but the experience system is broken for this session. Please DM me if you're seeing this!");
			}
		}

		public void LevelUp()
		{
			_saveDataManager.SaveData.level += 1;
			_saveDataManager.SaveData.currentExp = 0;
			_saveDataManager.SaveData.skillPointRemaining += 1;
			RefreshTotalNeededExperience();
			OnLeveledUp();
		}

		private void TestLevel() //TODO: Remove later, FPFC testing
		{
			AddFightExperience(1155);
			ApplyExperience();
		}
	}
}