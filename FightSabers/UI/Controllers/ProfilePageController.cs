using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace FightSabers.UI.Controllers
{
	[HotReload(RelativePathToLayout = @"..\Views\ProfilePageView.bsml")]
	[ViewDefinition("FightSabers.UI.Views.ProfilePageView.bsml")]
	internal class ProfilePageController : BSMLAutomaticViewController
	{
	}
}