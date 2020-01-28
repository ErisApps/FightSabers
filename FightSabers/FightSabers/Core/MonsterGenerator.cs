using System;
using System.Collections.Generic;
using System.Linq;
using BS_Utils.Gameplay;
using FightSabers.Models;
using FightSabers.Models.Modifiers;
using FightSabers.Patches;
using FightSabers.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FightSabers.Core
{
    public class MonsterGenerator : MonoBehaviour
    {
        public class MonsterSpawnInfo
        {
            public MonsterSpawnInfo(string monsterName, uint monsterHp, float spawnTime, float unspawnTime, uint noteCount, uint monsterDifficulty, Type[] modifierTypes)
            {
                this.monsterName = monsterName;
                this.monsterHp = monsterHp;
                this.spawnTime = spawnTime;
                this.unspawnTime = unspawnTime;
                this.noteCount = noteCount;
                this.monsterDifficulty = monsterDifficulty;
                this.modifierTypes = modifierTypes;
            }

            public string monsterName;
            public uint   monsterHp;
            public float  spawnTime;
            public float  unspawnTime;
            public uint   noteCount;
            public uint   monsterDifficulty;
            public Type[] modifierTypes;
        }

        public static MonsterGenerator instance { get; private set; }

        public MonsterBehaviour CurrentMonster { get; private set; }

        private static readonly Type[] _modifierTypes = { typeof(NoteShrinker), typeof(ColorSucker), typeof(TimeWarper) };

        private AudioTimeSyncController _audioTimeSyncController;
        private LevelData               _mainGameSceneSetupData;
        private BeatmapData             _beatmapData;
        private List<MonsterSpawnInfo>  _monsterSpawnInfos;

        #region Events

        public delegate void MonsterStateHandler(object self, MonsterStatus status);
        public event MonsterStateHandler MonsterAdded;
        public event MonsterStateHandler MonsterRemoved;
        public event MonsterBehaviour.MonsterHitHandler MonsterHurt;

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
            Random.InitState((int)DateTime.Now.Ticks);
            _audioTimeSyncController = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
            _mainGameSceneSetupData = BS_Utils.Plugin.LevelData;
            _beatmapData = _mainGameSceneSetupData?.GameplayCoreSceneSetupData?.difficultyBeatmap?.beatmapData;
            _monsterSpawnInfos = new List<MonsterSpawnInfo>();

            if (_beatmapData != null)
            {
                Logger.log.Debug("There are " + _beatmapData.notesCount              + " notes");
                Logger.log.Debug("There are " + _beatmapData.beatmapLinesData.Length + " lines");
                var songDuration = _mainGameSceneSetupData?.GameplayCoreSceneSetupData?.difficultyBeatmap?.level?.beatmapLevelData?.audioClip?.length ?? -1f;
                if (songDuration >= 0)
                {
                    var notePeriods = GetNotesInPeriods(Mathf.CeilToInt(songDuration / 60));
                    foreach (var notePeriod in notePeriods)
                    {
                        var noteCountDuration = (uint)Random.Range((int)(notePeriod.Count * 0.15f), (int)(notePeriod.Count * 0.3f));
                        var noteIndex = Random.Range(0, notePeriod.Count - (int)noteCountDuration);
                        var monsterDifficulty = (uint)Random.Range(1, 12);
                        var monsterSpawnInfo = new MonsterSpawnInfo("Uber Cthulhu", ((int)(ScoreController.kMaxCutRawScore / 2f) + monsterDifficulty * 4) * noteCountDuration,
                                                                    notePeriod[noteIndex].time - 0.25f, notePeriod[noteIndex + (int)noteCountDuration].time,
                                                                    noteCountDuration, monsterDifficulty,
                                                                    new[] { _modifierTypes[Random.Range(0, _modifierTypes.Length)] });
                        _monsterSpawnInfos.Add(monsterSpawnInfo);
                        Logger.log.Debug(monsterSpawnInfo.monsterName + " lv." + monsterSpawnInfo.monsterDifficulty +
                                         " with " + monsterSpawnInfo.monsterHp + " HP will spawn at: " + monsterSpawnInfo.spawnTime +
                                         " | and will finish at: " + monsterSpawnInfo.unspawnTime);
                        Logger.log.Debug("Modifiers applied: ");
                        foreach (var modifierType in monsterSpawnInfo.modifierTypes)
                            Logger.log.Debug($"-> {modifierType}");
                        Logger.log.Debug("-----------------");
                    }
                }
            }

            InvokeRepeating("CheckForCreateMonster", 0, 0.25f);
        }

        public List<List<BeatmapObjectData>> GetNotesInPeriods(int periodCount)
        {
            if (!_audioTimeSyncController) return null;

            var notePeriods = new List<List<BeatmapObjectData>>();
            for (var idx = 0; idx < periodCount; ++idx)
                notePeriods.Add(new List<BeatmapObjectData>());

            var songDuration = _mainGameSceneSetupData?.GameplayCoreSceneSetupData?.difficultyBeatmap?.level?.beatmapLevelData?.audioClip?.length ?? -1f;
            if (!(songDuration >= 0)) return null;
            var periodTime = songDuration / periodCount;
            Logger.log.Debug("songDuration: " + songDuration);
            Logger.log.Debug("periodTime: "   + periodTime);

            foreach (var beatmapLineData in _beatmapData.beatmapLinesData)
            {
                foreach (var beatmapObjectData in beatmapLineData.beatmapObjectsData)
                {
                    for (var i = 1; i <= periodCount; ++i)
                    {
                        if (beatmapObjectData.beatmapObjectType != BeatmapObjectType.Note || !(beatmapObjectData.time >= periodTime * (i - 1)) || !(beatmapObjectData.time < periodTime * i)) continue;
                        notePeriods[i - 1].Add(beatmapObjectData);
                        break;
                    }
                }
            }

            foreach (var t in notePeriods)
                t.Sort((s1, s2) => s1.time.CompareTo(s2.time));

            foreach (var notePeriod in notePeriods)
                Logger.log.Debug("notePeriod contains " + notePeriod.Count + " notes");

            return notePeriods;
        }

        public void EndCurrentMonsterEncounter()
        {
            if (!CurrentMonster) return;
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
                    if (!(_audioTimeSyncController.songTime > monsterSpawnInfo.spawnTime)) continue;
                    createdMonsterInfo = monsterSpawnInfo;
                    CurrentMonster = MonsterBehaviour.Create();
                    CurrentMonster.MonsterHurt += OnMonsterHurt;
                    new UnityTask(CurrentMonster.ConfigureMonster(createdMonsterInfo)).Finished += (manual, self) => {
                        OnMonsterAdded();
                    };
                    break;
                }
                _monsterSpawnInfos.Remove(createdMonsterInfo);
            }
        }
    }
}