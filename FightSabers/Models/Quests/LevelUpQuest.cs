using System;
using FightSabers.Core;
using FightSabers.Models.Abstracts;
using SiraUtil.Tools;

namespace FightSabers.Models.Quests
{
	internal class LevelUpQuest : Quest
	{
		public uint currentLevelUpCount;
		public uint toLevelUpCount;

		public LevelUpQuest(SiraLog logger, ExperienceSystem experienceSystem) : base(logger, experienceSystem)
		{
		}

		private void OnLeveledUp(object self)
		{
			currentLevelUpCount += 1;
			Progress = currentLevelUpCount / (float) toLevelUpCount;
			if (currentLevelUpCount >= toLevelUpCount || Math.Abs(Progress - 1) < 0.001f) //Double security here
			{
				Complete();
			}
		}

		protected override void Refresh()
		{
			ProgressHint = $"{currentLevelUpCount} / {toLevelUpCount}";
		}

		public override void Activate(bool forceInitialize = false)
		{
			if (!IsInitialized && forceInitialize)
			{
				IsInitialized = true;
			}

			if (!IsInitialized || IsActivated)
			{
				return;
			}

			Logger.Debug(">>> Specific quest activated!");
			ExperienceSystem.LeveledUp += OnLeveledUp;
		}

		public override void Deactivate()
		{
			if (!IsInitialized || !IsActivated)
			{
				return;
			}

			ExperienceSystem.LeveledUp -= OnLeveledUp;
			base.Deactivate();
		}

		public override void Complete()
		{
			ExperienceSystem.LeveledUp -= OnLeveledUp;
			base.Complete();
			ExperienceSystem.ApplyExperience();
		}

		public void Prepare(uint currentLevelUpCount = 0, uint toLevelUpCount = 1)
		{
			this.currentLevelUpCount = currentLevelUpCount;
			this.toLevelUpCount = toLevelUpCount;
			base.Prepare("Raise up", $"Level up <color=#ffa500ff>{toLevelUpCount}</color> level{(toLevelUpCount != 1 ? "s" : "")}!",
				$"{currentLevelUpCount} / {toLevelUpCount}",
				GetType().ToString(), 15 * toLevelUpCount,
				currentLevelUpCount / (float) toLevelUpCount);
		}
	}
}