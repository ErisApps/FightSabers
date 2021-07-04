using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.ViewControllers;
using FightSabers.UI.Controllers;
using HMUI;
using Zenject;

namespace FightSabers.UI.FlowCoordinators
{
	internal class FightSabersFlowCoordinator : FlowCoordinator
	{
		private HomePageController _homePageController = null!;
		private BottomPageController _bottomPageController = null!;

		public enum PageStatus
		{
			Home,
			Skills,
			Profile,
			Quests,
			Statistics
		}

		public FlowCoordinator oldCoordinator;

		public PageStatus CurrentPageStatus { get; private set; }

		[Inject]
		internal void Construct(HomePageController homePageController, BottomPageController bottomPageController)
		{
			_homePageController = homePageController;
			_bottomPageController = bottomPageController;
		}

		protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
		{
			if (!addedToHierarchy)
			{
				return;
			}

			_bottomPageController.FlowCoordinatorOwner = this;
			ActivatePage(PageStatus.Home);
		}

		public void ActivatePage(PageStatus status)
		{
			if (status == CurrentPageStatus)
			{
				return;
			}

			BSMLAutomaticViewController controller;
			switch (status)
			{
				case PageStatus.Home:
					controller = _homePageController;
					ReplaceTopViewController(controller);
					SetLeftScreenViewController(null, ViewController.AnimationType.Out);
					SetRightScreenViewController(null, ViewController.AnimationType.Out);
					ProvideInitialViewControllers(controller, null, null, _bottomPageController);
					break;
				case PageStatus.Skills:
					controller = BeatSaberUI.CreateViewController<SkillTreePageController>();
					ReplaceTopViewController(controller);
					SetLeftScreenViewController(null, ViewController.AnimationType.Out);
					SetRightScreenViewController(null, ViewController.AnimationType.Out);
					ProvideInitialViewControllers(controller, null, null, _bottomPageController);
					break;
				case PageStatus.Profile:
					controller = BeatSaberUI.CreateViewController<ProfilePageController>();
					ReplaceTopViewController(controller);
					SetLeftScreenViewController(null, ViewController.AnimationType.Out);
					SetRightScreenViewController(null, ViewController.AnimationType.Out);
					ProvideInitialViewControllers(controller, null, null, _bottomPageController);
					break;
				case PageStatus.Quests:
					controller = BeatSaberUI.CreateViewController<QuestPickerPageController>();
					ReplaceTopViewController(controller);
					SetRightScreenViewController(null, ViewController.AnimationType.Out);
					ProvideInitialViewControllers(controller, null, null, _bottomPageController);
					SetLeftScreenViewController(BeatSaberUI.CreateViewController<CurrentQuestPageController>(), ViewController.AnimationType.In);
					break;
				case PageStatus.Statistics:
					controller = BeatSaberUI.CreateViewController<CharacterStatsPageController>();
					ReplaceTopViewController(controller);
					ProvideInitialViewControllers(controller, null, null, _bottomPageController);
					SetLeftScreenViewController(BeatSaberUI.CreateViewController<MonsterInfoPageController>(), ViewController.AnimationType.In);
					SetRightScreenViewController(BeatSaberUI.CreateViewController<ModifierStatsPageController>(), ViewController.AnimationType.In);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(status), status, null);
			}

			CurrentPageStatus = status;
		}
	}
}