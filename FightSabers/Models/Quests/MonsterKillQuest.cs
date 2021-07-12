﻿using System;
using FightSabers.Core;
using FightSabers.Models.Abstracts;
using SiraUtil.Tools;

namespace FightSabers.Models.Quests
{
	internal class MonsterKillQuest : Quest
	{
		public uint currentKillCount;
		public uint toKillCount;

		public MonsterKillQuest(SiraLog logger, ExperienceSystem experienceSystem) : base(logger, experienceSystem)
		{
		}

		private void OnMonsterRemoved(object self, MonsterStatus status)
		{
			if (status != MonsterStatus.Killed)
			{
				return;
			}

			currentKillCount += 1;
			Progress = currentKillCount / (float) toKillCount;
			if (currentKillCount > toKillCount || Math.Abs(Progress - 1) < 0.001f) //Double security here
			{
				Complete();
			}
		}

		protected override void Refresh()
		{
			ProgressHint = $"{currentKillCount} / {toKillCount}";
		}

		public override void Complete()
		{
			UnlinkGameEvents();
			base.Complete();
		}

		public override void LinkGameEvents()
		{
			if (IsCompleted && !HasGameEventsActivated)
			{
				return;
			}

			MonsterGenerator.instance.MonsterRemoved += OnMonsterRemoved;
			base.LinkGameEvents();
		}

		public override void UnlinkGameEvents()
		{
			if (IsCompleted && HasGameEventsActivated)
			{
				return;
			}

			MonsterGenerator.instance.MonsterRemoved -= OnMonsterRemoved;
			base.UnlinkGameEvents();
		}

		public void Prepare(uint currentKillCount = 0, uint toKillCount = 10)
		{
			this.currentKillCount = currentKillCount;
			this.toKillCount = toKillCount;
			base.Prepare("Demon slayer", $"Kill a total of <color=#ffa500ff>{toKillCount}</color> monsters!",
				$"{currentKillCount} / {toKillCount}",
				GetType().ToString(), toKillCount * 6,
				currentKillCount / (float) toKillCount);
		}
	}
}