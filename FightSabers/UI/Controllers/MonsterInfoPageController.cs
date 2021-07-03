using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace FightSabers.UI.Controllers
{
	[HotReload(RelativePathToLayout = @"..\Views\MonsterInfoPageView.bsml")]
	[ViewDefinition("FightSabers.UI.Views.MonsterInfoPageView.bsml")]
	internal class MonsterInfoPageController : BSMLAutomaticViewController
	{
	}
}