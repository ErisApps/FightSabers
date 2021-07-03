using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace FightSabers.UI.Controllers
{
	[HotReload(RelativePathToLayout = @"..\Views\CharacterStatsPageView.bsml")]
	[ViewDefinition("FightSabers.UI.Views.CharacterStatsPageView.bsml")]
	internal class CharacterStatsPageController : BSMLAutomaticViewController
	{
	}
}