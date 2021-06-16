using DigitalRuby.Tween;
using FightSabers.Models.Abstracts;
using UnityEngine;

namespace FightSabers.Models.Modifiers
{
    public class NoteShrinker : Modifier
    {
        public static readonly Vector3 BaseScaleApplied = new Vector3(0.65f, 0.65f, 0.65f);
        public static Vector3 ScaleApplied { get; private set; }

        private Vector3 _oldScale;

        private void Awake()
        {
            title = "Note shrinker";
            description = "Shrinks notes";
            ScaleApplied = BaseScaleApplied;
        }

        public override void EnableModifier()
        {
            strength = strength < 0 ? 1 : strength;
            ScaleApplied = BaseScaleApplied * (1 / strength);
            ScaleApplied = ScaleApplied.x >= 1 ? Vector3.one : ScaleApplied;
            ScaleApplied = ScaleApplied.x <= 0 ? new Vector3(Mathf.Abs(ScaleApplied.x), Mathf.Abs(ScaleApplied.y), Mathf.Abs(ScaleApplied.z)) : ScaleApplied;
            _oldScale = transform.localScale;
            var t = gameObject.Tween("ShrinkerScaleTransition" + gameObject.GetInstanceID(), transform.localScale, ScaleApplied, 0.35f,
                                     TweenScaleFunctions.Linear, tween => { transform.localScale = tween.CurrentValue; });
            t.ForceUpdate = true;
        }

        public override void DisableModifier()
        {
            var t = gameObject.Tween("ShrinkerScaleTransition" + gameObject.GetInstanceID(), transform.localScale, _oldScale, 0.35f,
                                     TweenScaleFunctions.Linear, tween => { transform.localScale = tween.CurrentValue; });
            t.ForceUpdate = true;
        }
    }
}
