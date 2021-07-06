using System;
using BeatSaberMarkupLanguage;
using DigitalRuby.Tween;
using TMPro;
using UnityEngine;

namespace FightSabers.Utilities
{
	public class FloatingText : MonoBehaviour
	{
		public Vector2 canvasSize = new(100, 50);
		public Vector2 labelPosition = new(0, 15);
		public Vector2 labelSize = new(100, 20);
		public float labelFontSize = 14f;

		public bool fadeOutText;
		public Func<float, float>? TweenScaleFunc;
		public Vector3 tweenEndPosition;

		private Canvas _canvas = null!;
		private TMP_Text _textLabel = null!;

		public static FloatingText Create()
		{
			return new GameObject("[FloatingText]").AddComponent<FloatingText>();
		}

		public void ConfigureText()
		{
			if (!_canvas)
			{
				_canvas = gameObject.AddComponent<Canvas>();
			}

			_canvas.renderMode = RenderMode.WorldSpace;
			var rectTransform = _canvas.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.sizeDelta = canvasSize;
				if (!_textLabel)
				{
					_textLabel = BeatSaberUI.CreateText((RectTransform) _canvas.transform, "", labelPosition);
				}
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

		// ReSharper disable once CognitiveComplexity
		public void DisplayText(Vector3 pos, Vector3 rot, Vector3 scale, string labelInfo, float duration)
		{
			var transform1 = transform;
			transform1.localPosition = pos;
			transform1.localEulerAngles = rot;
			transform1.localScale = scale;

			if (_textLabel)
			{
				_textLabel.text = labelInfo;
			}

			if (_canvas)
			{
				_canvas.enabled = true;
			}

			if (TweenScaleFunc != null)
			{
				gameObject.Tween("FloatingText" + gameObject.GetInstanceID(), transform.localPosition, tweenEndPosition, duration, TweenScaleFunc,
					tween =>
					{
						if (!this)
						{
							return;
						}

						transform.localPosition = tween.CurrentValue;
						if (fadeOutText && _textLabel)
						{
							_textLabel.alpha = 1f - tween.CurrentProgress;
						}
					}, _ =>
					{
						if (this && gameObject)
						{
							Destroy(gameObject);
						}
					});
			}
		}
	}
}