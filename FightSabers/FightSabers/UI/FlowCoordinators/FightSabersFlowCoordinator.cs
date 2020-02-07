using System;
using BeatSaberMarkupLanguage;
using FightSabers.Models.Interfaces;
using FightSabers.UI.Controllers;
using HMUI;

namespace FightSabers.UI.FlowCoordinators
{
    internal class FightSabersFlowCoordinator : FlowCoordinator
    {
        public enum PageStatus
        {
            Home,
            Skills,
            Profile,
            Quests,
            Statistics,
            Shop,
            SaberShop,
            PlatformShop,
            AvatarShop,
            NoteShop,
            WallShop,
            ItemShop
        }

        public FlowCoordinator      oldCoordinator;
        public BottomPageController bottomController;

        public PageStatus CurrentPageStatus { get; private set; }

        private FightSabersViewController _currentController;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (activationType != ActivationType.AddedToHierarchy)
                return;
            CurrentPageStatus = PageStatus.Home;
            var homeController = BeatSaberUI.CreateViewController<HomePageController>();
            bottomController = BeatSaberUI.CreateViewController<BottomPageController>();
            bottomController.flowCoordinatorOwner = homeController.flowCoordinatorOwner = this;
            ProvideInitialViewControllers(homeController, null, null, bottomController);
        }

        public void DisplayShopItemPreview(bool state, IRewardItem rewardItem = null)
        {
            if (state)
            {
                var sippc = BeatSaberUI.CreateViewController<ShopItemPreviewPageController>();
                sippc.OpenPreview(rewardItem);
                SetRightScreenViewController(sippc);
            }
            else
                SetRightScreenViewController(null);
        }

        public void ActivatePage(PageStatus status)
        {
            if (status == CurrentPageStatus) return;
            switch (status)
            {
                case PageStatus.Home:
                    _currentController = BeatSaberUI.CreateViewController<HomePageController>();
                    ReplaceTopViewController(_currentController, null, false, status > CurrentPageStatus ? ViewController.SlideAnimationDirection.Right : ViewController.SlideAnimationDirection.Left);
                    SetLeftScreenViewController(null);
                    SetRightScreenViewController(null);
                    ProvideInitialViewControllers(_currentController, null, null, bottomController);
                    break;
                case PageStatus.Skills:
                    _currentController = BeatSaberUI.CreateViewController<SkillTreePageController>();
                    ReplaceTopViewController(_currentController, null, false, status > CurrentPageStatus ? ViewController.SlideAnimationDirection.Right : ViewController.SlideAnimationDirection.Left);
                    SetLeftScreenViewController(null);
                    SetRightScreenViewController(null);
                    ProvideInitialViewControllers(_currentController, null, null, bottomController);
                    break;
                case PageStatus.Profile:
                    _currentController = BeatSaberUI.CreateViewController<ProfilePageController>();
                    ReplaceTopViewController(_currentController, null, false, status > CurrentPageStatus ? ViewController.SlideAnimationDirection.Right : ViewController.SlideAnimationDirection.Left);
                    SetLeftScreenViewController(null);
                    SetRightScreenViewController(null);
                    ProvideInitialViewControllers(_currentController, null, null, bottomController);
                    break;
                case PageStatus.Quests:
                    _currentController = BeatSaberUI.CreateViewController<QuestPickerPageController>();
                    ReplaceTopViewController(_currentController, null, false, status > CurrentPageStatus ? ViewController.SlideAnimationDirection.Right : ViewController.SlideAnimationDirection.Left);
                    SetRightScreenViewController(null);
                    ProvideInitialViewControllers(_currentController, null, null, bottomController);
                    SetLeftScreenViewController(BeatSaberUI.CreateViewController<CurrentQuestPageController>(), false);
                    break;
                case PageStatus.Statistics:
                    _currentController = BeatSaberUI.CreateViewController<CharacterStatsPageController>();
                    ReplaceTopViewController(_currentController, null, false, status > CurrentPageStatus ? ViewController.SlideAnimationDirection.Right : ViewController.SlideAnimationDirection.Left);
                    ProvideInitialViewControllers(_currentController, null, null, bottomController);
                    SetLeftScreenViewController(BeatSaberUI.CreateViewController<MonsterInfoPageController>(), false);
                    SetRightScreenViewController(BeatSaberUI.CreateViewController<ModifierStatsPageController>(), false);
                    break;
                case PageStatus.Shop:
                    _currentController = BeatSaberUI.CreateViewController<ShopMainPageController>();
                    ReplaceTopViewController(_currentController, null, false, status > CurrentPageStatus ? ViewController.SlideAnimationDirection.Right : ViewController.SlideAnimationDirection.Left);
                    SetLeftScreenViewController(null);
                    SetRightScreenViewController(null);
                    ProvideInitialViewControllers(_currentController, null, null, bottomController);
                    break;
                case PageStatus.SaberShop:
                    _currentController = BeatSaberUI.CreateViewController<ItemShopPageController>();
                    var sspc = (ItemShopPageController)_currentController;
                    sspc.shopType = ItemShopPageController.ShopType.Sabers;
                    ReplaceTopViewController(_currentController, null, false, status > CurrentPageStatus ? ViewController.SlideAnimationDirection.Right : ViewController.SlideAnimationDirection.Left);
                    SetLeftScreenViewController(null);
                    SetRightScreenViewController(null);
                    ProvideInitialViewControllers(_currentController, null, null, bottomController);
                    break;
                case PageStatus.PlatformShop:
                    _currentController = BeatSaberUI.CreateViewController<ItemShopPageController>();
                    var pspc = (ItemShopPageController)_currentController;
                    pspc.shopType = ItemShopPageController.ShopType.Platforms;
                    ReplaceTopViewController(_currentController, null, false, status > CurrentPageStatus ? ViewController.SlideAnimationDirection.Right : ViewController.SlideAnimationDirection.Left);
                    SetLeftScreenViewController(null);
                    SetRightScreenViewController(null);
                    ProvideInitialViewControllers(_currentController, null, null, bottomController);
                    break;
                case PageStatus.AvatarShop: //TODO: WIP
                    _currentController = BeatSaberUI.CreateViewController<ItemShopPageController>();
                    var aspc = (ItemShopPageController)_currentController;
                    aspc.shopType = ItemShopPageController.ShopType.Avatars;
                    ReplaceTopViewController(_currentController, null, false, status > CurrentPageStatus ? ViewController.SlideAnimationDirection.Right : ViewController.SlideAnimationDirection.Left);
                    SetLeftScreenViewController(null);
                    SetRightScreenViewController(null);
                    ProvideInitialViewControllers(_currentController, null, null, bottomController);
                    break;
                case PageStatus.NoteShop: //TODO: WIP
                    _currentController = BeatSaberUI.CreateViewController<ItemShopPageController>();
                    var nspc = (ItemShopPageController)_currentController;
                    nspc.shopType = ItemShopPageController.ShopType.Notes;
                    ReplaceTopViewController(_currentController, null, false, status > CurrentPageStatus ? ViewController.SlideAnimationDirection.Right : ViewController.SlideAnimationDirection.Left);
                    SetLeftScreenViewController(null);
                    SetRightScreenViewController(null);
                    ProvideInitialViewControllers(_currentController, null, null, bottomController);
                    break;
                case PageStatus.WallShop: //TODO: WIP
                    _currentController = BeatSaberUI.CreateViewController<ItemShopPageController>();
                    var wspc = (ItemShopPageController)_currentController;
                    wspc.shopType = ItemShopPageController.ShopType.Walls;
                    ReplaceTopViewController(_currentController, null, false, status > CurrentPageStatus ? ViewController.SlideAnimationDirection.Right : ViewController.SlideAnimationDirection.Left);
                    SetLeftScreenViewController(null);
                    SetRightScreenViewController(null);
                    ProvideInitialViewControllers(_currentController, null, null, bottomController);
                    break;
                case PageStatus.ItemShop: //TODO: WIP
                    _currentController = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
            ProvideInitialViewControllers(_currentController, null, null, bottomController);
            if (_currentController)
                _currentController.flowCoordinatorOwner = this;
            CurrentPageStatus = status;
        }
    }
}