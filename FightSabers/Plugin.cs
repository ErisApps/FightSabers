using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.GameplaySetup;
using BS_Utils.Gameplay;
using BS_Utils.Utilities;
using FightSabers.Core;
using FightSabers.Models;
using FightSabers.Models.Modifiers;
using FightSabers.Patches;
using FightSabers.Settings;
using FightSabers.UI.Controllers;
using FightSabers.Utilities;
using Harmony;
using IPA;
using IPA.Config;
using IPA.Loader;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Config = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace FightSabers
{
    public class Plugin : IBeatSaberPlugin
    {
        #region Properties
        internal static Ref<PluginConfig>           config;
        internal static IConfigProvider             configProvider;
        internal static PluginLoader.PluginMetadata fightSabersMetadata;

        public static SceneState CurrentSceneState { get; private set; } = SceneState.Menu;
        #endregion

        #region BSIPA events
        public void Init(IPALogger logger, [Config.Prefer("json")] IConfigProvider cfgProvider)
        {
            Logger.log = logger;
            BSEvents.menuSceneLoadedFresh += MenuLoadFresh;
            configProvider = cfgProvider;

            config = cfgProvider.MakeLink<PluginConfig>((p, v) => {
                if (v.Value == null || v.Value.RegenerateConfig)
                    p.Store(v.Value = new PluginConfig() { RegenerateConfig = false });
                config = v;
            });
        }

        public void OnApplicationStart()
        {
            var harmony = HarmonyInstance.Create("com.Shoko84.beatsaber.FightSabers");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            fightSabersMetadata = PluginManager.AllPlugins.Select(x => x.Metadata).First(x => x.Name == "FightSabers");
            BSEvents.menuSceneActive += OnMenuSceneActive;
            BSEvents.gameSceneActive += OnGameSceneActive;
            GameplaySetup.instance.AddTab("FS Modifiers", "FightSabers.UI.Views.FightSabersGameplaySetupView.bsml", FightSabersGameplaySetup.instance);
        }

        public void OnApplicationQuit() { }

        public void OnFixedUpdate() { }

        public void OnUpdate() { }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene) { }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode) { }

        public void OnSceneUnloaded(Scene scene) { }
        #endregion

        #region Custom events
        private static void MenuLoadFresh()
        {
            SaveDataManager.instance.Setup();
            QuestManager.instance.LoadQuests();
            ExperienceSystem.instance.Setup();
            ExperienceSystem.instance.FixOverflowedExperience();
            ExperienceSystem.instance.ApplyExperienceFinished += delegate { SaveDataManager.instance.ApplyToFile(); };

            //ExperienceSystem.instance.Invoke("TestLevel", 5f); //TODO: Remove later, FPFC testing
            var floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(120, 52f), true,
                                                                     Float3.ToVector3(config.Value.FSPanelPosition),
                                                                     Quaternion.Euler(Float3.ToVector3(config.Value.FSPanelRotation)));
            floatingScreen.screenMover.OnRelease += (pos, rot) => {
                config.Value.FSPanelPosition = new Float3(pos.x, pos.y, pos.z);
                config.Value.FSPanelRotation = new Float3(rot.eulerAngles.x, rot.eulerAngles.y, rot.eulerAngles.z);
                configProvider.Store(config.Value);
            };
            floatingScreen.SetRootViewController(BeatSaberUI.CreateViewController<OverlayViewController>(), true);
            floatingScreen.GetComponent<Image>().enabled = false;
        }

        private static void OnMenuSceneActive()
        {
            if (CurrentSceneState == SceneState.Menu) return;
            if (config.Value.Enabled)
            {
                ExperienceSystem.instance.ApplyExperience();
                QuestManager.instance.UnlinkGameEventsForActivatedQuests();
            }
            if (ModifierManager.instance)
                ModifierManager.instance.modifiers = new Type[]{};
            CurrentSceneState = SceneState.Menu;
        }

        private static void OnGameSceneActive()
        {
            if (CurrentSceneState == SceneState.Game) return;
            if (GameNoteControllerAwakePatch.colorSuckers == null)
                GameNoteControllerAwakePatch.colorSuckers = new List<ColorSucker>();
            else
                GameNoteControllerAwakePatch.colorSuckers.Clear();
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
                    modifiers.Add(typeof(ColorSucker));
                if (FightSabersGameplaySetup.instance.NoteShrinkerEnabled)
                    modifiers.Add(typeof(NoteShrinker));
                if (FightSabersGameplaySetup.instance.TimeWarperEnabled)
                    modifiers.Add(typeof(TimeWarper));
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
        #endregion
    }
}