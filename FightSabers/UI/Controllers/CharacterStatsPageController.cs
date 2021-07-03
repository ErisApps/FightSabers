using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;

namespace FightSabers.UI.Controllers
{
	internal class CharacterStatsPageController : FightSabersViewController
	{
		public override string ResourceName => "FightSabers.UI.Views.CharacterStatsPageView.bsml";
		public override string ContentFilePath => "D:\\Bibliotheques\\Documents\\GitHub\\FightSabers\\FightSabers\\FightSabers\\UI\\Views\\CharacterStatsPageView.bsml";

		[UIParams]
		private BSMLParserParams parserParams;
	}
}