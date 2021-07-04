using FightSabers.UI.FlowCoordinators;

namespace FightSabers.UI.Controllers
{
	internal interface ICanControlFlowCoordinator
	{
		FightSabersFlowCoordinator? FlowCoordinatorOwner { get; set; }
	}
}