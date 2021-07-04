using Newtonsoft.Json;

namespace FightSabers.Models.Interfaces
{
	public interface IQuest
	{
		/// <summary>
		/// Title of the quest
		/// </summary>
		string Title { get; set; }

		/// <summary>
		/// Description of the quest
		/// </summary>
		string Description { get; set; }

		/// <summary>
		/// Progress string visual of the quest
		/// </summary>
		string ProgressHint { get; set; }

		/// <summary>
		/// Type of the quest
		/// </summary>
		string QuestType { get; set; }

		/// <summary>
		/// Experience reward of the quest
		/// </summary>
		uint ExpReward { get; set; }

		/// <summary>
		/// Is the quest initialized?
		/// </summary>
		[JsonIgnore]
		bool IsInitialized { get; set; }

		/// <summary>
		/// Is the quest activated?
		/// </summary>
		[JsonIgnore]
		bool IsActivated { get; set; }

		/// <summary>
		/// Is the quest completed?
		/// </summary>
		[JsonIgnore]
		bool IsCompleted { get; set; }

		/// <summary>
		/// Does the quest has game events activated?
		/// </summary>
		[JsonIgnore]
		bool HasGameEventsActivated { get; set; }

		[JsonProperty("progress")]
		float Progress { get; set; }

		void ForceInitialize();
		void Activate(bool forceInitialize = false);
		void Deactivate();
		void Complete();
		void LinkGameEvents();
		void UnlinkGameEvents();
	}
}