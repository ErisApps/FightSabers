using FightSabers.Models.Interfaces;
using UnityEngine;

namespace FightSabers.Models.Abstracts
{
	public abstract class Modifier : MonoBehaviour, IModifier
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public float Strength { get; set; } = 1f;
		public GameNoteController GameNoteController;

		public abstract void EnableModifier();
		public abstract void DisableModifier();
	}
}