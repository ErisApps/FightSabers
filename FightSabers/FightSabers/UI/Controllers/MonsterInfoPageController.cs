using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;

namespace FightSabers.UI.Controllers
{
    internal class MonsterInfoPageController : FightSabersViewController
    {
        public override string ResourceName => "FightSabers.UI.Views.MonsterInfoPageView.bsml";
        //public override string ContentFilePath => "D:\\Bibliotheques\\Documents\\GitHub\\FightSabers\\FightSabers\\FightSabers\\UI\\Views\\MonsterInfoPageView.bsml";

        [UIParams]
        private BSMLParserParams parserParams;

        
    }
}
