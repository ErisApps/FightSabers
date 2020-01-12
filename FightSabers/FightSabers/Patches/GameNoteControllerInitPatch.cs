﻿using Harmony;
using System;
using System.Collections.Generic;
using FightSabers.Core;
using FightSabers.Models.Abstracts;
using FightSabers.Models.Modifiers;

namespace FightSabers.Patches
{
    [HarmonyPatch(typeof(GameNoteController))]
    [HarmonyPatch("Awake")]
    [HarmonyPatch(new Type[] { })]
    internal class GameNoteControllerInitPatch
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
                if (MonsterGenerator.instance)
                {
                    void OnMonsterAdded(object self)
                    {
                        //Logger.log.Debug(">>>>>> Enabling modifiers to " + MonsterGenerator.instance.CurrentMonster.gameObject.name);
                        foreach (var monsterModifier in MonsterGenerator.instance.CurrentMonster.Modifiers)
                        {
                            if (__instance.noteTransform.gameObject.GetComponent(monsterModifier) is Modifier modifier)
                                modifier.EnableModifier();
                        }
                    }

                    void OnMonsterRemoved(object self)
                    {
                        //Logger.log.Debug(">>>>>> Disabling modifiers to " + MonsterGenerator.instance.CurrentMonster.gameObject.name);
                        foreach (var monsterModifier in MonsterGenerator.instance.CurrentMonster.Modifiers)
                        {
                            if (__instance.noteTransform.gameObject.GetComponent(monsterModifier) is Modifier modifier)
                                modifier.DisableModifier();
                        }
                    }
                    
                    MonsterGenerator.instance.MonsterAdded += OnMonsterAdded;
                    MonsterGenerator.instance.MonsterRemoved += OnMonsterRemoved;
                }
            }
        }
    }
}