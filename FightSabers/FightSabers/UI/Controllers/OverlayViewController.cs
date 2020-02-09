using System.Collections;
using System.Linq;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using DigitalRuby.Tween;
using FightSabers.Core;
using FightSabers.Settings;
using FightSabers.UI.FlowCoordinators;
using FightSabers.Utilities;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace FightSabers.UI.Controllers
{
    internal class OverlayViewController : FightSabersViewController
    {
        public override string ResourceName    => "FightSabers.UI.Views.OverlayView.bsml";
        public override string ContentFilePath => "D:\\Bibliotheques\\Documents\\GitHub\\FightSabers\\FightSabers\\FightSabers\\UI\\Views\\OverlayView.bsml";

        public static OverlayViewController instance;

        #region Properties
        [UIParams] private BSMLParserParams parserParams;

        private bool                       _shouldOpenPage = true;
        private FightSabersFlowCoordinator _flowCoordinatorOwner;

        [UIComponent("switch-fightsabers-btn")]
        private Button _openFightSabersButton;

        [UIComponent("progress-bar-img")]
        private Image _progressBarImage;

        [UIComponent("progress-bg-bar-img")]
        private Image _progressBgBarImage;

        [UIObject("main-background")] private GameObject _mainBackground;
        [UIObject("progress-bar-stack")] private GameObject _progressBarStack;

        public string _coinCount;
        [UIValue("coin-count")]
        public string CoinCount
        {
            get { return _coinCount; }
            set
            {
                _coinCount = value;
                NotifyPropertyChanged();
            }
        }

        private bool _experienceContainerState = true;
        [UIValue("experience-container-state")]
        public bool experienceContainerState
        {
            get { return _experienceContainerState; }
            set
            {
                _experienceContainerState = value;
                NotifyPropertyChanged();
            }
        }

        private bool _fsDisableContainerState;
        [UIValue("fs-disable-container-state")]
        public bool fsDisableContainerState
        {
            get { return _fsDisableContainerState; }
            set
            {
                _fsDisableContainerState = value;
                NotifyPropertyChanged();
            }
        }

        private string _buttonStatus = "Open FightSabers";
        [UIValue("fightsabers-btn-status")]
        public string buttonStatus
        {
            get { return _buttonStatus; }
            private set
            {
                _buttonStatus = value;
                NotifyPropertyChanged();
            }
        }

        private string _headerText = "";
        [UIValue("header-text")]
        public string headerText
        {
            get { return _headerText; }
            private set
            {
                _headerText = value;
                NotifyPropertyChanged();
            }
        }

        private string _currentLevelText = "";
        [UIValue("current-level")]
        public string currentLevelText
        {
            get { return _currentLevelText; }
            private set
            {
                _currentLevelText = value;
                NotifyPropertyChanged();
            }
        }

        [UIComponent("current-exp-text")]
        private TextMeshProUGUI _currentExpTextComp;

        private string _currentExpText = "";
        [UIValue("current-exp")]
        public string currentExpText
        {
            get { return _currentExpText; }
            private set
            {
                _currentExpText = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Events

        public delegate void AnimatedHandler(object self, bool state);
        public event AnimatedHandler BarBeginAnimated;
        public event AnimatedHandler BarEndAnimated;
        public event AnimatedHandler CoinCountBeginAnimated;
        public event AnimatedHandler CoinCountEndAnimated;

        public float BarProgressSpeed     { get; } = 2.5f;
        public bool BarCurrentlyAnimated { get; private set; }
        public bool CoinCountCurrentlyAnimated { get; private set; }

        private void OnBarBeginAnimated()
        {
            BarCurrentlyAnimated = true;
            BarBeginAnimated?.Invoke(this, BarCurrentlyAnimated);
        }

        private void OnBarEndAnimated()
        {
            BarCurrentlyAnimated = false;
            BarEndAnimated?.Invoke(this, BarCurrentlyAnimated);
        }

        private void OnCoinCountBeginAnimated()
        {
            CoinCountCurrentlyAnimated = true;
            CoinCountBeginAnimated?.Invoke(this, CoinCountCurrentlyAnimated);
        }

        private void OnCoinCountEndAnimated()
        {
            CoinCountCurrentlyAnimated = false;
            CoinCountEndAnimated?.Invoke(this, CoinCountCurrentlyAnimated);
        }

        private void OnLeveledUp(object self)
        {
            LevelUpAnimation();
        }

        #endregion

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            base.DidActivate(firstActivation, type);

            instance = this;
            if (firstActivation)
                ExperienceSystem.instance.LeveledUp += OnLeveledUp;

            experienceContainerState = Plugin.config.Value.Enabled;
            fsDisableContainerState = !experienceContainerState;
            //ProgressBar
            var tex = Texture2D.whiteTexture;
            var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f, 100, 1);
            _progressBgBarImage.sprite = sprite;
            _progressBgBarImage.color = new Color(0, 0, 0, 0.85f);
            _progressBarImage.sprite = sprite;
            _progressBarImage.type = Image.Type.Filled;
            _progressBarImage.fillAmount = 0f;
            _progressBarImage.fillMethod = Image.FillMethod.Horizontal;
            _progressBarImage.color = new Color32(0, 255, 0, 80);
            _progressBarImage.material = null;
            //Header text
            headerText = $"{Plugin.fightSabersMetadata.Name} v{Plugin.fightSabersMetadata.Version}";
            //Level text
            currentLevelText = $"Level {SaveDataManager.instance.SaveData.level}";
            //Current exp
            currentExpText = $"0 / {ExperienceSystem.instance.TotalNeededExperienceForNextLevel}";
            CoinCount = "FightCoins: 0";
            new UnityTask(FillExperienceBar(0, SaveDataManager.instance.SaveData.currentExp, 3.5f));
            new UnityTask(FillCoinCount(0, SaveDataManager.instance.SaveData.fightCoinsAmount, 3.5f));
        }

        #region Animation methods

        public IEnumerator FillCoinCount(int currentCoins, int toCoins, float delayBefore = 0f)
        {
            OnCoinCountBeginAnimated();
            yield return new WaitForSeconds(delayBefore);
            gameObject.Tween("FillCoinCount" + gameObject.GetInstanceID(), currentCoins, toCoins, 3,
                             TweenScaleFunctions.SineEaseIn, tween => {
                                 CoinCount = $"FightCoins: {(int)tween.CurrentValue}";
                             }, tween => {
                                 OnCoinCountEndAnimated();
                             });
        }

        public IEnumerator FillExperienceBar(uint currentExp, uint toExp, float delayBefore = 0f)
        {
            OnBarBeginAnimated();
            yield return new WaitForSeconds(delayBefore);
            gameObject.Tween("FillExpBar" + gameObject.GetInstanceID(), currentExp, toExp, (ExperienceSystem.instance.GetPercentageForExperience(toExp) - ExperienceSystem.instance.GetPercentageForExperience(currentExp)) * BarProgressSpeed,
                             TweenScaleFunctions.SineEaseIn, tween => {
                                 currentExpText = $"{(uint)tween.CurrentValue} / {ExperienceSystem.instance.TotalNeededExperienceForNextLevel}";
                                 _progressBarImage.fillAmount = ExperienceSystem.instance.GetPercentageForExperiencePrecise(tween.CurrentValue);
                             }, tween => {
                                 OnBarEndAnimated();
                             });
        }

        private void LevelUpAnimation()
        {
            OnBarBeginAnimated();
            if (!_currentExpTextComp)
            {
                Logger.log.Warn("Current experience text component was null, definitely not expected so skipping the animation");
                return;
            }
            AnimateTextPosition();
            new UnityTask(AnimateTextRotation());
            AnimateFontSize();
            new UnityTask(AnimateBarColor());
        }

        private void AnimateTextPosition()
        {
            currentExpText = "LEVEL UP!";
            _currentExpTextComp.transform.SetParent(_mainBackground.transform);
            gameObject.Tween("FloatingTextPosition" + gameObject.GetInstanceID(), _currentExpTextComp.transform.localPosition,
                             _currentExpTextComp.transform.localPosition + new Vector3(0, 0, -10f), 2.25f,
                             TweenScaleFunctions.SineEaseOut, tween => {
                                 _currentExpTextComp.transform.localPosition = tween.CurrentValue;
                             }, tween => {
                                 gameObject.Tween("FloatingTextPosition" + gameObject.GetInstanceID(), _currentExpTextComp.transform.localPosition,
                                                  _currentExpTextComp.transform.localPosition + new Vector3(0, 0, 10), 2.25f, TweenScaleFunctions.SineEaseIn, tween2 => {
                                                      _currentExpTextComp.transform.localPosition = tween2.CurrentValue;
                                                  }, _ => {
                                                      _currentExpTextComp.transform.SetParent(_progressBarStack.transform);
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
                                     _currentExpTextComp.transform.localRotation = tween.CurrentValue;
                                 }, tween => {
                                     gameObject.Tween("FloatingTextRotation" + gameObject.GetInstanceID(), Quaternion.Euler(0, 0, -20),
                                                      Quaternion.Euler(0, 0, 20), 0.35f, TweenScaleFunctions.SineEaseIn, tween2 => {
                                                          _currentExpTextComp.transform.localRotation = tween2.CurrentValue;
                                                      }, tween2 => {
                                                          finished = true;
                                                      });
                                 });
                yield return new WaitUntil(() => finished);
            }
            gameObject.Tween("FloatingTextRotation" + gameObject.GetInstanceID(), Quaternion.Euler(0, 0, 20),
                             Quaternion.Euler(0, 0, 0), 0.175f, TweenScaleFunctions.SineEaseOut, tween => {
                                 _currentExpTextComp.transform.localRotation = tween.CurrentValue;
                             });
        }

        private void AnimateFontSize()
        {
            gameObject.Tween("FloatingTextSize" + gameObject.GetInstanceID(), 4.5f, 9f, 2.25f, TweenScaleFunctions.SineEaseOut, tween => {
                _currentExpTextComp.fontSize = tween.CurrentValue;
            }, tween => {
                gameObject.Tween("FloatingTextSize" + gameObject.GetInstanceID(), 9f, 4.5f, 2.25f, TweenScaleFunctions.SineEaseIn, tween2 => {
                    _currentExpTextComp.fontSize = tween2.CurrentValue;
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
                                     _progressBarImage.color = tween.CurrentValue;
                                 }, tween => {
                                     var i2 = i1;
                                     gameObject.Tween("LevelUpColor" + gameObject.GetInstanceID(), new Color32(255, 255, 0, 120), new Color32(232, 126, 4, 80), 0.75f,
                                                      TweenScaleFunctions.Linear, tween2 => {
                                                          _progressBarImage.color = tween2.CurrentValue;
                                                      }, tween2 => {
                                                          finished = true;
                                                          if (i2 != 2) return;
                                                          _progressBarImage.fillAmount = 0;
                                                          _progressBarImage.color = new Color32(0, 255, 0, 80);
                                                          currentExpText = $"0 / {ExperienceSystem.instance.TotalNeededExperienceForNextLevel}";
                                                          currentLevelText = $"Level {SaveDataManager.instance.SaveData.level}";
                                                          OnBarEndAnimated();
                                                      });
                                 });
                yield return new WaitUntil(() => finished);
            }
        }

        #endregion

        [UIAction("switch-fightsabers-act")]
        public void ShowModPageClick()
        {
            if (_shouldOpenPage)
            {
                var currentFlow = Resources.FindObjectsOfTypeAll<FlowCoordinator>().FirstOrDefault(f => f.isActivated);
                _flowCoordinatorOwner = BeatSaberUI.CreateFlowCoordinator<FightSabersFlowCoordinator>();
                _flowCoordinatorOwner.oldCoordinator = currentFlow;
                currentFlow.PresentFlowCoordinator(_flowCoordinatorOwner);
            }
            else
                _flowCoordinatorOwner.oldCoordinator.DismissFlowCoordinator(_flowCoordinatorOwner);
            _shouldOpenPage = !_shouldOpenPage;
            buttonStatus = _shouldOpenPage ? "Open FightSabers" : "Close FightSabers";
        }
    }
}