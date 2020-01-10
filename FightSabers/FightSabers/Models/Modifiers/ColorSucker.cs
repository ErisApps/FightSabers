using System.Linq;
using BS_Utils.Utilities;
using FightSabers.Core;
using FightSabers.Models.Abstracts;
using FightSabers.Patches;
using UnityEngine;

namespace FightSabers.Models.Modifiers
{
    public class ColorSucker : Modifier
    {
        public ColorNoteVisuals Cnv { get; private set; }

        private void Start()
        {
            Cnv = GetComponentInParent<ColorNoteVisuals>();
        }

        public static void ApplyColorVisualOnNotes(bool lerpMode)
        {
            foreach (var colorSucker in GameNoteControllerInitPatch.colorSuckers)
                colorSucker?.ApplyColorVisualOnNote(lerpMode);
        }

        public void ApplyColorVisualOnNote(bool lerpMode)
        {
            if (!Cnv)
                Cnv = transform.parent.GetComponent<ColorNoteVisuals>();
            var color = lerpMode ? Color.Lerp(Color.grey, Cnv.noteColor, MonsterGenerator.instance.CurrentMonster.LerpValue) : Cnv.noteColor;
            var arrowSpriteRenderer = Cnv.GetPrivateField<SpriteRenderer>("_arrowGlowSpriteRenderer");
            var circleSpriteRenderer = Cnv.GetPrivateField<SpriteRenderer>("_circleGlowSpriteRenderer");
            var blockControllers = Cnv.GetPrivateField<MaterialPropertyBlockController[]>("_materialPropertyBlockControllers") ?? Enumerable.Empty<MaterialPropertyBlockController>();
            if (arrowSpriteRenderer)
                arrowSpriteRenderer.color = color.ColorWithAlpha(Cnv.GetPrivateField<float>("_arrowGlowIntensity"));
            if (circleSpriteRenderer)
                circleSpriteRenderer.color = color;
            foreach (var blockController in blockControllers)
            {
                blockController.materialPropertyBlock.SetColor(Shader.PropertyToID("_Color"), color.ColorWithAlpha(1f));
                blockController.ApplyChanges();
            }
        }

        public override void EnableModifier()
        {
            if (!Cnv)
                Cnv = transform.parent.GetComponent<ColorNoteVisuals>();
            Cnv.didInitEvent += OnDidInitEvent;
            MonsterGenerator.instance.CurrentMonster.LerpValue = 0.5f;
            ApplyColorVisualOnNotes(true);
        }

        public override void DisableModifier()
        {
            if (!Cnv)
                Cnv = transform.parent.GetComponent<ColorNoteVisuals>();
            Cnv.didInitEvent -= OnDidInitEvent;
            ApplyColorVisualOnNotes(false);
        }

        private void OnDidInitEvent(ColorNoteVisuals cnv, NoteController noteController)
        {
            ApplyColorVisualOnNotes(true);
        }
    }
}