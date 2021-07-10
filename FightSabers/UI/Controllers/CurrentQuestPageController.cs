using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using FightSabers.Core;
using FightSabers.Models;
using FightSabers.Models.Abstracts;
using FightSabers.Models.Interfaces;
using IPA.Utilities;
using JetBrains.Annotations;
using SiraUtil.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FightSabers.UI.Controllers
{
	[HotReload(RelativePathToLayout = @"..\Views\CurrentQuestPageView.bsml")]
	[ViewDefinition("FightSabers.UI.Views.CurrentQuestPageView.bsml")]
	internal class CurrentQuestPageController : BSMLAutomaticViewController, IDisposable
	{
		private SiraLog _logger = null!;
		private QuestManager _questManager = null!;

		[Inject]
		internal void Construct(SiraLog logger, QuestManager questManager)
		{
			_logger = logger;
			_questManager = questManager;

			_questManager.QuestPicked -= OnQuestPicked;
			_questManager.QuestPicked += OnQuestPicked;
			_questManager.QuestCanceled -= OnQuestCanceled;
			_questManager.QuestCanceled += OnQuestCanceled;
			_questManager.QuestCompleted -= OnQuestCompleted;
			_questManager.QuestCompleted += OnQuestCompleted;
			_questManager.QuestProgressChanged -= OnQuestProgressChanged;
			_questManager.QuestProgressChanged += OnQuestProgressChanged;
		}

		public void Dispose()
		{
			_questManager.QuestPicked -= OnQuestPicked;
			_questManager.QuestCanceled -= OnQuestCanceled;
			_questManager.QuestCompleted -= OnQuestCompleted;
			_questManager.QuestProgressChanged -= OnQuestProgressChanged;
		}

		// TODO: Find a better way to infer usage without pragma disables of CS0414
		[UIComponent("progress-bar-quest-1-img")]
#pragma warning disable 414
		internal Image ProgressBarImageQuest1 = null!;
#pragma warning restore 414

		[UIComponent("progress-bg-bar-quest-1-img")]
#pragma warning disable 414
		internal Image ProgressBgBarImageQuest1 = null!;
#pragma warning restore 414

		[UIComponent("current-progress-quest-1-text")]
#pragma warning disable 414
		internal TextMeshProUGUI CurrentProgressQuest1TextComp = null!;
#pragma warning restore 414

		[UIValue("quest-1-container-state")]
		internal bool Quest1ContainerState { get; set; }

		[UIValue("title-quest-1")]
		internal string TitleQuest1Text { get; set; } = string.Empty;

		[UIValue("desc-quest-1")]
		internal string DescQuest1Text { get; set; } = string.Empty;

		[UIValue("current-progress-quest-1")]
		internal string CurrentProgressQuest1Text { get; set; } = string.Empty;

		[UIValue("hover-quest-1-progress")]
		internal string HoverQuest1Progress { get; set; } = string.Empty;


		[UIComponent("progress-bar-quest-2-img")]
#pragma warning disable 414
		internal Image ProgressBarImageQuest2 = null!;
#pragma warning restore 414

		[UIComponent("progress-bg-bar-quest-2-img")]
#pragma warning disable 414
		internal Image ProgressBgBarImageQuest2 = null!;
#pragma warning restore 414

		[UIComponent("current-progress-quest-2-text")]
#pragma warning disable 414
		internal TextMeshProUGUI CurrentProgressQuest2TextComp = null!;
#pragma warning restore 414

		[UIValue("quest-2-container-state")]
		internal bool Quest2ContainerState { get; set; }

		[UIValue("title-quest-2")]
		internal string TitleQuest2Text { get; set; } = string.Empty;

		[UIValue("desc-quest-2")]
		internal string DescQuest2Text { get; set; } = string.Empty;

		[UIValue("current-progress-quest-2")]
		internal string CurrentProgressQuest2Text { get; set; } = string.Empty;

		[UIValue("hover-quest-2-progress")]
		internal string HoverQuest2Progress { get; set; } = string.Empty;


		[UIComponent("progress-bar-quest-3-img")]
#pragma warning disable 414
		internal Image ProgressBarImageQuest3 = null!;
#pragma warning restore 414

		[UIComponent("progress-bg-bar-quest-3-img")]
#pragma warning disable 414
		internal Image ProgressBgBarImageQuest3= null!;
#pragma warning restore 414

		[UIComponent("current-progress-quest-3-text")]
#pragma warning disable 414
		internal TextMeshProUGUI CurrentProgressQuest3TextComp = null!;
#pragma warning restore 414

		[UIValue("quest-3-container-state")]
		internal bool Quest3ContainerState { get; set; }

		[UIValue("title-quest-3")]
		internal string TitleQuest3Text { get; set; } = string.Empty;

		[UIValue("desc-quest-3")]
		internal string DescQuest3Text { get; set; } = string.Empty;

		[UIValue("current-progress-quest-3")]
		internal string CurrentProgressQuest3Text { get; set; } = string.Empty;

		[UIValue("hover-quest-3-progress")]
		internal string HoverQuest3Progress { get; set; } = string.Empty;


		[UIAction("quest-1-cancel-act")]
		internal void Quest1Canceled()
		{
			_questManager.CancelQuest(0);
		}

		[UIAction("quest-2-cancel-act")]
		internal void Quest2Canceled()
		{
			_questManager.CancelQuest(1);
		}

		[UIAction("quest-3-cancel-act")]
		internal void Quest3Canceled()
		{
			_questManager.CancelQuest(2);
		}

		protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
		{
			base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

			RefreshWholeUI();
		}

		private void OnQuestPicked(object self)
		{
			RefreshWholeUI();
		}

		private void OnQuestCanceled(object self, Quest quest)
		{
			RefreshWholeUI();
		}

		private void OnQuestCompleted(object self, Quest quest)
		{
			RefreshWholeUI();
		}

		private void OnQuestProgressChanged(object self, Quest quest)
		{
			RefreshWholeUI();
		}

		private void RefreshWholeUI()
		{
			if (Plugin.CurrentSceneState == SceneState.Game)
			{
				return;
			}

			_logger.Debug($"CurrentQuests.Count: {_questManager.CurrentQuests.Count.ToString()}");
			for (var i = 0; i < 3; ++i)
			{
				if (_questManager.CurrentQuests.Count > i && _questManager.CurrentQuests[i] is { } quest)
				{
					RefreshQuestUI(this.GetField<Image, CurrentQuestPageController>($"_progressBgBarImageQuest{(i + 1).ToString()}"),
						this.GetField<Image, CurrentQuestPageController>($"_progressBarImageQuest{(i + 1).ToString()}"),
						i + 1, quest);
				}
				else
				{
					this.SetProperty($"Quest{(i + 1).ToString()}ContainerState", false);
				}
			}
		}

		private void RefreshQuestUI([NotNull] Image progressBg, [NotNull] Image progressBar, int questIdx, [NotNull] IQuest quest)
		{
			if (progressBg == null)
			{
				throw new ArgumentNullException(nameof(progressBg));
			}

			if (progressBar == null)
			{
				throw new ArgumentNullException(nameof(progressBar));
			}

			if (quest == null)
			{
				throw new ArgumentNullException(nameof(quest));
			}

			if (questIdx <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(questIdx));
			}

			//ProgressBar
			var tex = Texture2D.whiteTexture;
			var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f, 100, 1);
			progressBg.sprite = sprite;
			progressBg.color = new Color(0, 0, 0, 0.85f);
			progressBar.sprite = sprite;
			progressBar.type = Image.Type.Filled;
			progressBar.fillAmount = quest.Progress;
			progressBar.fillMethod = Image.FillMethod.Horizontal;
			progressBar.color = new Color32(0, 255, 0, 80);
			progressBar.material = null;

			//Title
			this.SetProperty($"TitleQuest{questIdx.ToString()}Text", quest.Title);
			this.SetProperty($"DescQuest{questIdx.ToString()}Text", quest.Description);
			this.SetProperty($"CurrentProgressQuest{questIdx.ToString()}Text", quest.ProgressHint);
			this.SetProperty($"HoverQuest{questIdx.ToString()}Progress", $"Reward: <color=#FFA500>{quest.ExpReward.ToString()} EXP</color>");

			//UI state
			this.SetProperty($"Quest{questIdx.ToString()}ContainerState", true);
		}
	}
}