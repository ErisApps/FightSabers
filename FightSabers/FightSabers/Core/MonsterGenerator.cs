using System;
using System.Collections.Generic;
using System.Linq;
using BS_Utils.Gameplay;
using FightSabers.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FightSabers.Core
{
    public class MonsterGenerator : MonoBehaviour
    {
        public class MonsterSpawnInfo
        {
            public MonsterSpawnInfo(string monsterName, uint monsterHp, float spawnTime, uint noteCount, uint monsterDifficulty)
            {
                this.monsterName = monsterName;
                this.monsterHp = monsterHp;
                this.spawnTime = spawnTime;
                this.noteCount = noteCount;
                this.monsterDifficulty = monsterDifficulty;
            }

            public string monsterName;
            public uint   monsterHp;
            public float  spawnTime;
            public uint   noteCount;
            public uint   monsterDifficulty;
        }

        private AudioTimeSyncController _audioTimeSyncController;
        private LevelData               _mainGameSceneSetupData;
        private BeatmapData             _beatmapData;
        private List<MonsterSpawnInfo>  _monsterSpawnInfos;
        private MonsterBehaviour        _currentMonster;

        public static MonsterGenerator Create()
        {
            return new GameObject("[FS|MonsterGenerator]").AddComponent<MonsterGenerator>();
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
                                                                    notePeriod[noteIndex].time - 0.25f,
                                                                    noteCountDuration, monsterDifficulty);
                        _monsterSpawnInfos.Add(monsterSpawnInfo);
                        Logger.log.Warn(monsterSpawnInfo.monsterName + " lv." + monsterSpawnInfo.monsterDifficulty +
                                         " with " + monsterSpawnInfo.monsterHp + " HP will spawn at: " + monsterSpawnInfo.spawnTime +
                                         " | and will finish at: " + notePeriod[noteIndex + (int)monsterSpawnInfo.noteCount].time);
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

        private void CheckForCreateMonster()
        {
            if (_audioTimeSyncController && (_currentMonster == null || _currentMonster != null && !_currentMonster.GetComponent<Canvas>().enabled))
            {
                MonsterSpawnInfo createdMonsterInfo = null;
                foreach (var monsterSpawnInfo in _monsterSpawnInfos)
                {
                    if (!(_audioTimeSyncController.songTime > monsterSpawnInfo.spawnTime)) continue;
                    createdMonsterInfo = monsterSpawnInfo;
                    _currentMonster = MonsterBehaviour.Create();
                    new UnityTask(_currentMonster.ConfigureMonster(createdMonsterInfo.monsterName, createdMonsterInfo.noteCount,
                                                                   createdMonsterInfo.monsterHp, createdMonsterInfo.monsterDifficulty));
                    break;
                }
                _monsterSpawnInfos.Remove(createdMonsterInfo);
            }
        }
    }
}