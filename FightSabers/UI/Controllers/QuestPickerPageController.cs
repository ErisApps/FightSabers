using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using FightSabers.Core;
using UnityEngine.UI;
using Zenject;

namespace FightSabers.UI.Controllers
{
	[HotReload(RelativePathToLayout = @"..\Views\QuestPickerPageView.bsml")]
	[ViewDefinition("FightSabers.UI.Views.QuestPickerPageView.bsml")]
	internal class QuestPickerPageController : BSMLAutomaticViewController
	{
		private QuestManager _questManager = null!;

		[Inject]
		internal void Construct(QuestManager questManager)
		{
			_questManager = questManager;
		}

		[UIComponent("picker-quest-1-btn")]
		internal Button PickerQuest1Btn = null!;

		[UIComponent("picker-quest-2-btn")]
		internal Button PickerQuest2Btn = null!;

		[UIComponent("picker-quest-3-btn")]
		internal Button PickerQuest3Btn = null!;


		[UIValue("quest-pick-status")]
		internal string QuestPickStatus { get; set; } = "Loading..";


		[UIValue("picker-quest-1-active")]
		internal bool PickerQuest1Active { get; set; }

		[UIValue("picker-quest-2-active")]
		internal bool PickerQuest2Active { get; set; }

		[UIValue("picker-quest-3-active")]
		internal bool PickerQuest3Active { get; set; }


		[UIValue("title-quest-1")]
		internal string TitleQuest1 { get; set; } = "Quest not available";

		[UIValue("title-quest-2")]
		internal string TitleQuest2 { get; set; } = "Quest not available";

		[UIValue("title-quest-3")]
		internal string TitleQuest3 { get; set; } = "Quest not available";


		[UIValue("desc-quest-1")]
		internal string DescQuest1 { get; set; } = string.Empty;

		[UIValue("desc-quest-2")]
		internal string DescQuest2 { get; set; } = string.Empty;

		[UIValue("desc-quest-3")]
		internal string DescQuest3 { get; set; } = string.Empty;


		[UIValue("exp-reward-quest-1")]
		internal string ExpRewardQuest1 { get; set; } = string.Empty;

		[UIValue("exp-reward-quest-2")]
		internal string ExpRewardQuest2 { get; set; } = string.Empty;

		[UIValue("exp-reward-quest-3")]
		internal string ExpRewardQuest3 { get; set; } = string.Empty;


		[UIAction("select-quest-1-act")]
		private void Quest1Selected()
		{
			_questManager.PickQuest(0);
			PickerQuest1Active = false;
			if (!_questManager.CanPickQuest)
			{
				ChangePickingStatus(false);
			}

			RefreshPickStatusText();
			RefreshPickableQuestContent();
		}

		[UIAction("select-quest-2-act")]
		private void Quest2Selected()
		{
			_questManager.PickQuest(1);
			PickerQuest2Active = false;
			if (!_questManager.CanPickQuest)
			{
				ChangePickingStatus(false);
			}

			RefreshPickStatusText();
			RefreshPickableQuestContent();
		}

		[UIAction("select-quest-3-act")]
		private void Quest3Selected()
		{
			_questManager.PickQuest(2);
			PickerQuest3Active = false;
			if (!_questManager.CanPickQuest)
			{
				ChangePickingStatus(false);
			}

			RefreshPickStatusText();
			RefreshPickableQuestContent();
		}

		public void ChangePickingStatus(bool status)
		{
			PickerQuest1Active = _questManager.PickableQuests.Count > 0 && _questManager.PickableQuests[0] != null && status;
			PickerQuest2Active = _questManager.PickableQuests.Count > 1 && _questManager.PickableQuests[1] != null && status;
			PickerQuest3Active = _questManager.PickableQuests.Count > 2 && _questManager.PickableQuests[2] != null && status;
		}

		public void RefreshPickableQuestContent()
		{
			for (var i = 0; i < 3; i++)
			{
				var pickableQuest = i >= _questManager.PickableQuests.Count ? null : _questManager.PickableQuests[i];
				switch (i)
				{
					case 0:
						TitleQuest1 = pickableQuest != null ? pickableQuest.Title : "Quest not available";
						DescQuest1 = pickableQuest != null ? pickableQuest.Description : string.Empty;
						ExpRewardQuest1 = pickableQuest != null ? $"{pickableQuest.ExpReward} EXP" : string.Empty;
						PickerQuest1Active = pickableQuest != null;
						break;
					case 1:
						TitleQuest2 = pickableQuest != null ? pickableQuest.Title : "Quest not available";
						DescQuest2 = pickableQuest != null ? pickableQuest.Description : string.Empty;
						ExpRewardQuest2 = pickableQuest != null ? $"{pickableQuest.ExpReward} EXP" : string.Empty;
						PickerQuest2Active = pickableQuest != null;
						break;
					case 2:
						TitleQuest3 = pickableQuest != null ? pickableQuest.Title : "Quest not available";
						DescQuest3 = pickableQuest != null ? pickableQuest.Description : string.Empty;
						ExpRewardQuest3 = pickableQuest != null ? $"{pickableQuest.ExpReward} EXP" : string.Empty;
						PickerQuest3Active = pickableQuest != null;
						break;
				}
			}
		}

		public void RefreshPickStatusText()
		{
			var pickableQuestAmount = 3 - _questManager.CurrentQuests.Count;
			QuestPickStatus = pickableQuestAmount <= 0
				? "<color=#e74c3c>Your quest list is full!</color>"
				: $"You can still pick <color=#ffa500ff>{pickableQuestAmount}</color> quest{(pickableQuestAmount != 1 ? "s" : "")}!";
		}

		protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
		{
			base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

			if (firstActivation)
			{
				var iconImage = PickerQuest1Btn.GetComponentsInChildren<Image>().FirstOrDefault(image => image != null && image.name == "Icon");
				if (PickerQuest1Btn != null && iconImage != null)
				{
					iconImage.enabled = false;
				}

				iconImage = PickerQuest2Btn.GetComponentsInChildren<Image>().FirstOrDefault(image => image != null && image.name == "Icon");
				if (PickerQuest2Btn != null &&  iconImage != null)
				{
					iconImage.enabled = false;
				}

				iconImage = PickerQuest3Btn.GetComponentsInChildren<Image>().FirstOrDefault(image => image != null && image.name == "Icon");
				if (PickerQuest3Btn != null && iconImage != null)
				{
					iconImage.enabled = false;
				}

				RefreshPickableQuestContent();
				if (!_questManager.CanPickQuest)
				{
					ChangePickingStatus(false);
				}

				RefreshPickStatusText();
			}
		}
	}
}