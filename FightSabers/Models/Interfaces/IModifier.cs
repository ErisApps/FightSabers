namespace FightSabers.Models.Interfaces
{
	public interface IModifier
	{
		/// <summary>
		/// Title of the modifier
		/// </summary>
		string Title { get; set; }

		/// <summary>
		/// Description of the modifier
		/// </summary>
		string Description { get; set; }

		/// <summary>
		/// Strength of the modifier, above 0
		/// </summary>
		float Strength { get; set; }

		void EnableModifier();
		void DisableModifier();
	}
}