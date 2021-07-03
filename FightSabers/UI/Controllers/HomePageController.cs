using System.Diagnostics;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;

namespace FightSabers.UI.Controllers
{
	internal class HomePageController : FightSabersViewController
	{
		private const string GITHUB_REPO_URL = "https://github.com/ErisApps/FightSabers/";

		public override string ResourceName => "FightSabers.UI.Views.HomePageView.bsml";
		public override string ContentFilePath => "D:\\Bibliotheques\\Documents\\GitHub\\FightSabers\\FightSabers\\FightSabers\\UI\\Views\\HomePageView.bsml";

		[UIParams]
		private BSMLParserParams parserParams;

		private string _modStatus = "Disable";

		[UIValue("switch-plugin-btn-status")]
		public string modStatus
		{
			get => _modStatus;
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
			get => _pluginTextStatus;
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
			get => _pluginTextColorStatus;
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
			get => _versionText;
			private set
			{
				_versionText = value;
				NotifyPropertyChanged();
			}
		}

		protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
		{
			base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

			versionText = $"Version {Plugin.FightSabersMetadata.Version}";
			RefreshPageUI();
		}

		[UIAction("plugin-status-act")]
		private void PluginSwitchAction()
		{
			Plugin.config.Value.Enabled = !Plugin.config.Value.Enabled;
			OverlayViewController.instance.experienceContainerState = Plugin.config.Value.Enabled;
			OverlayViewController.instance.fsDisableContainerState = !Plugin.config.Value.Enabled;
			Plugin.configProvider.Store(Plugin.config.Value);
			RefreshPageUI();
		}

		[UIAction("github-link-act")]
		private void OpenGithubLink()
		{
			Process.Start(GITHUB_REPO_URL);
		}

		[UIAction("donate-link-act")]
		private void OpenDonateLink()
		{
			Process.Start("https://ko-fi.com/shoko84");
		}

		[UIAction("bug-link-act")]
		private void OpenBugLink()
		{
			Process.Start(GITHUB_REPO_URL + "issues");
		}

		private void RefreshPageUI()
		{
			modStatus = Plugin.config.Value.Enabled ? "Disable" : "Enable";
			pluginTextStatus = $"FightSabers is {(Plugin.config.Value.Enabled ? "enabled" : "disabled")}!";
			pluginTextColorStatus = Plugin.config.Value.Enabled ? "lime" : "red";
		}
	}
}