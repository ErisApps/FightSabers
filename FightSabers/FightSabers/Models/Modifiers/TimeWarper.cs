using DigitalRuby.Tween;
using FightSabers.Models.Abstracts;
using UnityEngine;

namespace FightSabers.Models.Modifiers
{
    public class TimeWarper : Modifier
    {
        public override void EnableModifier()
        {
            gameObject.Tween("TimeWarping" + gameObject.GetInstanceID(), TimeController.Instance.ManipulatedTime,
                             Mathf.Clamp(1.25f * strength, 1, float.PositiveInfinity), 1f, TweenScaleFunctions.Linear,
                             tween => { TimeController.Instance.ManipulatedTime = tween.CurrentValue; });
        }

        public override void DisableModifier()
        {
            gameObject.Tween("TimeWarping" + gameObject.GetInstanceID(), TimeController.Instance.ManipulatedTime,
                             1, 1f, TweenScaleFunctions.Linear,
                             tween => { TimeController.Instance.ManipulatedTime = tween.CurrentValue; });
        }
    }
}
