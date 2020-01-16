using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;

namespace FightSabers.UI.Controllers
{
    internal class ModifierStatsPageController : FightSabersViewController
    {
        public override string ResourceName => "FightSabers.UI.Views.ModifierStatsPageView.bsml";
        //public override string ContentFilePath => "D:\\Bibliotheques\\Documents\\GitHub\\FightSabers\\FightSabers\\FightSabers\\UI\\Views\\ModifierStatsPageView.bsml";

        [UIParams]
        private BSMLParserParams parserParams;
    }
}
