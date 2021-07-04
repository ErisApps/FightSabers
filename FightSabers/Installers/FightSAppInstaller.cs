using FightSabers.Settings;
using IPA.Loader;
using IPA.Logging;
using SiraUtil;
using Zenject;

namespace FightSabers.Installers
{
	internal class FightSAppInstaller : Installer<Logger, PluginMetadata, PluginConfig, FightSAppInstaller>
	{
		private readonly Logger _logger;
		private readonly PluginMetadata _pluginMetadata;
		private readonly PluginConfig _config;

		public FightSAppInstaller(Logger logger, PluginMetadata pluginMetadata, PluginConfig config)
		{
			_logger = logger;
			_pluginMetadata = pluginMetadata;
			_config = config;
		}

		public override void InstallBindings()
		{
			Container.BindLoggerAsSiraLogger(_logger);
			Container.BindInstance(_pluginMetadata).WithId(Constants.BindingIds.METADATA);
			Container.BindInstance(_config).AsSingle();
		}
	}
}