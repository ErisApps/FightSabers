using BS_Utils.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FightSabers
{
    public class TimeController : MonoBehaviour
    {
        private static TimeController _timeController;

        public static TimeController Instance {
            get {
                if (_timeController == null)
                    _timeController = new GameObject().AddComponent<TimeController>();
                return _timeController;
            }
        }

        private const float _baseTimeScale = 1f;
        private float _controlledTime = 1f;
        private float _modifiedTime   = 1f;

        private AudioTimeSyncController AudioTimeSync { get; set; }
        private AudioSource             _songAudio;
        private GamePause               _gamePauseManager;

        public float TimeMult {
            get { return _baseTimeScale * ManipulatedTime; }
        }

        //public float TimeScale {
        //    get { return controlledTime; }
        //}

        private float _manipulatedTime = 1f;

        public float ManipulatedTime {
            get { return _manipulatedTime; }
            set {
                _manipulatedTime = Mathf.Clamp(value, 0f, 5f);
                UpdateTimeScale(_modifiedTime);
            }
        }

        public float OverrideTimeScale { get; set; } = -1f;

        private void Start()
        {
            AudioTimeSync = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
            //var ncsem = Resources.FindObjectsOfTypeAll<NoteCutSoundEffectManager>().FirstOrDefault();
            //var ncse = Resources.FindObjectsOfTypeAll<NoteCutSoundEffect>().FirstOrDefault();

            if (AudioTimeSync != null)
            {
                AudioTimeSync.forcedNoAudioSync = true;
                _songAudio = AudioTimeSync.GetField<AudioSource>("_audioSource");
            }
            _gamePauseManager = Resources.FindObjectsOfTypeAll<GamePause>().FirstOrDefault();
        }

        private void LateUpdate()
        {
            Time.timeScale = OverrideTimeScale >= 0f ? OverrideTimeScale : _gamePauseManager.GetPrivateField<bool>("_pause") ? 1f : TimeMult;
            if (!_gamePauseManager.GetPrivateField<bool>("_pause"))
            {
                if (!AudioTimeSync.forcedNoAudioSync)
                    UpdateTimeScale(1f);
                else if (Math.Abs(_songAudio.pitch - _controlledTime) > float.Epsilon)
                    UpdateTimeScale(_songAudio.pitch);
            }
        }

        private void UpdateTimeScale(float modifiedTime)
        {
            _modifiedTime = modifiedTime;
            _controlledTime = modifiedTime * TimeMult;
            if (!_songAudio) return;
            _songAudio.pitch = _controlledTime;
            AudioTimeSync.forcedNoAudioSync = !Mathf.Approximately(_songAudio.pitch, 1f);
        }
    }
}