using FightSabers.Models;
using UnityEngine;

namespace FightSabers.Settings
{
    internal class PluginConfig
    {
        public bool RegenerateConfig = true;
        public bool Enabled = true;
        public Float3 FSPanelPosition = new Float3(0, 3.85f, 2.4f);
        public Float3 FSPanelRotation = new Float3(-15, 0, 0);
    }
}
