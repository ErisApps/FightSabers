﻿using System;
using System.Collections.Generic;
using System.Linq;
using FightSabers.Models;
using FightSabers.Models.Modifiers;
using FightSabers.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FightSabers.Core
{
	public class MonsterGenerator : MonoBehaviour
	{
		public class MonsterSpawnInfo
		{
			public MonsterSpawnInfo(string monsterName, uint monsterHp, float spawnTime, float deSpawnTime, uint noteCount, uint monsterDifficulty, Type[] modifierTypes)
			{
				MonsterName = monsterName;
				MonsterHp = monsterHp;
				SpawnTime = spawnTime;
				DeSpawnTime = deSpawnTime;
				NoteCount = noteCount;
				MonsterDifficulty = monsterDifficulty;
				ModifierTypes = modifierTypes;
			}

			public readonly string MonsterName;
			public readonly uint MonsterHp;
			public readonly float SpawnTime;
			public readonly float DeSpawnTime;
			public readonly uint NoteCount;
			public readonly uint MonsterDifficulty;
			public readonly Type[] ModifierTypes;
		}

		public static MonsterGenerator instance { get; private set; }

		public MonsterBehaviour CurrentMonster { get; private set; }

		private static readonly Type[] _modifierTypes =
		{
			typeof(NoteShrinker),
			typeof(ColorSucker),
			typeof(TimeWarper)
		};

		private AudioTimeSyncController _audioTimeSyncController;
		private LevelData _mainGameSceneSetupData;
		private BeatmapData _beatmapData;
		private List<MonsterSpawnInfo> _monsterSpawnInfos;

		#region Events

		public delegate void MonsterStateHandler(object self, MonsterStatus status);

		public event MonsterStateHandler? MonsterAdded;
		public event MonsterStateHandler? MonsterRemoved;
		public event MonsterBehaviour.MonsterHitHandler? MonsterHurt;

		private void OnMonsterAdded()
		{
			MonsterAdded?.Invoke(this, MonsterStatus.Alive);
		}

		private void OnMonsterRemoved(MonsterStatus status)
		{
			MonsterRemoved?.Invoke(this, status);
		}

		private void OnMonsterHurt(object self, int damage)
		{
			MonsterHurt?.Invoke(self, damage);
		}

		#endregion

		public static MonsterGenerator Create()
		{
			instance = new GameObject("[FS|MonsterGenerator]").AddComponent<MonsterGenerator>();
			return instance;
		}

		private void OnDestroy()
		{
			CancelInvoke();
		}

		private void Start()
		{
			Random.InitState((int) DateTime.Now.Ticks);
			_audioTimeSyncController = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
			_mainGameSceneSetupData = BS_Utils.Plugin.LevelData;
			_beatmapData = _mainGameSceneSetupData?.GameplayCoreSceneSetupData?.difficultyBeatmap?.beatmapData;
			_monsterSpawnInfos = new List<MonsterSpawnInfo>();

			if (_beatmapData != null)
			{
				Logger.log.Debug("There are " + _beatmapData.notesCount + " notes");
				Logger.log.Debug("There are " + _beatmapData.beatmapLinesData.Length + " lines");
				var songDuration = _mainGameSceneSetupData?.GameplayCoreSceneSetupData?.difficultyBeatmap?.level?.beatmapLevelData?.audioClip?.length ?? -1f;
				if (songDuration >= 0)
				{
					var notePeriods = GetNotesInPeriods(Mathf.CeilToInt(songDuration / 60));
					foreach (var notePeriod in notePeriods)
					{
						var noteCountDuration = (uint) Random.Range((int) (notePeriod.Count * 0.15f), (int) (notePeriod.Count * 0.3f));
						var noteIndex = Random.Range(0, notePeriod.Count - (int) noteCountDuration);
						var monsterDifficulty = (uint) Random.Range(1, 12);
						var monsterSpawnInfo = new MonsterSpawnInfo("Uber Cthulhu", ((int) (ScoreController.kMaxCutRawScore / 2f) + monsterDifficulty * 4) * noteCountDuration,
							notePeriod[noteIndex].time - 0.25f, notePeriod[noteIndex + (int) noteCountDuration].time,
							noteCountDuration, monsterDifficulty,
							new[]
							{
								_modifierTypes[Random.Range(0, _modifierTypes.Length)]
							});
						_monsterSpawnInfos.Add(monsterSpawnInfo);
						Logger.log.Debug(monsterSpawnInfo.MonsterName + " lv." + monsterSpawnInfo.MonsterDifficulty +
						                 " with " + monsterSpawnInfo.MonsterHp + " HP will spawn at: " + monsterSpawnInfo.SpawnTime +
						                 " | and will finish at: " + monsterSpawnInfo.DeSpawnTime);
						Logger.log.Debug("Modifiers applied: ");
						foreach (var modifierType in monsterSpawnInfo.ModifierTypes)
						{
							Logger.log.Debug($"-> {modifierType}");
						}

						Logger.log.Debug("-----------------");
					}
				}
			}

			InvokeRepeating("CheckForCreateMonster", 0, 0.25f);
		}

		public List<List<BeatmapObjectData>> GetNotesInPeriods(int periodCount)
		{
			if (!_audioTimeSyncController)
			{
				return null;
			}

			var notePeriods = new List<List<BeatmapObjectData>>();
			for (var idx = 0; idx < periodCount; ++idx)
			{
				notePeriods.Add(new List<BeatmapObjectData>());
			}

			var songDuration = _mainGameSceneSetupData?.GameplayCoreSceneSetupData?.difficultyBeatmap?.level?.beatmapLevelData?.audioClip?.length ?? -1f;
			if (!(songDuration >= 0))
			{
				return null;
			}

			var periodTime = songDuration / periodCount;
			Logger.log.Debug("songDuration: " + songDuration);
			Logger.log.Debug("periodTime: " + periodTime);

			foreach (var beatmapLineData in _beatmapData.beatmapLinesData)
			{
				foreach (var beatmapObjectData in beatmapLineData.beatmapObjectsData)
				{
					for (var i = 1; i <= periodCount; ++i)
					{
						if (beatmapObjectData.beatmapObjectType != BeatmapObjectType.Note || !(beatmapObjectData.time >= periodTime * (i - 1)) || !(beatmapObjectData.time < periodTime * i))
						{
							continue;
						}

						notePeriods[i - 1].Add(beatmapObjectData);
						break;
					}
				}
			}

			foreach (var t in notePeriods)
			{
				t.Sort((s1, s2) => s1.time.CompareTo(s2.time));
			}

			foreach (var notePeriod in notePeriods)
			{
				Logger.log.Debug("notePeriod contains " + notePeriod.Count + " notes");
			}

			return notePeriods;
		}

		public void EndCurrentMonsterEncounter()
		{
			if (!CurrentMonster)
			{
				return;
			}

			Destroy(CurrentMonster.gameObject, 4);
			OnMonsterRemoved(CurrentMonster.CurrentStatus);
			CurrentMonster = null;
		}

		private void CheckForCreateMonster()
		{
			if (_audioTimeSyncController && (CurrentMonster == null /*|| CurrentMonster != null && !CurrentMonster.Canvas.enabled*/))
			{
				MonsterSpawnInfo createdMonsterInfo = null;
				foreach (var monsterSpawnInfo in _monsterSpawnInfos)
				{
					if (!(_audioTimeSyncController.songTime > monsterSpawnInfo.SpawnTime))
					{
						continue;
					}

					createdMonsterInfo = monsterSpawnInfo;
					CurrentMonster = MonsterBehaviour.Create();
					CurrentMonster.MonsterHurt += OnMonsterHurt;
					new UnityTask(CurrentMonster.ConfigureMonster(createdMonsterInfo)).Finished += (manual, self) =>
					{
						OnMonsterAdded();
					};
					break;
				}

				_monsterSpawnInfos.Remove(createdMonsterInfo);
			}
		}
	}
}