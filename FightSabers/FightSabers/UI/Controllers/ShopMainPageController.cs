using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;

namespace FightSabers.UI.Controllers
{
    internal class ShopMainPageController : FightSabersViewController
    {
        public override string ResourceName => "FightSabers.UI.Views.ShopMainPageView.bsml";
        public override string ContentFilePath => "D:\\Bibliotheques\\Documents\\GitHub\\FightSabers\\FightSabers\\FightSabers\\UI\\Views\\ShopMainPageView.bsml";

        [UIParams]
        private BSMLParserParams parserParams;
    }
}
