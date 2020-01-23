using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using FightSabers.Core;
using UnityEngine.UI;

namespace FightSabers.UI.Controllers
{
    internal class QuestPickerPageController : FightSabersViewController
    {
        public override string ResourceName    => "FightSabers.UI.Views.QuestPickerPageView.bsml";
        public override string ContentFilePath => "D:\\Bibliotheques\\Documents\\GitHub\\FightSabers\\FightSabers\\FightSabers\\UI\\Views\\QuestPickerPageView.bsml";

        [UIParams] private BSMLParserParams parserParams;

        #region Properties
        [UIComponent("picker-quest-1-btn")] private Button _pickerQuest1Btn;
        [UIComponent("picker-quest-2-btn")] private Button _pickerQuest2Btn;
        [UIComponent("picker-quest-3-btn")] private Button _pickerQuest3Btn;

        #region Interact states
        private bool _pickerQuest1Active;

        [UIValue("picker-quest-1-active")]
        public bool pickerQuest1Active {
            get { return _pickerQuest1Active; }
            private set {
                _pickerQuest1Active = value;
                NotifyPropertyChanged();
            }
        }

        private bool _pickerQuest2Active;

        [UIValue("picker-quest-2-active")]
        public bool pickerQuest2Active {
            get { return _pickerQuest2Active; }
            private set {
                _pickerQuest2Active = value;
                NotifyPropertyChanged();
            }
        }

        private bool _pickerQuest3Active;

        [UIValue("picker-quest-3-active")]
        public bool pickerQuest3Active {
            get { return _pickerQuest3Active; }
            private set {
                _pickerQuest3Active = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Titles
        private string _titleQuest1 = "None";

        [UIValue("title-quest-1")]
        public string titleQuest1 {
            get { return _titleQuest1; }
            private set {
                _titleQuest1 = value;
                NotifyPropertyChanged();
            }
        }

        private string _titleQuest2 = "None";

        [UIValue("title-quest-2")]
        public string titleQuest2 {
            get { return _titleQuest2; }
            private set {
                _titleQuest2 = value;
                NotifyPropertyChanged();
            }
        }

        private string _titleQuest3 = "None";

        [UIValue("title-quest-3")]
        public string titleQuest3 {
            get { return _titleQuest3; }
            private set {
                _titleQuest3 = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Descriptions
        private string _descQuest1 = "None";

        [UIValue("desc-quest-1")]
        public string descQuest1 {
            get { return _descQuest1; }
            private set {
                _descQuest1 = value;
                NotifyPropertyChanged();
            }
        }

        private string _descQuest2 = "None";

        [UIValue("desc-quest-2")]
        public string descQuest2 {
            get { return _descQuest2; }
            private set {
                _descQuest2 = value;
                NotifyPropertyChanged();
            }
        }

        private string _descQuest3 = "None";

        [UIValue("desc-quest-3")]
        public string descQuest3 {
            get { return _descQuest3; }
            private set {
                _descQuest3 = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Exp rewards
        private string _expRewardQuest1 = "None";

        [UIValue("exp-reward-quest-1")]
        public string expRewardQuest1 {
            get { return _expRewardQuest1; }
            private set {
                _expRewardQuest1 = value;
                NotifyPropertyChanged();
            }
        }

        private string _expRewardQuest2 = "None";

        [UIValue("exp-reward-quest-2")]
        public string expRewardQuest2 {
            get { return _expRewardQuest2; }
            private set {
                _expRewardQuest2 = value;
                NotifyPropertyChanged();
            }
        }

        private string _expRewardQuest3 = "None";

        [UIValue("exp-reward-quest-3")]
        public string expRewardQuest3 {
            get { return _expRewardQuest3; }
            private set {
                _expRewardQuest3 = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
        #endregion

        [UIAction("select-quest-1-act")]
        private void Quest1Selected()
        {
            QuestManager.instance.PickQuest(0);
            pickerQuest1Active = false;
            if (!QuestManager.instance.CanPickQuest)
                ChangePickingStatus(false);
        }

        [UIAction("select-quest-2-act")]
        private void Quest2Selected()
        {
            QuestManager.instance.PickQuest(1);
            pickerQuest2Active = false;
            if (!QuestManager.instance.CanPickQuest)
                ChangePickingStatus(false);
        }

        [UIAction("select-quest-3-act")]
        private void Quest3Selected()
        {
            QuestManager.instance.PickQuest(2);
            pickerQuest3Active = false;
            if (!QuestManager.instance.CanPickQuest)
                ChangePickingStatus(false);
        }

        private void ChangePickingStatus(bool status)
        {
            pickerQuest1Active = pickerQuest2Active = pickerQuest3Active = status;
        }

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            base.DidActivate(firstActivation, type);

            if (firstActivation)
            {
                var iconImage = _pickerQuest1Btn.GetComponentsInChildren<Image>().FirstOrDefault(image => image != null && image.name == "Icon");
                if (_pickerQuest1Btn != null && iconImage)
                    iconImage.enabled = false;
                iconImage = _pickerQuest2Btn.GetComponentsInChildren<Image>().FirstOrDefault(image => image != null && image.name == "Icon");
                if (_pickerQuest2Btn != null && iconImage)
                    iconImage.enabled = false;
                iconImage = _pickerQuest3Btn.GetComponentsInChildren<Image>().FirstOrDefault(image => image != null && image.name == "Icon");
                if (_pickerQuest3Btn != null && iconImage)
                    iconImage.enabled = false;
                for (var i = 0; i < QuestManager.instance.PickableQuests.Count; i++)
                {
                    var pickableQuest = QuestManager.instance.PickableQuests[i];
                    if (pickableQuest == null) continue;
                    switch (i)
                    {
                        case 0:
                            titleQuest1 = pickableQuest.title;
                            descQuest1 = pickableQuest.description;
                            expRewardQuest1 = $"{pickableQuest.expReward} EXP";
                            pickerQuest1Active = true;
                            break;
                        case 1:
                            titleQuest2 = pickableQuest.title;
                            descQuest2 = pickableQuest.description;
                            expRewardQuest2 = $"{pickableQuest.expReward} EXP";
                            pickerQuest2Active = true;
                            break;
                        case 2:
                            titleQuest3 = pickableQuest.title;
                            descQuest3 = pickableQuest.description;
                            expRewardQuest3 = $"{pickableQuest.expReward} EXP";
                            pickerQuest3Active = true;
                            break;
                    }
                }
                if (!QuestManager.instance.CanPickQuest)
                    ChangePickingStatus(false);
            }
        }
    }
}