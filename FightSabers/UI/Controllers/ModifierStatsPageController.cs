using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace FightSabers.UI.Controllers
{
	[HotReload(RelativePathToLayout = @"..\Views\ModifierStatsPageView.bsml")]
	[ViewDefinition("FightSabers.UI.Views.ModifierStatsPageView.bsml")]
	internal class ModifierStatsPageController : BSMLAutomaticViewController
	{
	}
}