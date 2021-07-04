using BeatSaberMarkupLanguage.GameplaySetup;
using FightSabers.UI.Controllers;
using Zenject;

namespace FightSabers.UI.Managers
{
	internal class GamePlaySetupManager : IInitializable
	{
		private readonly FightSabersGameplaySetup _fightSabersGameplaySetup;

		public GamePlaySetupManager(FightSabersGameplaySetup fightSabersGameplaySetup)
		{
			_fightSabersGameplaySetup = fightSabersGameplaySetup;
		}

		public void Initialize()
		{
			GameplaySetup.instance.AddTab("FS Modifiers", "FightSabers.UI.Views.FightSabersGameplaySetupView.bsml", _fightSabersGameplaySetup, MenuType.Solo | MenuType.Custom);
		}
	}
}