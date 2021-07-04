using System;
using System.Collections.Generic;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.GameplaySetup;
using FightSabers.Core;
using FightSabers.Installers;
using FightSabers.Models;
using FightSabers.Models.Modifiers;
using FightSabers.Patches;
using FightSabers.Settings;
using FightSabers.UI.Controllers;
using FightSabers.Utilities;
using HarmonyLib;
using HMUI;
using IPA;
using IPA.Config.Stores;
using IPA.Loader;
using SiraUtil.Zenject;
using UnityEngine;
using UnityEngine.UI;
using Config = IPA.Config.Config;

namespace FightSabers
{
	[Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
	    private const string HARMONY_ID = "be.erisapps.FightSabers";

	    private Harmony? _harmonyInstance;

	    public static SceneState CurrentSceneState { get; private set; } = SceneState.Menu;

        [Init]
        public void Init(Logger logger, PluginMetadata metadata, Config config, Zenjector zenjector)
        {
	        zenjector.OnApp<FightSAppInstaller>().WithParameters(logger, config.Generated<PluginConfig>());
	        zenjector.OnMenu<FightSMenuInstaller>();
	        zenjector.OnGame<FightSGameInstaller>();
        }

        [OnEnable]
        public void OnEnable()
        {
	        _harmonyInstance = new Harmony(HARMONY_ID);
	        _harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

            GameplaySetup.instance.AddTab("FS Modifiers", "FightSabers.UI.Views.FightSabersGameplaySetupView.bsml", FightSabersGameplaySetup.instance);
        }

        [OnDisable]
        public void OnDisable()
        {
	        _harmonyInstance?.UnpatchAll(HARMONY_ID);
	        _harmonyInstance = null;
        }

        private static void MenuLoadFresh()
        {
            SaveDataManager.instance.Setup();
            QuestManager.instance.LoadQuests();
            ExperienceSystem.instance.Setup();
            ExperienceSystem.instance.FixOverflowedExperience();
            ExperienceSystem.instance.ApplyExperienceFinished += delegate { SaveDataManager.instance.ApplyToFile(); };

            //ExperienceSystem.instance.Invoke("TestLevel", 5f); //TODO: Remove later, FPFC testing
            var floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(120, 52f), true,
                                                                     config.Value.FSPanelPosition,
                                                                     Quaternion.Euler(config.Value.FSPanelRotation));
            floatingScreen.screenMover.OnRelease += (pos, rot) => {
                config.Value.FSPanelPosition = new Vector3(pos.x, pos.y, pos.z);
                config.Value.FSPanelRotation = new Vector3(rot.eulerAngles.x, rot.eulerAngles.y, rot.eulerAngles.z);
                configProvider.Store(config.Value);
            };
            floatingScreen.SetRootViewController(BeatSaberUI.CreateViewController<OverlayViewController>(), ViewController.AnimationType.None);
            floatingScreen.GetComponent<Image>().enabled = false;
        }

        private static void OnMenuSceneActive()
        {
            if (CurrentSceneState == SceneState.Menu)
            {
	            return;
            }

            if (config.Value.Enabled)
            {
                ExperienceSystem.instance.ApplyExperience();
                QuestManager.instance.UnlinkGameEventsForActivatedQuests();
            }

            if (ModifierManager.instance)
            {
	            ModifierManager.instance.modifiers = new Type[]{};
            }

            CurrentSceneState = SceneState.Menu;
        }

        private static void OnGameSceneActive()
        {
            if (CurrentSceneState == SceneState.Game)
            {
	            return;
            }

            if (GameNoteControllerAwakePatch.colorSuckers == null)
            {
	            GameNoteControllerAwakePatch.colorSuckers = new List<ColorSucker>();
            }
            else
            {
	            GameNoteControllerAwakePatch.colorSuckers.Clear();
            }

            if (config.Value.Enabled && (FightSabersGameplaySetup.instance.ColorSuckerEnabled  ||
                                         FightSabersGameplaySetup.instance.NoteShrinkerEnabled ||
                                         FightSabersGameplaySetup.instance.TimeWarperEnabled))
            {
                config.Value.Enabled = false;
                OverlayViewController.instance.fsDisableContainerState = !config.Value.Enabled;
                OverlayViewController.instance.experienceContainerState = config.Value.Enabled;
                configProvider.Store(config.Value);
            }
            if (config.Value.Enabled)
            {
                ScoreSubmission.DisableSubmission("FightSabers");
                MonsterGenerator.Create();
                QuestManager.instance.LinkGameEventsForActivatedQuests();
            }
            else if (FightSabersGameplaySetup.instance.ColorSuckerEnabled || FightSabersGameplaySetup.instance.NoteShrinkerEnabled || FightSabersGameplaySetup.instance.TimeWarperEnabled)
            {
                ScoreSubmission.DisableSubmission("FightSabers");
                var go = new GameObject("[FS|ModifierManager]");
                var modifierManager = go.AddComponent<ModifierManager>();
                var modifiers = new List<Type>();
                if (FightSabersGameplaySetup.instance.ColorSuckerEnabled)
                {
	                modifiers.Add(typeof(ColorSucker));
                }

                if (FightSabersGameplaySetup.instance.NoteShrinkerEnabled)
                {
	                modifiers.Add(typeof(NoteShrinker));
                }

                if (FightSabersGameplaySetup.instance.TimeWarperEnabled)
                {
	                modifiers.Add(typeof(TimeWarper));
                }

                modifierManager.modifiers = modifiers.ToArray();
                modifierManager.noteShrinkerStrength = FightSabersGameplaySetup.instance.NoteShrinkerStrength;
                modifierManager.colorSuckerStrength = FightSabersGameplaySetup.instance.ColorSuckerStrength;
                modifierManager.timeWarperStrength = FightSabersGameplaySetup.instance.TimeWarperStrength;
                new UnityTask(modifierManager.ConfigureModifiers(0.05f));
                var scoreControllerManager = go.AddComponent<ScoreControllerManager>();
                scoreControllerManager.BombCut += self => {
                    modifierManager.ReduceColorSuckerColorness();
                };
                scoreControllerManager.NoteCut += self => {
                    modifierManager.ImproveColorSuckerColorness();
                };
                scoreControllerManager.NoteMissed += self => {
                    modifierManager.ReduceColorSuckerColorness();
                };
            }
            CurrentSceneState = SceneState.Game;
        }
    }
}