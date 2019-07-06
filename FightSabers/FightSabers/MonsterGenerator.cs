using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS_Utils.Gameplay;
using UnityEngine;

namespace FightSabers
{
    public class MonsterGenerator : MonoBehaviour
    {
        private LevelData   _mainGameSceneSetupData;
        private BeatmapData _beatmapData;

        public static MonsterGenerator Create()
        {
            return new GameObject("[FS|MonsterGenerator]").AddComponent<MonsterGenerator>();
        }

        private void Start()
        {
            _mainGameSceneSetupData = BS_Utils.Plugin.LevelData;
            _beatmapData = _mainGameSceneSetupData?.GameplayCoreSceneSetupData?.difficultyBeatmap?.beatmapData;

            if (_beatmapData != null)
            {
                Logger.log.Info("There are " + _beatmapData.notesCount              + " notes");
                Logger.log.Info("There are " + _beatmapData.beatmapLinesData.Length + " lines");
            }

            var monster = MonsterBehaviour.Create();
            monster.ConfigureMonster("Uber Cthulhu", 15, 60 * 15);
        }
    }
}