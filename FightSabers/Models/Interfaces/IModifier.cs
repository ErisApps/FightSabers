namespace FightSabers.Models.Interfaces
{
    public interface IModifier
    {
        #region Properties

        /// <summary>
        /// Title of the modifier
        /// </summary>
        string title { get; set; }
        /// <summary>
        /// Description of the modifier
        /// </summary>
        string description { get; set; }
        /// <summary>
        /// Strength of the modifier, above 0
        /// </summary>
        float strength { get; set; }

        #endregion

        void EnableModifier();
        void DisableModifier();
    }
}
