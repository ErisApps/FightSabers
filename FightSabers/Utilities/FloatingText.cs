using System;
using DigitalRuby.Tween;
using TMPro;
using UnityEngine;

namespace FightSabers.Utilities
{
    public class FloatingText : MonoBehaviour
    {
        #region Properties
        public Vector2 canvasSize    = new Vector2(100, 50);
        public Vector2 labelPosition = new Vector2(0, 15);
        public Vector2 labelSize     = new Vector2(100, 20);
        public float   labelFontSize = 14f;

        public bool               fadeOutText;
        public Func<float, float> tweenScaleFunc;
        public Vector3            tweenEndPosition;
        #endregion

        private Canvas   _canvas;
        private TMP_Text _textLabel;

        public static FloatingText Create()
        {
            return new GameObject("[FloatingText]").AddComponent<FloatingText>();
        }

        public void ConfigureText()
        {
            if (!_canvas)
                _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;
            var rectTransform = _canvas.transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = canvasSize;
                if (!_textLabel)
                    _textLabel = Utils.CreateText((RectTransform)_canvas.transform, "", labelPosition);
            }
            rectTransform = _textLabel.transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.SetParent(_canvas.transform, false);
                rectTransform.anchoredPosition = labelPosition;
                rectTransform.sizeDelta = labelSize;
            }
            _textLabel.fontSize = labelFontSize;

            _canvas.enabled = false;
        }

        public void DisplayText(Vector3 pos, Vector3 rot, Vector3 scale, string labelInfo, float duration)
        {
            transform.localPosition = pos;
            transform.localEulerAngles = rot;
            transform.localScale = scale;

            if (_textLabel)
                _textLabel.text = labelInfo;
            if (_canvas)
                _canvas.enabled = true;

            if (tweenScaleFunc != null)
            {
                gameObject.Tween("FloatingText" + gameObject.GetInstanceID(), transform.localPosition, tweenEndPosition, duration, tweenScaleFunc,
                                 delegate (ITween<Vector3> tween) {
                                     if (!this) return;
                                     transform.localPosition = tween.CurrentValue;
                                     if (fadeOutText && _textLabel)
                                         _textLabel.alpha = 1f - tween.CurrentProgress;
                                 }, delegate
                                 {
                                     if (this && gameObject)
                                        Destroy(gameObject);
                                 });
            }
        }
    }
}