using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;

namespace FightSabers.UI.Controllers
{
    internal class CurrentQuestPageController : FightSabersViewController
    {
        public override string ResourceName => "FightSabers.UI.Views.CurrentQuestPageView.bsml";
        public override string ContentFilePath => "D:\\Bibliotheques\\Documents\\GitHub\\FightSabers\\FightSabers\\FightSabers\\UI\\Views\\CurrentQuestPageView.bsml";

        [UIParams]
        private BSMLParserParams parserParams;
    }
}
