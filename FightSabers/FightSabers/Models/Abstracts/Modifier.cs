using FightSabers.Models.Interfaces;
using UnityEngine;

namespace FightSabers.Models.Abstracts
{
    public abstract class Modifier : MonoBehaviour, IModifier
    {
        public string title { get; set; }
        public string description { get; set; }
        public float strength { get; set; } = 1f;
        public GameNoteController gameNoteController;

        public abstract void EnableModifier();
        public abstract void DisableModifier();
    }
}
