using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;

namespace FightSabers.UI.Controllers
{
	internal class SkillTreePageController : FightSabersViewController
	{
		public override string ResourceName => "FightSabers.UI.Views.SkillTreePageView.bsml";
		public override string ContentFilePath => "D:\\Bibliotheques\\Documents\\GitHub\\FightSabers\\FightSabers\\FightSabers\\UI\\Views\\SkillTreePageView.bsml";

		[UIParams]
		private BSMLParserParams parserParams;
	}
}