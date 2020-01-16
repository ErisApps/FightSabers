using System.Linq;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using FightSabers.UI.FlowCoordinators;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace FightSabers.UI.Controllers
{
    internal class OverlayViewController : FightSabersViewController
    {
        public override string ResourceName => "FightSabers.UI.Views.OverlayView.bsml";
        //public override string ContentFilePath => "D:\\Bibliotheques\\Documents\\GitHub\\FightSabers\\FightSabers\\FightSabers\\UI\\Views\\OverlayView.bsml";

        [UIParams]
        private BSMLParserParams parserParams;

        private bool _shouldOpenPage = true;
        private FightSabersFlowCoordinator _flowCoordinatorOwner;
        [UIComponent("switch-fightsabers-btn")] private Button _openFightSabersButton;

        private string _buttonStatus = "Open FightSabers";
        [UIValue("fightsabers-btn-status")]
        public string buttonStatus
        {
            get { return _buttonStatus; }
            private set
            {
                _buttonStatus = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("switch-fightsabers-act")]
        public void ShowModPageClick()
        {
            if (_shouldOpenPage)
            {
                var currentFlow = Resources.FindObjectsOfTypeAll<FlowCoordinator>().FirstOrDefault(f => f.isActivated);
                _flowCoordinatorOwner = BeatSaberUI.CreateFlowCoordinator<FightSabersFlowCoordinator>();
                _flowCoordinatorOwner.oldCoordinator = currentFlow;
                currentFlow.PresentFlowCoordinator(_flowCoordinatorOwner);
            }
            else
                _flowCoordinatorOwner.oldCoordinator.DismissFlowCoordinator(_flowCoordinatorOwner);
            _shouldOpenPage = !_shouldOpenPage;
            buttonStatus = _shouldOpenPage ? "Open FightSabers" : "Close FightSabers";
        }
    }
}
