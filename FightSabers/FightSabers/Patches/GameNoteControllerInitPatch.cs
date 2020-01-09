using Harmony;
using System;
using BS_Utils.Utilities;
using FightSabers.Core;
using FightSabers.Models.Abstracts;
using FightSabers.Models.Modifiers;
using UnityEngine;

namespace FightSabers.Patches
{
    [HarmonyPatch(typeof(GameNoteController))]
    [HarmonyPatch("Awake")]
    [HarmonyPatch(new Type[] { })]
    class GameNoteControllerInitPatch
    {
        public static void Postfix(GameNoteController __instance)
        {
            if (__instance)
            {
                if (!__instance.noteTransform.gameObject.GetComponent<NoteShrinker>())
                    __instance.noteTransform.gameObject.AddComponent(typeof(NoteShrinker));
                if (MonsterGenerator.instance)
                {
                    void OnMonsterAdded(object self)
                    {
                        //Logger.log.Warn(">>>>>> Enabling modifiers to " + MonsterGenerator.instance.CurrentMonster.gameObject.name);
                        foreach (var monsterModifier in MonsterGenerator.instance.CurrentMonster.Modifiers)
                        {
                            if (__instance.noteTransform.gameObject.GetComponent(monsterModifier) is Modifier modifier)
                                modifier.EnableModifier();
                        }
                    }

                    void OnMonsterRemoved(object self)
                    {
                        //Logger.log.Warn(">>>>>> Disabling modifiers to " + MonsterGenerator.instance.CurrentMonster.gameObject.name);
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