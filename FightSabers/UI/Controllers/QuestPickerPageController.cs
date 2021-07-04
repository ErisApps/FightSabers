using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using FightSabers.Core;
using UnityEngine.UI;

namespace FightSabers.UI.Controllers
{
	[HotReload(RelativePathToLayout = @"..\Views\QuestPickerPageView.bsml")]
	[ViewDefinition("FightSabers.UI.Views.QuestPickerPageView.bsml")]
	internal class QuestPickerPageController : BSMLAutomaticViewController
	{
		public static QuestPickerPageController instance;

		[UIComponent("picker-quest-1-btn")]
		private Button? _pickerQuest1Btn = null;

		[UIComponent("picker-quest-2-btn")]
		private Button? _pickerQuest2Btn = null;

		[UIComponent("picker-quest-3-btn")]
		private Button? _pickerQuest3Btn = null;

		private string _questPickStatus = "Loading..";

		[UIValue("quest-pick-status")]
		public string QuestPickStatus
		{
			get => _questPickStatus;
			private set
			{
				_questPickStatus = value;
				NotifyPropertyChanged();
			}
		}

		private bool _pickerQuest1Active;

		[UIValue("picker-quest-1-active")]
		public bool pickerQuest1Active
		{
			get => _pickerQuest1Active;
			private set
			{
				_pickerQuest1Active = value;
				NotifyPropertyChanged();
			}
		}

		private bool _pickerQuest2Active;

		[UIValue("picker-quest-2-active")]
		public bool pickerQuest2Active
		{
			get => _pickerQuest2Active;
			private set
			{
				_pickerQuest2Active = value;
				NotifyPropertyChanged();
			}
		}

		private bool _pickerQuest3Active;

		[UIValue("picker-quest-3-active")]
		public bool pickerQuest3Active
		{
			get => _pickerQuest3Active;
			private set
			{
				_pickerQuest3Active = value;
				NotifyPropertyChanged();
			}
		}

		private string _titleQuest1 = "Quest not available";

		[UIValue("title-quest-1")]
		public string titleQuest1
		{
			get => _titleQuest1;
			private set
			{
				_titleQuest1 = value;
				NotifyPropertyChanged();
			}
		}

		private string _titleQuest2 = "Quest not available";

		[UIValue("title-quest-2")]
		public string titleQuest2
		{
			get => _titleQuest2;
			private set
			{
				_titleQuest2 = value;
				NotifyPropertyChanged();
			}
		}

		private string _titleQuest3 = "Quest not available";

		[UIValue("title-quest-3")]
		public string titleQuest3
		{
			get => _titleQuest3;
			private set
			{
				_titleQuest3 = value;
				NotifyPropertyChanged();
			}
		}

		private string _descQuest1 = "";

		[UIValue("desc-quest-1")]
		public string descQuest1
		{
			get => _descQuest1;
			private set
			{
				_descQuest1 = value;
				NotifyPropertyChanged();
			}
		}

		private string _descQuest2 = "";

		[UIValue("desc-quest-2")]
		public string descQuest2
		{
			get => _descQuest2;
			private set
			{
				_descQuest2 = value;
				NotifyPropertyChanged();
			}
		}

		private string _descQuest3 = "";

		[UIValue("desc-quest-3")]
		public string descQuest3
		{
			get => _descQuest3;
			private set
			{
				_descQuest3 = value;
				NotifyPropertyChanged();
			}
		}

		private string _expRewardQuest1 = "";

		[UIValue("exp-reward-quest-1")]
		public string expRewardQuest1
		{
			get => _expRewardQuest1;
			private set
			{
				_expRewardQuest1 = value;
				NotifyPropertyChanged();
			}
		}

		private string _expRewardQuest2 = "";

		[UIValue("exp-reward-quest-2")]
		public string expRewardQuest2
		{
			get => _expRewardQuest2;
			private set
			{
				_expRewardQuest2 = value;
				NotifyPropertyChanged();
			}
		}

		private string _expRewardQuest3 = "";

		[UIValue("exp-reward-quest-3")]
		public string expRewardQuest3
		{
			get => _expRewardQuest3;
			private set
			{
				_expRewardQuest3 = value;
				NotifyPropertyChanged();
			}
		}

		[UIAction("select-quest-1-act")]
		private void Quest1Selected()
		{
			QuestManager.instance.PickQuest(0);
			pickerQuest1Active = false;
			if (!QuestManager.instance.CanPickQuest)
			{
				ChangePickingStatus(false);
			}

			RefreshPickStatusText();
			RefreshPickableQuestContent();
		}

		[UIAction("select-quest-2-act")]
		private void Quest2Selected()
		{
			QuestManager.instance.PickQuest(1);
			pickerQuest2Active = false;
			if (!QuestManager.instance.CanPickQuest)
			{
				ChangePickingStatus(false);
			}

			RefreshPickStatusText();
			RefreshPickableQuestContent();
		}

		[UIAction("select-quest-3-act")]
		private void Quest3Selected()
		{
			QuestManager.instance.PickQuest(2);
			pickerQuest3Active = false;
			if (!QuestManager.instance.CanPickQuest)
			{
				ChangePickingStatus(false);
			}

			RefreshPickStatusText();
			RefreshPickableQuestContent();
		}

		public void ChangePickingStatus(bool status)
		{
			pickerQuest1Active = QuestManager.instance.PickableQuests.Count > 0 && QuestManager.instance.PickableQuests[0] != null && status;
			pickerQuest2Active = QuestManager.instance.PickableQuests.Count > 1 && QuestManager.instance.PickableQuests[1] != null && status;
			pickerQuest3Active = QuestManager.instance.PickableQuests.Count > 2 && QuestManager.instance.PickableQuests[2] != null && status;
		}

		public void RefreshPickableQuestContent()
		{
			for (var i = 0; i < 3; i++)
			{
				var pickableQuest = i >= QuestManager.instance.PickableQuests.Count ? null : QuestManager.instance.PickableQuests[i];
				switch (i)
				{
					case 0:
						titleQuest1 = pickableQuest != null ? pickableQuest.Title : "Quest not available";
						descQuest1 = pickableQuest != null ? pickableQuest.Description : "";
						expRewardQuest1 = pickableQuest != null ? $"{pickableQuest.ExpReward} EXP" : "";
						pickerQuest1Active = pickableQuest != null;
						break;
					case 1:
						titleQuest2 = pickableQuest != null ? pickableQuest.Title : "Quest not available";
						descQuest2 = pickableQuest != null ? pickableQuest.Description : "";
						expRewardQuest2 = pickableQuest != null ? $"{pickableQuest.ExpReward} EXP" : "";
						pickerQuest2Active = pickableQuest != null;
						break;
					case 2:
						titleQuest3 = pickableQuest != null ? pickableQuest.Title : "Quest not available";
						descQuest3 = pickableQuest != null ? pickableQuest.Description : "";
						expRewardQuest3 = pickableQuest != null ? $"{pickableQuest.ExpReward} EXP" : "";
						pickerQuest3Active = pickableQuest != null;
						break;
				}
			}
		}

		public void RefreshPickStatusText()
		{
			var pickableQuestAmount = 3 - QuestManager.instance.CurrentQuests.Count;
			QuestPickStatus = pickableQuestAmount <= 0
				? "<color=#e74c3c>Your quest list is full!</color>"
				: $"You can still pick <color=#ffa500ff>{pickableQuestAmount}</color> quest{(pickableQuestAmount != 1 ? "s" : "")}!";
		}

		protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
		{
			base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

			if (firstActivation)
			{
				instance = this;

				var iconImage = _pickerQuest1Btn.GetComponentsInChildren<Image>().FirstOrDefault(image => image != null && image.name == "Icon");
				if (_pickerQuest1Btn != null && iconImage)
				{
					iconImage.enabled = false;
				}

				iconImage = _pickerQuest2Btn.GetComponentsInChildren<Image>().FirstOrDefault(image => image != null && image.name == "Icon");
				if (_pickerQuest2Btn != null && iconImage)
				{
					iconImage.enabled = false;
				}

				iconImage = _pickerQuest3Btn.GetComponentsInChildren<Image>().FirstOrDefault(image => image != null && image.name == "Icon");
				if (_pickerQuest3Btn != null && iconImage)
				{
					iconImage.enabled = false;
				}

				RefreshPickableQuestContent();
				if (!QuestManager.instance.CanPickQuest)
				{
					ChangePickingStatus(false);
				}

				RefreshPickStatusText();
			}
		}
	}
}