using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using FightSabers.UI.FlowCoordinators;

namespace FightSabers.UI.Controllers
{
	[HotReload(RelativePathToLayout = @"..\Views\BottomPageView.bsml")]
	[ViewDefinition("FightSabers.UI.Views.BottomPageView.bsml")]
	internal class BottomPageController : BSMLAutomaticViewController, ICanControlFlowCoordinator
	{
		public FightSabersFlowCoordinator FlowCoordinatorOwner { get; set; }

		[UIParams]
		private BSMLParserParams parserParams;

		[UIAction("quests-page-act")]
		private void QuestsPageClicked()
		{
			FlowCoordinatorOwner.ActivatePage(FightSabersFlowCoordinator.PageStatus.Quests);
		}

		[UIAction("skills-page-act")]
		private void SkillsPageClicked()
		{
			FlowCoordinatorOwner.ActivatePage(FightSabersFlowCoordinator.PageStatus.Skills);
		}

		[UIAction("home-page-act")]
		private void HomePageClicked()
		{
			FlowCoordinatorOwner.ActivatePage(FightSabersFlowCoordinator.PageStatus.Home);
		}

		[UIAction("profile-page-act")]
		private void ProfilePageClicked()
		{
			FlowCoordinatorOwner.ActivatePage(FightSabersFlowCoordinator.PageStatus.Profile);
		}

		[UIAction("stats-page-act")]
		private void StatsPageClicked()
		{
			FlowCoordinatorOwner.ActivatePage(FightSabersFlowCoordinator.PageStatus.Statistics);
		}
	}
}