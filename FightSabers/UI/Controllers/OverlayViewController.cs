using System;
using System.Collections;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.ViewControllers;
using DigitalRuby.Tween;
using FightSabers.Core;
using FightSabers.Settings;
using FightSabers.UI.FlowCoordinators;
using FightSabers.Utilities;
using IPA.Loader;
using IPA.Utilities;
using SiraUtil.Tools;
using TMPro;
using UnityEngine;
using VRUIControls;
using Zenject;
using Image = UnityEngine.UI.Image;

namespace FightSabers.UI.Controllers
{
	[HotReload(RelativePathToLayout = @"..\Views\OverlayView.bsml")]
	[ViewDefinition("FightSabers.UI.Views.OverlayView.bsml")]
	internal class OverlayViewController : BSMLAutomaticViewController, IDisposable
	{
		private static readonly FieldAccessor<VRGraphicRaycaster, PhysicsRaycasterWithCache>.Accessor PhysicsRaycaster = FieldAccessor<VRGraphicRaycaster, PhysicsRaycasterWithCache>.GetAccessor("_physicsRaycaster");
		private static readonly FieldAccessor<LazyInject<FightSabersFlowCoordinator>, bool>.Accessor LazyInjectHasValueAccessor = FieldAccessor<LazyInject<FightSabersFlowCoordinator>, bool>.GetAccessor("_hasValue");

		private SiraLog _logger = null!;
		private PluginConfig _config = null!;
		private PluginMetadata _pluginMetadata = null!;
		private SaveDataManager _saveDataManager = null!;
		private ExperienceSystem _experienceSystem = null!;
		private LazyInject<FightSabersFlowCoordinator> _fightSabersFlowCoordinator = null!;

		private FloatingScreen _floatingScreen = null!;

		private bool IsFightSabersFlowCoordinatorOpen => LazyInjectHasValueAccessor(ref _fightSabersFlowCoordinator) && _fightSabersFlowCoordinator.Value.YoungestChildFlowCoordinatorOrSelf() is FightSabersFlowCoordinator;

		[Inject]
		internal void Construct(SiraLog logger, PluginConfig config, [Inject(Id = Constants.BindingIds.METADATA)] PluginMetadata pluginMetadata, SaveDataManager saveDataManager,
			ExperienceSystem experienceSystem, LazyInject<FightSabersFlowCoordinator> fightSabersFlowCoordinator, PhysicsRaycasterWithCache physicsRaycasterWithCache)
		{
			_logger = logger;
			_config = config;
			_pluginMetadata = pluginMetadata;
			_saveDataManager = saveDataManager;
			_experienceSystem = experienceSystem;
			_fightSabersFlowCoordinator = fightSabersFlowCoordinator;

			_config.ConfigChanged -= ConfigOnConfigChanged;
			_config.ConfigChanged += ConfigOnConfigChanged;

			_floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(120, 52f), true, config.PanelPosition, Quaternion.Euler(config.PanelRotation));
			var graphicRaycaster = _floatingScreen.GetComponent<VRGraphicRaycaster>();
			PhysicsRaycaster(ref graphicRaycaster) = physicsRaycasterWithCache;
			_floatingScreen.SetRootViewController(this, AnimationType.None);

			_floatingScreen.HandleReleased -= FloatingScreenOnHandleReleased;
			_floatingScreen.HandleReleased += FloatingScreenOnHandleReleased;
		}

		public void Dispose()
		{
			_floatingScreen.HandleReleased -= FloatingScreenOnHandleReleased;
			_config.ConfigChanged -= ConfigOnConfigChanged;
		}

		[UIComponent("progress-bar-img")]
		internal Image ProgressBarImage = null!;

		[UIComponent("progress-bg-bar-img")]
		internal Image ProgressBgBarImage = null!;

		[UIObject("main-background")]
		internal GameObject MainBackground = null!;

		[UIObject("progress-bar-stack")]
		internal GameObject ProgressBarStack = null!;


		[UIValue("experience-container-state")]
		public bool ExperienceContainerState => _config.Enabled;

		[UIValue("fs-disable-container-state")]
		public bool FsDisableContainerState => !_config.Enabled;


		[UIValue("fightsabers-btn-status")]
		public string ButtonStatus { get; private set; } = "Open FightSabers";

		[UIValue("header-text")]
		public string HeaderText { get; private set; } = string.Empty;

		[UIValue("current-level")]
		internal string CurrentLevelText { get; private set; } = string.Empty;

		[UIComponent("current-exp-text")]
		internal TextMeshProUGUI CurrentExpTextComp = null!;

		[UIValue("current-exp")]
		internal string CurrentExpText { get; set; } = string.Empty;

		public delegate void BarAnimatedHandler(object self, bool state);

		public event BarAnimatedHandler? BeginAnimated;

		public event BarAnimatedHandler? EndAnimated;

		public float ProgressSpeed => 2.5f;

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

			if (firstActivation)
			{
				_experienceSystem.LeveledUp += OnLeveledUp;
			}

			//ProgressBar
			var tex = Texture2D.whiteTexture;
			var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f, 100, 1);
			ProgressBgBarImage.sprite = sprite;
			ProgressBgBarImage.color = new Color(0, 0, 0, 0.85f);
			ProgressBarImage.sprite = sprite;
			ProgressBarImage.type = Image.Type.Filled;
			ProgressBarImage.fillAmount = 0f;
			ProgressBarImage.fillMethod = Image.FillMethod.Horizontal;
			ProgressBarImage.color = new Color32(0, 255, 0, 80);
			ProgressBarImage.material = null;
			//Header text
			HeaderText = $"{_pluginMetadata.Name} v{_pluginMetadata.Version}";
			//Level text
			CurrentLevelText = $"Level {_saveDataManager.SaveData.level}";
			//Current exp
			CurrentExpText = $"0 / {_experienceSystem.TotalNeededExperienceForNextLevel}";
			new UnityTask(FillExperienceBar(0, _saveDataManager.SaveData.currentExp, 3.5f));
		}

		public IEnumerator FillExperienceBar(uint currentExp, uint toExp, float delayBefore = 0f)
		{
			OnBeginAnimated();
			yield return new WaitForSeconds(delayBefore);
			gameObject.Tween("FillExpBar" + gameObject.GetInstanceID(), currentExp, toExp,
				(_experienceSystem.GetPercentageForExperience(toExp) - _experienceSystem.GetPercentageForExperience(currentExp)) * ProgressSpeed,
				TweenScaleFunctions.SineEaseIn, tween =>
				{
					CurrentExpText = $"{(uint) tween.CurrentValue} / {_experienceSystem.TotalNeededExperienceForNextLevel}";
					ProgressBarImage.fillAmount = _experienceSystem.GetPercentageForExperiencePrecise(tween.CurrentValue);
				}, _ =>
				{
					OnEndAnimated();
				});
		}

		private void LevelUpAnimation()
		{
			OnBeginAnimated();
			if (!CurrentExpTextComp)
			{
				_logger.Warning("Current experience text component was null, definitely not expected so skipping the animation");
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
			CurrentExpTextComp.transform.SetParent(MainBackground.transform);
			var localPosition = CurrentExpTextComp.transform.localPosition;
			gameObject.Tween("FloatingTextPosition" + gameObject.GetInstanceID(), localPosition,
				localPosition + new Vector3(0, 0, -10f), 2.25f,
				TweenScaleFunctions.SineEaseOut, tween =>
				{
					CurrentExpTextComp.transform.localPosition = tween.CurrentValue;
				}, _ =>
				{
					gameObject.Tween("FloatingTextPosition" + gameObject.GetInstanceID(), CurrentExpTextComp.transform.localPosition,
						CurrentExpTextComp.transform.localPosition + new Vector3(0, 0, 10), 2.25f, TweenScaleFunctions.SineEaseIn, tween2 =>
						{
							CurrentExpTextComp.transform.localPosition = tween2.CurrentValue;
						}, _ =>
						{
							CurrentExpTextComp.transform.SetParent(ProgressBarStack.transform);
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
						CurrentExpTextComp.transform.localRotation = tween.CurrentValue;
					}, _ =>
					{
						gameObject.Tween("FloatingTextRotation" + gameObject.GetInstanceID(), Quaternion.Euler(0, 0, -20),
							Quaternion.Euler(0, 0, 20), 0.35f, TweenScaleFunctions.SineEaseIn, tween2 =>
							{
								CurrentExpTextComp.transform.localRotation = tween2.CurrentValue;
							}, _ =>
							{
								finished = true;
							});
					});
				yield return new WaitUntil(() => finished);
			}

			gameObject.Tween("FloatingTextRotation" + gameObject.GetInstanceID(), Quaternion.Euler(0, 0, 20),
				Quaternion.Euler(0, 0, 0), 0.175f, TweenScaleFunctions.SineEaseOut, tween =>
				{
					CurrentExpTextComp.transform.localRotation = tween.CurrentValue;
				});
		}

		private void AnimateFontSize()
		{
			gameObject.Tween("FloatingTextSize" + gameObject.GetInstanceID(), 4.5f, 9f, 2.25f, TweenScaleFunctions.SineEaseOut, tween =>
			{
				CurrentExpTextComp.fontSize = tween.CurrentValue;
			}, tween =>
			{
				gameObject.Tween("FloatingTextSize" + gameObject.GetInstanceID(), 9f, 4.5f, 2.25f, TweenScaleFunctions.SineEaseIn, tween2 =>
				{
					CurrentExpTextComp.fontSize = tween2.CurrentValue;
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
						ProgressBarImage.color = tween.CurrentValue;
					}, _ =>
					{
						var i2 = i1;
						gameObject.Tween("LevelUpColor" + gameObject.GetInstanceID(), new Color32(255, 255, 0, 120), new Color32(232, 126, 4, 80), 0.75f,
							TweenScaleFunctions.Linear, tween2 =>
							{
								ProgressBarImage.color = tween2.CurrentValue;
							}, _ =>
							{
								finished = true;
								if (i2 != 2)
								{
									return;
								}

								ProgressBarImage.fillAmount = 0;
								ProgressBarImage.color = new Color32(0, 255, 0, 80);
								CurrentExpText = $"0 / {_experienceSystem.TotalNeededExperienceForNextLevel}";
								CurrentLevelText = $"Level {_saveDataManager.SaveData.level}";
								OnEndAnimated();
							});
					});
				yield return new WaitUntil(() => finished);
			}
		}

		[UIAction("switch-fightsabers-act")]
		public void ShowModPageClick()
		{
			if (IsFightSabersFlowCoordinatorOpen)
			{
				BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(_fightSabersFlowCoordinator.Value);
				ButtonStatus = "Close FightSabers";
			}
			else
			{
				BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(_fightSabersFlowCoordinator.Value);
				ButtonStatus = "Open FightSabers";
			}
		}

		private void ConfigOnConfigChanged(object sender, EventArgs e)
		{
			NotifyPropertyChanged(nameof(ExperienceContainerState));
			NotifyPropertyChanged(nameof(FsDisableContainerState));
		}

		private void FloatingScreenOnHandleReleased(object sender, FloatingScreenHandleEventArgs e)
		{
			_config.PanelPosition = e.Position;
			_config.PanelRotation = e.Rotation.eulerAngles;
		}
	}
}