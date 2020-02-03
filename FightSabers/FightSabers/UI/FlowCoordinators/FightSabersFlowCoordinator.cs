using System;
using BeatSaberMarkupLanguage;
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
            Shop
        }

        public FlowCoordinator      oldCoordinator;
        public BottomPageController bottomController;

        public PageStatus CurrentPageStatus { get; private set; }

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

        public void ActivatePage(PageStatus status)
        {
            if (status == CurrentPageStatus) return;
            FightSabersViewController controller;
            switch (status)
            {
                case PageStatus.Home:
                    controller = BeatSaberUI.CreateViewController<HomePageController>();
                    ReplaceTopViewController(controller, null, false, ViewController.SlideAnimationDirection.Left);
                    SetLeftScreenViewController(null);
                    SetRightScreenViewController(null);
                    ProvideInitialViewControllers(controller, null, null, bottomController);
                    break;
                case PageStatus.Skills:
                    controller = BeatSaberUI.CreateViewController<SkillTreePageController>();
                    ReplaceTopViewController(controller, null, false, ViewController.SlideAnimationDirection.Left);
                    SetLeftScreenViewController(null);
                    SetRightScreenViewController(null);
                    ProvideInitialViewControllers(controller, null, null, bottomController);
                    break;
                case PageStatus.Profile:
                    controller = BeatSaberUI.CreateViewController<ProfilePageController>();
                    ReplaceTopViewController(controller, null, false, ViewController.SlideAnimationDirection.Left);
                    SetLeftScreenViewController(null);
                    SetRightScreenViewController(null);
                    ProvideInitialViewControllers(controller, null, null, bottomController);
                    break;
                case PageStatus.Quests:
                    controller = BeatSaberUI.CreateViewController<QuestPickerPageController>();
                    ReplaceTopViewController(controller, null, false, ViewController.SlideAnimationDirection.Left);
                    SetRightScreenViewController(null);
                    ProvideInitialViewControllers(controller, null, null, bottomController);
                    SetLeftScreenViewController(BeatSaberUI.CreateViewController<CurrentQuestPageController>(), false);
                    break;
                case PageStatus.Statistics:
                    controller = BeatSaberUI.CreateViewController<CharacterStatsPageController>();
                    ReplaceTopViewController(controller, null, false, ViewController.SlideAnimationDirection.Left);
                    ProvideInitialViewControllers(controller, null, null, bottomController);
                    SetLeftScreenViewController(BeatSaberUI.CreateViewController<MonsterInfoPageController>(), false);
                    SetRightScreenViewController(BeatSaberUI.CreateViewController<ModifierStatsPageController>(), false);
                    break;
                case PageStatus.Shop:
                    controller = BeatSaberUI.CreateViewController<ShopMainPageController>();
                    ReplaceTopViewController(controller, null, false, ViewController.SlideAnimationDirection.Left);
                    SetLeftScreenViewController(null);
                    SetRightScreenViewController(BeatSaberUI.CreateViewController<ShopItemPreviewPageController>());
                    ProvideInitialViewControllers(controller, null, null, bottomController);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
            controller.flowCoordinatorOwner = this;
            CurrentPageStatus = status;
        }
    }
}