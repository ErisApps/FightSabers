using Harmony;
using System;
using System.Collections.Generic;
using FightSabers.Core;
using FightSabers.Models;
using FightSabers.Models.Abstracts;
using FightSabers.Models.Modifiers;

namespace FightSabers.Patches
{
    [HarmonyPatch(typeof(GameNoteController))]
    [HarmonyPatch("Awake")]
    [HarmonyPatch(new Type[] { })]
    internal class GameNoteControllerAwakePatch
    {
        public static List<ColorSucker> colorSuckers;

        public static void Postfix(GameNoteController __instance)
        {
            if (colorSuckers == null)
                colorSuckers = new List<ColorSucker>();
            if (__instance)
            {
                if (!__instance.noteTransform.gameObject.GetComponent<NoteShrinker>())
                    __instance.noteTransform.gameObject.AddComponent<NoteShrinker>().gameNoteController = __instance;
                if (!__instance.noteTransform.gameObject.GetComponent<ColorSucker>())
                {
                    var colorSucker = __instance.noteTransform.gameObject.AddComponent<ColorSucker>();
                    colorSucker.gameNoteController = __instance;
                    colorSuckers.Add(colorSucker);
                }

                void OnMonsterAdded(object self, MonsterStatus status)
                {
                    foreach (var monsterModifier in ModifierManager.instance.modifiers)
                    {
                        if (__instance.noteTransform.gameObject.GetComponent(monsterModifier) is Modifier modifier)
                            modifier.EnableModifier();
                    }
                }

                void OnMonsterRemoved(object self, MonsterStatus status)
                {
                    foreach (var monsterModifier in ModifierManager.instance.modifiers)
                    {
                        if (__instance.noteTransform.gameObject.GetComponent(monsterModifier) is Modifier modifier)
                            modifier.DisableModifier();
                    }
                }

                if (!Plugin.config.Value.Enabled)
                {
                    ModifierManager.instance.noteCountDuration = 35;
                    foreach (var monsterModifier in ModifierManager.instance.modifiers)
                    {
                        if (__instance.noteTransform.gameObject.GetComponent(monsterModifier) is Modifier modifier)
                            modifier.EnableModifier();
                    }
                }
                else if (MonsterGenerator.instance)
                {
                    MonsterGenerator.instance.MonsterAdded += OnMonsterAdded;
                    MonsterGenerator.instance.MonsterRemoved += OnMonsterRemoved;
                }
            }
        }
    }
}