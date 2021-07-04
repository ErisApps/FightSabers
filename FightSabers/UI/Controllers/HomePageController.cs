using System;
using System.Diagnostics;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using FightSabers.Settings;
using IPA.Loader;
using Zenject;

namespace FightSabers.UI.Controllers
{
	[HotReload(RelativePathToLayout = @"..\Views\HomePageView.bsml")]
	[ViewDefinition("FightSabers.UI.Views.HomePageView.bsml")]
	internal class HomePageController : BSMLAutomaticViewController, IDisposable
	{
		private PluginConfig _config = null!;
		private PluginMetadata _metadata = null!;

		[Inject]
		internal void Construct(PluginConfig config, [Inject(Id = Constants.BindingIds.METADATA)] PluginMetadata metadata)
		{
			_metadata = metadata;
			_config = config;

			VersionText = $"Version {_metadata.Version}";

			_config.ConfigChanged -= OnConfigChanged;
			_config.ConfigChanged += OnConfigChanged;
		}

		public void Dispose()
		{
			_config.ConfigChanged -= OnConfigChanged;
		}

		[UIValue("switch-plugin-btn-status")]
		internal string ModStatus { get; private set; } = null!;

		[UIValue("plugin-text-status")]
		internal string PluginTextStatus { get; private set; } = null!;

		[UIValue("plugin-text-color-status")]
		internal string PluginTextColorStatus { get; private set; } = null!;

		[UIValue("version-text")]
		internal string VersionText { get; private set; } = null!;


		[UIAction("plugin-status-act")]
		private void PluginSwitchAction()
		{
			_config.Enabled = !_config.Enabled;

			RefreshPageUI();
		}

		[UIAction("github-link-act")]
		private void OpenGithubLink()
		{
			Process.Start(_metadata.PluginSourceLink.ToString());
		}

		[UIAction("donate-link-act")]
		private void OpenDonateLink()
		{
			Process.Start(_metadata.DonateLink.ToString());
		}

		[UIAction("bug-link-act")]
		private void OpenBugLink()
		{
			Process.Start(_metadata.PluginSourceLink + "/issues");
		}

		private void OnConfigChanged(object sender, EventArgs e)
		{
			RefreshPageUI();
		}

		private void RefreshPageUI()
		{
			ModStatus = _config.Enabled ? "Disable" : "Enable";
			PluginTextStatus = $"FightSabers is {(_config.Enabled ? "enabled" : "disabled")}!";
			PluginTextColorStatus = _config.Enabled ? "lime" : "red";
		}
	}
}