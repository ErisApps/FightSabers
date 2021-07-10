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
		private LazyInject<SkillTreePageController> _skillTreePageController = null!;
		private LazyInject<ProfilePageController> _profilePageController = null!;
		private LazyInject<QuestPickerPageController> _questPickerPageController = null!;
		private LazyInject<CurrentQuestPageController> _currentQuestPageController = null!;
		private LazyInject<CharacterStatsPageController> _characterStatsPageController = null!;
		private LazyInject<MonsterInfoPageController> _monsterInfoPageController = null!;
		private LazyInject<ModifierStatsPageController> _modifierStatsPageController = null!;

		private PageStatus _currentPageStatus;

		internal enum PageStatus
		{
			Home,
			Skills,
			Profile,
			Quests,
			Statistics
		}

		public FlowCoordinator oldCoordinator;

		[Inject]
		internal void Construct(HomePageController homePageController, BottomPageController bottomPageController, LazyInject<SkillTreePageController> skillTreePageController,
			LazyInject<ProfilePageController> profilePageController, LazyInject<QuestPickerPageController> questPickerPageController, LazyInject<CurrentQuestPageController> currentQuestPageController,
			LazyInject<CharacterStatsPageController> characterStatsPageController, LazyInject<MonsterInfoPageController> monsterInfoPageController,
			LazyInject<ModifierStatsPageController> modifierStatsPageController)
		{
			_homePageController = homePageController;
			_bottomPageController = bottomPageController;
			_skillTreePageController = skillTreePageController;
			_profilePageController = profilePageController;
			_questPickerPageController = questPickerPageController;
			_currentQuestPageController = currentQuestPageController;
			_characterStatsPageController = characterStatsPageController;
			_monsterInfoPageController = monsterInfoPageController;
			_modifierStatsPageController = modifierStatsPageController;
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
			if (status == _currentPageStatus)
			{
				return;
			}

			switch (status)
			{
				case PageStatus.Home:
					ReplaceTopViewController(_homePageController);
					SetLeftScreenViewController(null, ViewController.AnimationType.Out);
					SetRightScreenViewController(null, ViewController.AnimationType.Out);
					ProvideInitialViewControllers(_homePageController, null, null, _bottomPageController);
					break;
				case PageStatus.Skills:
					ReplaceTopViewController(_skillTreePageController.Value);
					SetLeftScreenViewController(null, ViewController.AnimationType.Out);
					SetRightScreenViewController(null, ViewController.AnimationType.Out);
					ProvideInitialViewControllers(_skillTreePageController.Value, null, null, _bottomPageController);
					break;
				case PageStatus.Profile:
					ReplaceTopViewController(_profilePageController.Value);
					SetLeftScreenViewController(null, ViewController.AnimationType.Out);
					SetRightScreenViewController(null, ViewController.AnimationType.Out);
					ProvideInitialViewControllers(_profilePageController.Value, null, null, _bottomPageController);
					break;
				case PageStatus.Quests:
					ReplaceTopViewController(_questPickerPageController.Value);
					SetRightScreenViewController(null, ViewController.AnimationType.Out);
					ProvideInitialViewControllers(_questPickerPageController.Value, null, null, _bottomPageController);
					SetLeftScreenViewController(_currentQuestPageController.Value, ViewController.AnimationType.In);
					break;
				case PageStatus.Statistics:
					ReplaceTopViewController(_characterStatsPageController.Value);
					ProvideInitialViewControllers(_characterStatsPageController.Value, null, null, _bottomPageController);
					SetLeftScreenViewController(_monsterInfoPageController.Value, ViewController.AnimationType.In);
					SetRightScreenViewController(_modifierStatsPageController.Value, ViewController.AnimationType.In);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(status), status, null);
			}

			_currentPageStatus = status;
		}
	}
}