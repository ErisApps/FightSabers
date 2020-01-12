using System.Linq;
using BS_Utils.Utilities;
using DigitalRuby.Tween;
using FightSabers.Models.Abstracts;
using UnityEngine;

namespace FightSabers.Models.Modifiers
{
    public class TimeWarper : Modifier
    {
        private AudioTimeSyncController _audioTimeSyncController;
        private float _baseTimeScale;
        private AudioSource _audioSource;
        private GameplayCoreSceneSetup _gameCoreSceneSetup;
        private AudioManagerSO _mixer;

        private void Awake()
        {
            title = "Time warper";
            description = "Song is playing faster";
        }

        public override void EnableModifier()
        {
            if (!_audioTimeSyncController)
                _audioTimeSyncController = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
            if (!_gameCoreSceneSetup)
                _gameCoreSceneSetup = Resources.FindObjectsOfTypeAll<GameplayCoreSceneSetup>().FirstOrDefault();
            if (!_mixer)
                _mixer = _gameCoreSceneSetup.GetPrivateField<AudioManagerSO>("_audioMixer");

            _audioSource = _audioTimeSyncController.GetPrivateField<AudioSource>("_audioSource");
            _baseTimeScale = _audioTimeSyncController.GetPrivateField<float>("_timeScale");
            gameObject.Tween("TimeWarping" + gameObject.GetInstanceID(), _baseTimeScale,
                             _baseTimeScale + Mathf.Clamp(0.2f * strength, 0, float.PositiveInfinity),
                             1f, TweenScaleFunctions.Linear, tween =>
                             {
                                 _audioTimeSyncController.SetPrivateField("_timeScale", tween.CurrentValue);
                                 _audioSource.pitch = tween.CurrentValue;
                                 _mixer.musicPitch = 1f / tween.CurrentValue;
                             });
            // RIP TimeController
            //gameObject.Tween("TimeWarping" + gameObject.GetInstanceID(), TimeController.Instance.ManipulatedTime,
            //                 Mathf.Clamp(1.25f * strength, 1, float.PositiveInfinity), 1f, TweenScaleFunctions.Linear,
            //                 tween => {
            //                     TimeController.Instance.ManipulatedTime = tween.CurrentValue;
            //                 });
        }

        public override void DisableModifier()
        {
            gameObject.Tween("TimeWarping" + gameObject.GetInstanceID(),
                             _audioTimeSyncController.GetPrivateField<float>("_timeScale"), _baseTimeScale,
                             1f, TweenScaleFunctions.Linear, tween =>
                             {
                                 _audioTimeSyncController.SetPrivateField("_timeScale", tween.CurrentValue);
                                 _audioSource.pitch = tween.CurrentValue;
                                 _mixer.musicPitch = 1f / tween.CurrentValue;
                             });
            // RIP TimeController
            //gameObject.Tween("TimeWarping" + gameObject.GetInstanceID(), TimeController.Instance.ManipulatedTime,
            //                 1, 1f, TweenScaleFunctions.Linear,
            //                 tween => {
            //                     TimeController.Instance.ManipulatedTime = tween.CurrentValue;
            //                 });
        }
    }
}
