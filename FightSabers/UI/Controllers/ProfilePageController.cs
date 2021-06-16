using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;

namespace FightSabers.UI.Controllers
{
    internal class ProfilePageController : FightSabersViewController
    {
        public override string ResourceName => "FightSabers.UI.Views.QuestPickerPageView.bsml";
        public override string ContentFilePath => "D:\\Bibliotheques\\Documents\\GitHub\\FightSabers\\FightSabers\\FightSabers\\UI\\Views\\QuestPickerPageView.bsml";

        [UIParams]
        private BSMLParserParams parserParams;
    }
}
