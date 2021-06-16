using System;
using System.Collections;
using System.Linq;
using FightSabers.Models.Modifiers;
using UnityEngine;

namespace FightSabers.Core
{
    public class ModifierManager : MonoBehaviour
    {
        public TimeWarper timeWarper;
        public Type[] modifiers;

        public static ModifierManager instance;

        public int noteCountDuration;

        public float noteShrinkerStrength = 1f;
        public float colorSuckerStrength = 1f;
        public float timeWarperStrength = 1f;

        public float   lerpValue;
        public Vector2 LerpValueRange { get; private set; }

        private void Awake()
        {
            instance = this;
        }

        public IEnumerator ConfigureModifiers(float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            foreach (var modifier in modifiers)
            {
                if (modifier == typeof(ColorSucker))
                {
                    lerpValue = 0.2f;
                    LerpValueRange = new Vector2(0.2f, 1);
                }
                else if (modifier == typeof(TimeWarper))
                {
                    timeWarper = gameObject.AddComponent<TimeWarper>();
                    timeWarper.strength = timeWarperStrength;
                    timeWarper.EnableModifier();
                }
            }
        }

        public void ReduceColorSuckerColorness()
        {
            if (modifiers.Contains(typeof(ColorSucker)))
            {
                lerpValue -= 0.1f;
                lerpValue = lerpValue < LerpValueRange.x ? LerpValueRange.x : lerpValue;
                ColorSucker.ApplyColorVisualOnNotes(true);
            }
        }

        public void ImproveColorSuckerColorness()
        {
            if (modifiers.Contains(typeof(ColorSucker)))
            {
                lerpValue += 0.8f / (noteCountDuration * 0.85f);
                lerpValue = lerpValue > LerpValueRange.y ? LerpValueRange.y : lerpValue;
                ColorSucker.ApplyColorVisualOnNotes(true);
            }
        }
    }
}
