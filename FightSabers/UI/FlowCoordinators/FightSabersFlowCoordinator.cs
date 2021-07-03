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
			Statistics
		}

		public FlowCoordinator oldCoordinator;
		public BottomPageController bottomController;

		public PageStatus CurrentPageStatus { get; private set; }

		protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
		{
			if (!addedToHierarchy)
			{
				return;
			}

			CurrentPageStatus = PageStatus.Home;
			var homeController = BeatSaberUI.CreateViewController<HomePageController>();
			bottomController = BeatSaberUI.CreateViewController<BottomPageController>();
			bottomController.flowCoordinatorOwner = homeController.flowCoordinatorOwner = this;
			ProvideInitialViewControllers(homeController, null, null, bottomController);
		}

		public void ActivatePage(PageStatus status)
		{
			if (status == CurrentPageStatus)
			{
				return;
			}

			FightSabersViewController controller;
			switch (status)
			{
				case PageStatus.Home:
					controller = BeatSaberUI.CreateViewController<HomePageController>();
					ReplaceTopViewController(controller);
					SetLeftScreenViewController(null, ViewController.AnimationType.Out);
					SetRightScreenViewController(null, ViewController.AnimationType.Out);
					ProvideInitialViewControllers(controller, null, null, bottomController);
					break;
				case PageStatus.Skills:
					controller = BeatSaberUI.CreateViewController<SkillTreePageController>();
					ReplaceTopViewController(controller);
					SetLeftScreenViewController(null, ViewController.AnimationType.Out);
					SetRightScreenViewController(null, ViewController.AnimationType.Out);
					ProvideInitialViewControllers(controller, null, null, bottomController);
					break;
				case PageStatus.Profile:
					controller = BeatSaberUI.CreateViewController<ProfilePageController>();
					ReplaceTopViewController(controller);
					SetLeftScreenViewController(null, ViewController.AnimationType.Out);
					SetRightScreenViewController(null, ViewController.AnimationType.Out);
					ProvideInitialViewControllers(controller, null, null, bottomController);
					break;
				case PageStatus.Quests:
					controller = BeatSaberUI.CreateViewController<QuestPickerPageController>();
					ReplaceTopViewController(controller);
					SetRightScreenViewController(null, ViewController.AnimationType.Out);
					ProvideInitialViewControllers(controller, null, null, bottomController);
					SetLeftScreenViewController(BeatSaberUI.CreateViewController<CurrentQuestPageController>(), ViewController.AnimationType.In);
					break;
				case PageStatus.Statistics:
					controller = BeatSaberUI.CreateViewController<CharacterStatsPageController>();
					ReplaceTopViewController(controller);
					ProvideInitialViewControllers(controller, null, null, bottomController);
					SetLeftScreenViewController(BeatSaberUI.CreateViewController<MonsterInfoPageController>(), ViewController.AnimationType.In);
					SetRightScreenViewController(BeatSaberUI.CreateViewController<ModifierStatsPageController>(), ViewController.AnimationType.In);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(status), status, null);
			}

			controller.flowCoordinatorOwner = this;
			CurrentPageStatus = status;
		}
	}
}