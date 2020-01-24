using Newtonsoft.Json;

namespace FightSabers.Models.Interfaces
{
    public interface IQuest
    {
        #region Properties

        /// <summary>
        /// Title of the quest
        /// </summary>
        string title { get; set; }
        /// <summary>
        /// Description of the quest
        /// </summary>
        string description { get; set; }
        /// <summary>
        /// Progress string visual of the quest
        /// </summary>
        string progressHint { get; set; }
        /// <summary>
        /// Type of the quest
        /// </summary>
        string questType { get; set; }
        /// <summary>
        /// Experience reward of the quest
        /// </summary>
        uint expReward { get; set; }
        /// <summary>
        /// Is the quest initialized?
        /// </summary>
        [JsonIgnore]
        bool isInitialized { get; set; }
        /// <summary>
        /// Is the quest activated?
        /// </summary>
        [JsonIgnore]
        bool isActivated { get; set; }
        /// <summary>
        /// Is the quest completed?
        /// </summary>
        [JsonIgnore]
        bool isCompleted { get; set; }
        /// <summary>
        /// Does the quest has game events activated?
        /// </summary>
        [JsonIgnore]
        bool hasGameEventsActivated { get; set; }

        [JsonProperty("progress")]
        float Progress { get; set; }

        void ForceInitialize();
        void Activate(bool forceInitialize = false);
        void Deactivate();
        void Complete();
        void LinkGameEvents();
        void UnlinkGameEvents();

        #endregion
    }
}
