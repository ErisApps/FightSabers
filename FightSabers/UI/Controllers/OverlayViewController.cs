using System.Collections;
using System.Linq;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using DigitalRuby.Tween;
using FightSabers.Core;
using FightSabers.Settings;
using FightSabers.UI.FlowCoordinators;
using FightSabers.Utilities;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Image = UnityEngine.UI.Image;

namespace FightSabers.UI.Controllers
{
	[HotReload(RelativePathToLayout = @"..\Views\OverlayView.bsml")]
	[ViewDefinition("FightSabers.UI.Views.OverlayView.bsml")]
	internal class OverlayViewController : BSMLAutomaticViewController, ICanControlFlowCoordinator
	{
		public static OverlayViewController instance;

		private bool _shouldOpenPage = true;
		public FightSabersFlowCoordinator FlowCoordinatorOwner { get; set; } = null!;

		[Inject]
		internal void Construct(PluginConfig config)
		{
			_config = config;
		}


		[UIComponent("switch-fightsabers-btn")]
		private Button _openFightSabersButton;

		[UIComponent("progress-bar-img")]
		private Image _progressBarImage;

		[UIComponent("progress-bg-bar-img")]
		private Image _progressBgBarImage;

		[UIObject("main-background")]
		private GameObject _mainBackground;

		[UIObject("progress-bar-stack")]
		private GameObject _progressBarStack;

		private bool _experienceContainerState = true;


		[UIValue("experience-container-state")]
		public bool experienceContainerState
		{
			get => _experienceContainerState;
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
			get => _fsDisableContainerState;
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
			get => _buttonStatus;
			private set
			{
				_buttonStatus = value;
				NotifyPropertyChanged();
			}
		}

		private string _headerText = "";


		[UIValue("header-text")]
		public string HeaderText
		{
			get => _headerText;
			private set
			{
				_headerText = value;
				NotifyPropertyChanged();
			}
		}

		private string _currentLevelText = "";


		[UIValue("current-level")]
		public string CurrentLevelText
		{
			get => _currentLevelText;
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
		public string CurrentExpText
		{
			get => _currentExpText;
			private set
			{
				_currentExpText = value;
				NotifyPropertyChanged();
			}
		}

		public delegate void BarAnimatedHandler(object self, bool state);

		public event BarAnimatedHandler? BeginAnimated;
		public event BarAnimatedHandler? EndAnimated;

		public float ProgressSpeed { get; } = 2.5f;
		public bool CurrentlyAnimated { get; private set; }


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

		protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
		{
			base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

			instance = this;
			if (firstActivation)
			{
				ExperienceSystem.instance.LeveledUp += OnLeveledUp;
			}

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
			HeaderText = $"{Plugin.FightSabersMetadata.Name} v{Plugin.FightSabersMetadata.Version}";
			//Level text
			CurrentLevelText = $"Level {SaveDataManager.instance.SaveData.level}";
			//Current exp
			CurrentExpText = $"0 / {ExperienceSystem.instance.TotalNeededExperienceForNextLevel}";
			new UnityTask(FillExperienceBar(0, SaveDataManager.instance.SaveData.currentExp, 3.5f));
		}

		public IEnumerator FillExperienceBar(uint currentExp, uint toExp, float delayBefore = 0f)
		{
			OnBeginAnimated();
			yield return new WaitForSeconds(delayBefore);
			gameObject.Tween("FillExpBar" + gameObject.GetInstanceID(), currentExp, toExp,
				(ExperienceSystem.instance.GetPercentageForExperience(toExp) - ExperienceSystem.instance.GetPercentageForExperience(currentExp)) * ProgressSpeed,
				TweenScaleFunctions.SineEaseIn, tween =>
				{
					CurrentExpText = $"{(uint) tween.CurrentValue} / {ExperienceSystem.instance.TotalNeededExperienceForNextLevel}";
					_progressBarImage.fillAmount = ExperienceSystem.instance.GetPercentageForExperiencePrecise(tween.CurrentValue);
				}, tween =>
				{
					OnEndAnimated();
				});
		}

		private void LevelUpAnimation()
		{
			OnBeginAnimated();
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
			CurrentExpText = "LEVEL UP!";
			_currentExpTextComp.transform.SetParent(_mainBackground.transform);
			gameObject.Tween("FloatingTextPosition" + gameObject.GetInstanceID(), _currentExpTextComp.transform.localPosition,
				_currentExpTextComp.transform.localPosition + new Vector3(0, 0, -10f), 2.25f,
				TweenScaleFunctions.SineEaseOut, tween =>
				{
					_currentExpTextComp.transform.localPosition = tween.CurrentValue;
				}, tween =>
				{
					gameObject.Tween("FloatingTextPosition" + gameObject.GetInstanceID(), _currentExpTextComp.transform.localPosition,
						_currentExpTextComp.transform.localPosition + new Vector3(0, 0, 10), 2.25f, TweenScaleFunctions.SineEaseIn, tween2 =>
						{
							_currentExpTextComp.transform.localPosition = tween2.CurrentValue;
						}, _ =>
						{
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
					Quaternion.Euler(0, 0, -20), 0.35f, TweenScaleFunctions.SineEaseOut, tween =>
					{
						_currentExpTextComp.transform.localRotation = tween.CurrentValue;
					}, tween =>
					{
						gameObject.Tween("FloatingTextRotation" + gameObject.GetInstanceID(), Quaternion.Euler(0, 0, -20),
							Quaternion.Euler(0, 0, 20), 0.35f, TweenScaleFunctions.SineEaseIn, tween2 =>
							{
								_currentExpTextComp.transform.localRotation = tween2.CurrentValue;
							}, tween2 =>
							{
								finished = true;
							});
					});
				yield return new WaitUntil(() => finished);
			}

			gameObject.Tween("FloatingTextRotation" + gameObject.GetInstanceID(), Quaternion.Euler(0, 0, 20),
				Quaternion.Euler(0, 0, 0), 0.175f, TweenScaleFunctions.SineEaseOut, tween =>
				{
					_currentExpTextComp.transform.localRotation = tween.CurrentValue;
				});
		}

		private void AnimateFontSize()
		{
			gameObject.Tween("FloatingTextSize" + gameObject.GetInstanceID(), 4.5f, 9f, 2.25f, TweenScaleFunctions.SineEaseOut, tween =>
			{
				_currentExpTextComp.fontSize = tween.CurrentValue;
			}, tween =>
			{
				gameObject.Tween("FloatingTextSize" + gameObject.GetInstanceID(), 9f, 4.5f, 2.25f, TweenScaleFunctions.SineEaseIn, tween2 =>
				{
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
					TweenScaleFunctions.Linear, tween =>
					{
						_progressBarImage.color = tween.CurrentValue;
					}, tween =>
					{
						var i2 = i1;
						gameObject.Tween("LevelUpColor" + gameObject.GetInstanceID(), new Color32(255, 255, 0, 120), new Color32(232, 126, 4, 80), 0.75f,
							TweenScaleFunctions.Linear, tween2 =>
							{
								_progressBarImage.color = tween2.CurrentValue;
							}, tween2 =>
							{
								finished = true;
								if (i2 != 2)
								{
									return;
								}

								_progressBarImage.fillAmount = 0;
								_progressBarImage.color = new Color32(0, 255, 0, 80);
								CurrentExpText = $"0 / {ExperienceSystem.instance.TotalNeededExperienceForNextLevel}";
								CurrentLevelText = $"Level {SaveDataManager.instance.SaveData.level}";
								OnEndAnimated();
							});
					});
				yield return new WaitUntil(() => finished);
			}
		}

		[UIAction("switch-fightsabers-act")]
		public void ShowModPageClick()
		{
			if (_shouldOpenPage)
			{
				var currentFlow = Resources.FindObjectsOfTypeAll<FlowCoordinator>().FirstOrDefault(f => f.isActivated);
				FlowCoordinatorOwner = BeatSaberUI.CreateFlowCoordinator<FightSabersFlowCoordinator>();
				FlowCoordinatorOwner.oldCoordinator = currentFlow;
				currentFlow.PresentFlowCoordinator(FlowCoordinatorOwner);
			}
			else
			{
				FlowCoordinatorOwner.oldCoordinator.DismissFlowCoordinator(FlowCoordinatorOwner);
			}

			_shouldOpenPage = !_shouldOpenPage;
			buttonStatus = _shouldOpenPage ? "Open FightSabers" : "Close FightSabers";
		}
	}
}