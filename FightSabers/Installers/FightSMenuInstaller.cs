using FightSabers.UI.Controllers;
using FightSabers.UI.FlowCoordinators;
using FightSabers.UI.Managers;
using SiraUtil;
using Zenject;

namespace FightSabers.Installers
{
	internal class FightSMenuInstaller : Installer<FightSMenuInstaller>
	{
		public override void InstallBindings()
		{
			Container.Bind<FightSabersGameplaySetup>().AsSingle();
			Container.BindInterfacesTo<GamePlaySetupManager>().AsSingle();

			Container.Bind<BottomPageController>().FromNewComponentAsViewController().AsSingle().Lazy();
			Container.Bind<CharacterStatsPageController>().FromNewComponentAsViewController().AsSingle().Lazy();
			Container.Bind<CurrentQuestPageController>().FromNewComponentAsViewController().AsSingle().Lazy();
			Container.Bind<HomePageController>().FromNewComponentAsViewController().AsSingle().Lazy();
			Container.Bind<ModifierStatsPageController>().FromNewComponentAsViewController().AsSingle().Lazy();
			Container.Bind<MonsterInfoPageController>().FromNewComponentAsViewController().AsSingle().Lazy();
			Container.Bind<OverlayViewController>().FromNewComponentAsViewController().AsSingle().Lazy();
			Container.Bind<ProfilePageController>().FromNewComponentAsViewController().AsSingle().Lazy();
			Container.Bind<QuestPickerPageController>().FromNewComponentAsViewController().AsSingle().Lazy();
			Container.Bind<SkillTreePageController>().FromNewComponentAsViewController().AsSingle().Lazy();
			Container.Bind<FightSabersFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle().Lazy();
		}
	}
}