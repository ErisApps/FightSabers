using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace FightSabers.UI.Controllers
{
	[HotReload(RelativePathToLayout = @"..\Views\SkillTreePageView.bsml")]
	[ViewDefinition("FightSabers.UI.Views.SkillTreePageView.bsml")]
	internal class SkillTreePageController : BSMLAutomaticViewController
	{
	}
}