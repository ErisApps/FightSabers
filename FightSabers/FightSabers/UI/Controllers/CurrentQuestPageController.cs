using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BS_Utils.Utilities;
using FightSabers.Core;
using FightSabers.Models.Abstracts;
using FightSabers.Models.Interfaces;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FightSabers.UI.Controllers
{
    internal class CurrentQuestPageController : FightSabersViewController
    {
        public override string ResourceName    => "FightSabers.UI.Views.CurrentQuestPageView.bsml";
        public override string ContentFilePath => "D:\\Bibliotheques\\Documents\\GitHub\\FightSabers\\FightSabers\\FightSabers\\UI\\Views\\CurrentQuestPageView.bsml";

        public static CurrentQuestPageController instance;

        [UIParams] private BSMLParserParams parserParams;

        #region Properties
        #region Quest 1
        [UIComponent("progress-bar-quest-1-img")]
        private Image _progressBarImageQuest1;

        [UIComponent("progress-bg-bar-quest-1-img")]
        private Image _progressBgBarImageQuest1;

        [UIComponent("current-progress-quest-1-text")]
        private TextMeshProUGUI _currentProgressQuest1TextComp;

        private bool _quest1ContainerState;

        [UIValue("quest-1-container-state")]
        public bool Quest1ContainerState {
            get { return _quest1ContainerState; }
            private set {
                _quest1ContainerState = value;
                NotifyPropertyChanged();
            }
        }

        private string _titleQuest1Text = "";

        [UIValue("title-quest-1")]
        public string TitleQuest1Text {
            get { return _titleQuest1Text; }
            private set {
                _titleQuest1Text = value;
                NotifyPropertyChanged();
            }
        }

        private string _descQuest1Text = "";

        [UIValue("desc-quest-1")]
        public string DescQuest1Text {
            get { return _descQuest1Text; }
            private set {
                _descQuest1Text = value;
                NotifyPropertyChanged();
            }
        }

        private string _currentProgressQuest1Text = "";

        [UIValue("current-progress-quest-1")]
        public string CurrentProgressQuest1Text {
            get { return _currentProgressQuest1Text; }
            private set {
                _currentProgressQuest1Text = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
        #region Quest 2
        [UIComponent("progress-bar-quest-2-img")]
        private Image _progressBarImageQuest2;

        [UIComponent("progress-bg-bar-quest-2-img")]
        private Image _progressBgBarImageQuest2;

        [UIComponent("current-progress-quest-2-text")]
        private TextMeshProUGUI _currentProgressQuest2TextComp;

        private bool _quest2ContainerState;

        [UIValue("quest-2-container-state")]
        public bool Quest2ContainerState {
            get { return _quest2ContainerState; }
            private set {
                _quest2ContainerState = value;
                NotifyPropertyChanged();
            }
        }

        private string _titleQuest2Text = "";

        [UIValue("title-quest-2")]
        public string TitleQuest2Text {
            get { return _titleQuest2Text; }
            private set {
                _titleQuest2Text = value;
                NotifyPropertyChanged();
            }
        }

        private string _descQuest2Text = "";

        [UIValue("desc-quest-2")]
        public string DescQuest2Text {
            get { return _descQuest2Text; }
            private set {
                _descQuest2Text = value;
                NotifyPropertyChanged();
            }
        }

        private string _currentProgressQuest2Text = "";

        [UIValue("current-progress-quest-2")]
        public string CurrentProgressQuest2Text {
            get { return _currentProgressQuest2Text; }
            private set {
                _currentProgressQuest2Text = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
        #region Quest 3
        [UIComponent("progress-bar-quest-3-img")]
        private Image _progressBarImageQuest3;

        [UIComponent("progress-bg-bar-quest-3-img")]
        private Image _progressBgBarImageQuest3;

        [UIComponent("current-progress-quest-3-text")]
        private TextMeshProUGUI _currentProgressQuest3TextComp;

        private bool _quest3ContainerState;

        [UIValue("quest-3-container-state")]
        public bool Quest3ContainerState {
            get { return _quest3ContainerState; }
            private set {
                _quest3ContainerState = value;
                NotifyPropertyChanged();
            }
        }

        private string _titleQuest3Text = "";

        [UIValue("title-quest-3")]
        public string TitleQuest3Text {
            get { return _titleQuest3Text; }
            private set {
                _titleQuest3Text = value;
                NotifyPropertyChanged();
            }
        }

        private string _descQuest3Text = "";

        [UIValue("desc-quest-3")]
        public string DescQuest3Text {
            get { return _descQuest3Text; }
            private set {
                _descQuest3Text = value;
                NotifyPropertyChanged();
            }
        }

        private string _currentProgressQuest3Text = "";

        [UIValue("current-progress-quest-3")]
        public string CurrentProgressQuest3Text {
            get { return _currentProgressQuest3Text; }
            private set {
                _currentProgressQuest3Text = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
        #endregion

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            base.DidActivate(firstActivation, type);
            instance = this;
            if (firstActivation)
            {
                QuestManager.instance.QuestPicked += OnQuestPicked;
                QuestManager.instance.QuestCompleted += OnQuestCompleted;
            }
            RefreshWholeUI();
        }

        private void OnQuestPicked(object self)
        {
            RefreshWholeUI();
        }

        private void OnQuestCompleted(object self, Quest quest)
        {
            RefreshWholeUI();
        }

        private void RefreshWholeUI()
        {
            Logger.log.Debug($"QuestManager.instance.CurrentQuests.Count: {QuestManager.instance.CurrentQuests.Count}");
            for (var i = 0; i < 3; ++i)
            {
                if (QuestManager.instance.CurrentQuests.Count > i && QuestManager.instance.CurrentQuests[i] is IQuest quest)
                {
                    RefreshQuestUI(this.GetPrivateField<Image>($"_progressBgBarImageQuest{i + 1}"),
                                   this.GetPrivateField<Image>($"_progressBarImageQuest{i   + 1}"),
                                   i + 1, quest);
                }
                else
                    this.SetProperty($"Quest{i + 1}ContainerState", false);
            }
        }


        private void RefreshQuestUI([NotNull] Image progressBg, [NotNull] Image progressBar, int questIdx, [NotNull] IQuest quest)
        {
            if (progressBg  == null) throw new ArgumentNullException(nameof(progressBg));
            if (progressBar == null) throw new ArgumentNullException(nameof(progressBar));
            if (quest       == null) throw new ArgumentNullException(nameof(quest));
            if (questIdx    <= 0) throw new ArgumentOutOfRangeException(nameof(questIdx));
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
            this.SetProperty($"TitleQuest{questIdx}Text", quest.title);
            //Description
            this.SetProperty($"DescQuest{questIdx}Text", quest.description);
            //Current progress
            this.SetProperty($"CurrentProgressQuest{questIdx}Text", quest.progressHint);
            //UI state
            this.SetProperty($"Quest{questIdx}ContainerState", true);
        }
    }
}