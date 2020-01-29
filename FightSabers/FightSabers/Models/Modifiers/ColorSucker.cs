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
        #region Properties

        public ColorNoteVisuals   Cnv { get; private set; }
        public GameNoteController Gnc { get; private set; }
        public float StartingColorValue = 0.2f;

        private bool _isDisappearingArrow;

        #endregion

        #region Events

        private static void OnDidInitEvent(ColorNoteVisuals cnv, NoteController noteController)
        {
            ApplyColorVisualOnNotes(true);
        }

        #endregion

        #region Unity methods

        private void Awake()
        {
            title = "Color sucker";
            description = "Note colors disappears if you miss them";
        }

        private void Start()
        {
            Cnv = GetComponentInParent<ColorNoteVisuals>();
            Gnc = GetComponentInParent<GameNoteController>();
            if (Gnc)
                _isDisappearingArrow = Gnc.GetPrivateField<bool>("_disappearingArrow");
        }

        #endregion

        #region Methods

        public static void ApplyColorVisualOnNotes(bool lerpMode)
        {
            foreach (var colorSucker in GameNoteControllerAwakePatch.colorSuckers)
                colorSucker?.ApplyColorVisualOnNote(lerpMode);
        }

        public void ApplyColorVisualOnNote(bool lerpMode)
        {
            if (!Cnv)
                Cnv = transform.parent.GetComponent<ColorNoteVisuals>();
            var color = lerpMode ? Color.Lerp(Color.grey, Cnv.noteColor, ModifierManager.instance != null ? ModifierManager.instance.lerpValue : 0.2f) : Cnv.noteColor;
            if (!_isDisappearingArrow)
            {
                var arrowSpriteRenderer = Cnv.GetPrivateField<SpriteRenderer>("_arrowGlowSpriteRenderer");
                var circleSpriteRenderer = Cnv.GetPrivateField<SpriteRenderer>("_circleGlowSpriteRenderer");
                if (arrowSpriteRenderer)
                    arrowSpriteRenderer.color = color.ColorWithAlpha(Cnv.GetPrivateField<float>("_arrowGlowIntensity"));
                if (circleSpriteRenderer)
                    circleSpriteRenderer.color = color;
            }
            var blockControllers = Cnv.GetPrivateField<MaterialPropertyBlockController[]>("_materialPropertyBlockControllers") ?? Enumerable.Empty<MaterialPropertyBlockController>();
            foreach (var blockController in blockControllers)
            {
                blockController.materialPropertyBlock.SetColor(Shader.PropertyToID("_Color"), color.ColorWithAlpha(1f));
                blockController.ApplyChanges();
            }
        }

        #endregion

        #region Overrides

        public override void EnableModifier()
        {
            if (!Cnv)
                Cnv = transform.parent.GetComponent<ColorNoteVisuals>();
            Cnv.didInitEvent += OnDidInitEvent;
            if (ModifierManager.instance)
                ModifierManager.instance.lerpValue = StartingColorValue;
            ApplyColorVisualOnNotes(true);
        }

        public override void DisableModifier()
        {
            if (!Cnv)
                Cnv = transform.parent.GetComponent<ColorNoteVisuals>();
            Cnv.didInitEvent -= OnDidInitEvent;
            ApplyColorVisualOnNotes(false);
        }

        #endregion
    }
}