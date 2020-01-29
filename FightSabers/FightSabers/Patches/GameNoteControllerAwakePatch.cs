using Harmony;
using System;
using System.Collections.Generic;
using FightSabers.Core;
using FightSabers.Models;
using FightSabers.Models.Abstracts;
using FightSabers.Models.Modifiers;
using FightSabers.UI.Controllers;

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
                {
                    var noteShrinker = __instance.noteTransform.gameObject.AddComponent<NoteShrinker>();
                    noteShrinker.gameNoteController = __instance;
                    noteShrinker.strength = ModifierManager.instance != null ? ModifierManager.instance.noteShrinkerStrength : 1;

                }
                if (!__instance.noteTransform.gameObject.GetComponent<ColorSucker>())
                {
                    var colorSucker = __instance.noteTransform.gameObject.AddComponent<ColorSucker>();
                    colorSucker.gameNoteController = __instance;
                    colorSucker.strength = ModifierManager.instance != null ? ModifierManager.instance.colorSuckerStrength : 1;
                    colorSuckers.Add(colorSucker);
                }

                void OnMonsterAdded(object self, MonsterStatus status)
                {
                    if (!ModifierManager.instance) return;
                    foreach (var monsterModifier in ModifierManager.instance.modifiers)
                    {
                        if (__instance.noteTransform.gameObject.GetComponent(monsterModifier) is Modifier modifier)
                            modifier.EnableModifier();
                    }
                }

                void OnMonsterRemoved(object self, MonsterStatus status)
                {
                    if (!ModifierManager.instance) return;
                    foreach (var monsterModifier in ModifierManager.instance.modifiers)
                    {
                        if (__instance.noteTransform.gameObject.GetComponent(monsterModifier) is Modifier modifier)
                            modifier.DisableModifier();
                    }
                }

                if (!Plugin.config.Value.Enabled && ModifierManager.instance != null)
                {
                    ModifierManager.instance.noteCountDuration = (int)Math.Ceiling(35 * FightSabersGameplaySetup.instance.ColorSuckerStrength);
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