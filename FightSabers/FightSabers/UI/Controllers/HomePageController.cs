using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;

namespace FightSabers.UI.Controllers
{
    internal class HomePageController : FightSabersViewController
    {
        public override string ResourceName => "FightSabers.UI.Views.HomePageView.bsml";
        //public override string ContentFilePath => "D:\\Bibliotheques\\Documents\\GitHub\\FightSabers\\FightSabers\\FightSabers\\UI\\Views\\HomePageView.bsml";

        #region Properties

        [UIParams]
        private BSMLParserParams parserParams;

        private string _modStatus = "Disable";
        [UIValue("switch-plugin-btn-status")]
        public string modStatus
        {
            get { return _modStatus; }
            private set
            {
                _modStatus = value;
                NotifyPropertyChanged();
            }
        }

        private string _pluginTextStatus = "FightSabers is enabled!";
        [UIValue("plugin-text-status")]
        public string pluginTextStatus
        {
            get { return _pluginTextStatus; }
            private set
            {
                _pluginTextStatus = value;
                NotifyPropertyChanged();
            }
        }

        private string _pluginTextColorStatus = "lime";
        [UIValue("plugin-text-color-status")]
        public string pluginTextColorStatus
        {
            get { return _pluginTextColorStatus; }
            private set
            {
                _pluginTextColorStatus = value;
                NotifyPropertyChanged();
            }
        }

        private string _versionText = "Version 0.0.0";
        [UIValue("version-text")]
        public string versionText
        {
            get { return _versionText; }
            private set
            {
                _versionText = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            base.DidActivate(firstActivation, type);
            versionText = $"Version {Plugin.fightSabersMetadata.Version}";
            RefreshPageUI();
        }

        [UIAction("plugin-status-act")]
        private void PluginSwitchAction()
        {
            Plugin.config.Value.Enabled = !Plugin.config.Value.Enabled;
            Plugin.configProvider.Store(Plugin.config.Value);
            RefreshPageUI();
        }

        [UIAction("github-link-act")]
        private void OpenGithubLink()
        {
            System.Diagnostics.Process.Start("https://github.com/Shoko84/FightSabers/");
        }

        [UIAction("donate-link-act")]
        private void OpenDonateLink()
        {
            System.Diagnostics.Process.Start("https://ko-fi.com/shoko84");
        }

        [UIAction("bug-link-act")]
        private void OpenBugLink()
        {
            System.Diagnostics.Process.Start("https://github.com/Shoko84/FightSabers/issues");
        }

        private void RefreshPageUI()
        {
            modStatus = Plugin.config.Value.Enabled ? "Disable" : "Enable";
            pluginTextStatus = $"FightSabers is {(Plugin.config.Value.Enabled ? "enabled" : "disabled")}!";
            pluginTextColorStatus = Plugin.config.Value.Enabled ? "lime" : "red";
        }
    }
}
