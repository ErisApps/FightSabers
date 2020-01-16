using System.Collections;
using System.Linq;
using DigitalRuby.Tween;
using FightSabers.Core;
using FightSabers.Settings;
using FightSabers.Utilities;
using HMUI;
using TMPro;
using UnityEngine;

namespace FightSabers.UI
{
    public class FightSabersProgress : PersistentSingleton<FightSabersProgress>
    {
        #region Properties

        public float ProgressSpeed { get; } = 2.5f;
        public bool CurrentlyAnimated { get; private set; }

        private Material _noGlow;

        private Canvas          _canvas;
        private Image           _monsterLifeBarBg;
        private Image           _monsterLifeBar;
        private TextMeshProUGUI _currentExpText;
        private TextMeshProUGUI _currentLevelText;
        private TextMeshProUGUI _headerText;

        #endregion

        #region Events

        public delegate void BarAnimatedHandler(object self, bool state);
        public event BarAnimatedHandler BeginAnimated;
        public event BarAnimatedHandler EndAnimated;

        private void OnBeginAnimated()
        {
            CurrentlyAnimated = true;
            BeginAnimated?.Invoke(this, CurrentlyAnimated);
        }

        private void OnEndAnimated()
        {
            CurrentlyAnimated = false;
            EndAnimated?.Invoke(this, CurrentlyAnimated);
        }

        private void OnLeveledUp(object self)
        {
            LevelUpAnimation();
        }

        #endregion

        #region Methods

        public void Setup()
        {
            ExperienceSystem.instance.LeveledUp += OnLeveledUp;

            _noGlow = Instantiate(Resources.FindObjectsOfTypeAll<Material>().FirstOrDefault(m => m.name == "UINoGlow"));
            _canvas = new GameObject("FightSabersResultCanvas").AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;
            _canvas.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            _canvas.transform.localPosition = new Vector3(0, 3, 2.5f);
            _canvas.transform.localRotation = Quaternion.Euler(-15, 0, 0);
            _monsterLifeBarBg = new GameObject("Background").AddComponent<Image>();
            _monsterLifeBar = new GameObject("Loading Bar").AddComponent<Image>();
            _currentExpText = new GameObject("CurrentExperienceText").AddComponent<TextMeshProUGUI>();
            _currentLevelText = new GameObject("CurrentLevelText").AddComponent<TextMeshProUGUI>();
            _headerText = new GameObject("HeaderText").AddComponent<TextMeshProUGUI>();

            var rectTransform = _monsterLifeBarBg.transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.SetParent(_canvas.transform, false);
                rectTransform.sizeDelta = new Vector2(160, 16);
                _monsterLifeBarBg.material = _noGlow;
                _monsterLifeBarBg.color = new Color(0, 0, 0, 0.85f);
            }
            rectTransform = _monsterLifeBar.transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.SetParent(_canvas.transform, false);
                rectTransform.sizeDelta = new Vector2(160, 16);
                //MonsterLifeBar.material = _noGlow;
                var tex = Texture2D.whiteTexture;
                var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f, 100, 1);
                _monsterLifeBar.sprite = sprite;
                _monsterLifeBar.type = Image.Type.Filled;
                _monsterLifeBar.fillMethod = Image.FillMethod.Horizontal;
                _monsterLifeBar.fillAmount = 0;
                _monsterLifeBar.color = new Color32(0, 255, 0, 80);
            }
            rectTransform = _headerText.transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.SetParent(_canvas.transform, false);
                rectTransform.localPosition = new Vector3(0, 15f);
                _headerText.alignment = TextAlignmentOptions.Center;
                _headerText.text = $"{Plugin.fightSabersMetadata.Name} v{Plugin.fightSabersMetadata.Version}";
                _headerText.color = Color.white;
                _headerText.fontSize = 6.5f;
            }
            rectTransform = _currentExpText.transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.SetParent(_canvas.transform, false);
                _currentExpText.alignment = TextAlignmentOptions.Center;
                _currentExpText.text = $"0 / {ExperienceSystem.instance.TotalNeededExperienceForNextLevel}";
                _currentExpText.color = Color.white;
                _currentExpText.fontSize = 10;
            }
            rectTransform = _currentLevelText.transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.SetParent(_canvas.transform, false);
                rectTransform.localPosition = new Vector3(0, -17.5f);
                _currentLevelText.alignment = TextAlignmentOptions.Center;
                _currentLevelText.text = $"Level {SaveDataManager.instance.SaveData.level}";
                _currentLevelText.color = Color.white;
                _currentLevelText.fontSize = 8;
            }
            new UnityTask(FillExperienceBar(0, SaveDataManager.instance.SaveData.currentExp, 3.5f));
        }

        public IEnumerator FillExperienceBar(uint currentExp, uint toExp, float delayBefore = 0f)
        {
            OnBeginAnimated();
            yield return new WaitForSeconds(delayBefore);
            gameObject.Tween("FillExpBar" + gameObject.GetInstanceID(), currentExp, toExp, (ExperienceSystem.instance.GetPercentageForExperience(toExp) - ExperienceSystem.instance.GetPercentageForExperience(currentExp)) * ProgressSpeed,
                             TweenScaleFunctions.SineEaseIn, tween => {
                                 _currentExpText.text = $"{(uint)tween.CurrentValue} / {ExperienceSystem.instance.TotalNeededExperienceForNextLevel}";
                                 _monsterLifeBar.fillAmount = ExperienceSystem.instance.GetPercentageForExperiencePrecise(tween.CurrentValue);
                             }, tween => {
                                 OnEndAnimated();
                             });
        }

        #endregion

        #region Animation methods

        private void LevelUpAnimation()
        {
            OnBeginAnimated();
            AnimateTextPosition();
            new UnityTask(AnimateTextRotation());
            AnimateFontSize();
            new UnityTask(AnimateBarColor());
        }

        private void AnimateTextPosition()
        {
            _currentExpText.text = "LEVEL UP!";
            gameObject.Tween("FloatingTextPosition" + gameObject.GetInstanceID(), _currentExpText.transform.localPosition,
                             new Vector3(0, 0, -10f), 2.25f, TweenScaleFunctions.SineEaseOut, tween => {
                                 _currentExpText.transform.localPosition = tween.CurrentValue;
                             }, tween => {
                                 gameObject.Tween("FloatingTextPosition" + gameObject.GetInstanceID(), new Vector3(0, 0, -10f),
                                                  new Vector3(0, 0, 0), 2.25f, TweenScaleFunctions.SineEaseIn, tween2 => {
                                                      _currentExpText.transform.localPosition = tween2.CurrentValue;
                                                  });
                             });
        }

        private IEnumerator AnimateTextRotation()
        {
            for (var i = 0; i < 4; ++i)
            {
                var finished = false;
                gameObject.Tween("FloatingTextRotation" + gameObject.GetInstanceID(), Quaternion.Euler(0, 0, 20),
                                 Quaternion.Euler(0, 0, -20), 0.35f, TweenScaleFunctions.SineEaseOut, tween => {
                                     _currentExpText.transform.localRotation = tween.CurrentValue;
                                 }, tween => {
                                     gameObject.Tween("FloatingTextRotation" + gameObject.GetInstanceID(), Quaternion.Euler(0, 0, -20),
                                                      Quaternion.Euler(0, 0, 20), 0.35f, TweenScaleFunctions.SineEaseIn, tween2 => {
                                                          _currentExpText.transform.localRotation = tween2.CurrentValue;
                                                      }, tween2 => {
                                                          finished = true;
                                                      });
                                 });
                yield return new WaitUntil(() => finished);
            }
            gameObject.Tween("FloatingTextRotation" + gameObject.GetInstanceID(), Quaternion.Euler(0, 0, 20),
                             Quaternion.Euler(0, 0, 0), 0.175f, TweenScaleFunctions.SineEaseOut, tween => {
                                 _currentExpText.transform.localRotation = tween.CurrentValue;
                             });
        }

        private void AnimateFontSize()
        {
            gameObject.Tween("FloatingTextSize" + gameObject.GetInstanceID(), 10, 20, 2.25f, TweenScaleFunctions.SineEaseOut, tween => {
                _currentExpText.fontSize = tween.CurrentValue;
            }, tween => {
                gameObject.Tween("FloatingTextSize" + gameObject.GetInstanceID(), 20, 10, 2.25f, TweenScaleFunctions.SineEaseIn, tween2 => {
                    _currentExpText.fontSize = tween2.CurrentValue;
                });
            });
        }

        private IEnumerator AnimateBarColor()
        {
            for (var i = 0; i < 3; ++i)
            {
                var i1 = i;
                var finished = false;
                gameObject.Tween("LevelUpColor" + gameObject.GetInstanceID(), new Color32(232, 126, 4, 80), new Color32(255, 255, 0, 120), 0.75f,
                                 TweenScaleFunctions.Linear, tween => {
                                     _monsterLifeBar.color = tween.CurrentValue;
                                 }, tween => {
                                     var i2 = i1;
                                     gameObject.Tween("LevelUpColor" + gameObject.GetInstanceID(), new Color32(255, 255, 0, 120), new Color32(232, 126, 4, 80), 0.75f,
                                                      TweenScaleFunctions.Linear, tween2 => {
                                                          _monsterLifeBar.color = tween2.CurrentValue;
                                                      }, tween2 => {
                                                          finished = true;
                                                          if (i2 != 2) return;
                                                          _monsterLifeBar.fillAmount = 0;
                                                          _monsterLifeBar.color = new Color32(0, 255, 0, 80);
                                                          _currentExpText.text = $"0 / {ExperienceSystem.instance.TotalNeededExperienceForNextLevel}";
                                                          _currentLevelText.text = $"Level {SaveDataManager.instance.SaveData.level}";
                                                          OnEndAnimated();
                                                      });
                                 });
                yield return new WaitUntil(() => finished);
            }
        }

        #endregion
    }
}