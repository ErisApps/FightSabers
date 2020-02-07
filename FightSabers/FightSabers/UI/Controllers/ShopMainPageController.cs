using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using FightSabers.Core.ExclusiveContent;
using FightSabers.UI.FlowCoordinators;
using UnityEngine.UI;

namespace FightSabers.UI.Controllers
{
    internal class ShopMainPageController : FightSabersViewController
    {
        public override string ResourceName => "FightSabers.UI.Views.ShopMainPageView.bsml";
        public override string ContentFilePath => "D:\\Bibliotheques\\Documents\\GitHub\\FightSabers\\FightSabers\\FightSabers\\UI\\Views\\ShopMainPageView.bsml";

        [UIParams]
        private BSMLParserParams parserParams;

        #region Properties

        [UIComponent("sabers-btn")]    private Button _sabersButton;
        [UIComponent("platforms-btn")] private Button _platformsButton;
        [UIComponent("avatars-btn")]   private Button _avatarsButton;
        [UIComponent("notes-btn")]     private Button _notesButton;
        [UIComponent("walls-btn")]     private Button _wallsButton;
        [UIComponent("items-btn")]     private Button _itemsButton;

        #region Hover hints

        private string _sabersHoverHint;
        [UIValue("sabers-hover-hint")]
        public string SabersHoverHint
        {
            get { return _sabersHoverHint; }
            private set
            {
                _sabersHoverHint = value;
                NotifyPropertyChanged();
            }
        }

        private string _platformsHoverHint;
        [UIValue("platforms-hover-hint")]
        public string PlatformsHoverHint
        {
            get { return _platformsHoverHint; }
            private set
            {
                _platformsHoverHint = value;
                NotifyPropertyChanged();
            }
        }

        private string _avatarsHoverHint;
        [UIValue("avatars-hover-hint")]
        public string AvatarsHoverHint
        {
            get { return _avatarsHoverHint; }
            private set
            {
                _avatarsHoverHint = value;
                NotifyPropertyChanged();
            }
        }

        private string _notesHoverHint;
        [UIValue("notes-hover-hint")]
        public string NotesHoverHint
        {
            get { return _notesHoverHint; }
            private set
            {
                _notesHoverHint = value;
                NotifyPropertyChanged();
            }
        }

        private string _wallsHoverHint;
        [UIValue("walls-hover-hint")]
        public string WallsHoverHint
        {
            get { return _wallsHoverHint; }
            private set
            {
                _wallsHoverHint = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Buttons state

        private bool _sabersButtonState = true;
        [UIValue("sabers-btn-state")]
        public bool SabersButtonState
        {
            get { return _sabersButtonState; }
            private set
            {
                _sabersButtonState = value;
                NotifyPropertyChanged();
            }
        }

        private bool _platformsButtonState = true;
        [UIValue("platforms-btn-state")]
        public bool PlatformsButtonState
        {
            get { return _platformsButtonState; }
            private set
            {
                _platformsButtonState = value;
                NotifyPropertyChanged();
            }
        }

        private bool _avatarsButtonState = true;
        [UIValue("avatars-btn-state")]
        public bool AvatarsButtonState
        {
            get { return _avatarsButtonState; }
            private set
            {
                _avatarsButtonState = value;
                NotifyPropertyChanged();
            }
        }

        private bool _notesButtonState = true;
        [UIValue("notes-btn-state")]
        public bool NotesButtonState
        {
            get { return _notesButtonState; }
            private set
            {
                _notesButtonState = value;
                NotifyPropertyChanged();
            }
        }

        private bool _wallsButtonState = true;
        [UIValue("walls-btn-state")]
        public bool WallsButtonState
        {
            get { return _wallsButtonState; }
            private set
            {
                _wallsButtonState = value;
                NotifyPropertyChanged();
            }
        }

        #endregion
        #endregion

        [UIAction("sabers-btn-act")]
        private void ShowSaberShop()
        {
            flowCoordinatorOwner.ActivatePage(FightSabersFlowCoordinator.PageStatus.SaberShop);
        }

        [UIAction("platforms-btn-act")]
        private void ShowPlatformShop()
        {
            flowCoordinatorOwner.ActivatePage(FightSabersFlowCoordinator.PageStatus.PlatformShop);
        }

        [UIAction("avatars-btn-act")]
        private void ShowAvatarShop()
        {
            flowCoordinatorOwner.ActivatePage(FightSabersFlowCoordinator.PageStatus.AvatarShop);
        }

        [UIAction("notes-btn-act")]
        private void ShowNoteShop()
        {
            flowCoordinatorOwner.ActivatePage(FightSabersFlowCoordinator.PageStatus.NoteShop);
        }

        [UIAction("walls-btn-act")]
        private void ShopWallShop()
        {
            flowCoordinatorOwner.ActivatePage(FightSabersFlowCoordinator.PageStatus.WallShop);
        }

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            base.DidActivate(firstActivation, type);
            
            DisableBigButtonIcon(_sabersButton);
            DisableBigButtonIcon(_platformsButton);
            DisableBigButtonIcon(_avatarsButton);
            DisableBigButtonIcon(_notesButton);
            DisableBigButtonIcon(_wallsButton);
            DisableBigButtonIcon(_itemsButton);

            if (!ExclusiveSabersManager.CustomSaberInstalled)
            {
                SabersButtonState = false;
                SabersHoverHint = "Custom Sabers is not installed!";
            }
            if (!ExclusivePlatformsManager.CustomPlatformInstalled)
            {
                PlatformsButtonState = false;
                PlatformsHoverHint = "Custom Platforms is not installed!";
            }
            if (!ExclusiveAvatarsManager.CustomAvatarInstalled)
            {
                AvatarsButtonState = false;
                AvatarsHoverHint = "Custom Avatars is not installed!";
            }
            if (!ExclusiveNotesManager.CustomNoteInstalled)
            {
                NotesButtonState = false;
                NotesHoverHint = "Custom Notes is not installed!";
            }
            if (!ExclusiveWallsManager.CustomWallInstalled)
            {
                WallsButtonState = false;
                WallsHoverHint = "Custom Walls is not installed!";
            }
        }

        public static void DisableBigButtonIcon(Button btn)
        {
            if (btn == null) return;
            var iconImage = btn.GetComponentsInChildren<Image>().FirstOrDefault(image => image != null && image.name == "Icon");
            if (iconImage)
                iconImage.enabled = false;
        }
    }
}
