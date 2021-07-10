using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using FightSabers.UI.FlowCoordinators;

namespace FightSabers.UI.Controllers
{
	[HotReload(RelativePathToLayout = @"..\Views\BottomPageView.bsml")]
	[ViewDefinition("FightSabers.UI.Views.BottomPageView.bsml")]
	internal class BottomPageController : BSMLAutomaticViewController
	{
		public event Action<FightSabersFlowCoordinator.PageStatus>? PageTypeActivationRequested;

		[UIAction("quests-page-act")]
		private void QuestsPageClicked()
		{
			PageTypeActivationRequested?.Invoke(FightSabersFlowCoordinator.PageStatus.Quests);
		}

		[UIAction("skills-page-act")]
		private void SkillsPageClicked()
		{
			PageTypeActivationRequested?.Invoke(FightSabersFlowCoordinator.PageStatus.Skills);
		}

		[UIAction("home-page-act")]
		private void HomePageClicked()
		{
			PageTypeActivationRequested?.Invoke(FightSabersFlowCoordinator.PageStatus.Home);
		}

		[UIAction("profile-page-act")]
		private void ProfilePageClicked()
		{
			PageTypeActivationRequested?.Invoke(FightSabersFlowCoordinator.PageStatus.Profile);
		}

		[UIAction("stats-page-act")]
		private void StatsPageClicked()
		{
			PageTypeActivationRequested?.Invoke(FightSabersFlowCoordinator.PageStatus.Statistics);
		}
	}
}