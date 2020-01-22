using System;
using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using BS_Utils.Utilities;
using FightSabers.Core;
using FightSabers.Models;
using FightSabers.Settings;
using FightSabers.UI.Controllers;
using FightSabers.Utilities;
using Harmony;
using IPA;
using IPA.Config;
using IPA.Loader;
using IPA.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Config = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;
using Random = UnityEngine.Random;

namespace FightSabers
{
    public class Plugin : IBeatSaberPlugin
    {
        #region Properties
        internal static Ref<PluginConfig> config;
        internal static IConfigProvider   configProvider;
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
            Random.InitState((int)DateTime.Now.Ticks);
        }

        public void OnApplicationQuit() { }

        public void OnFixedUpdate() { }

        public void OnUpdate() { }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (Plugin.config.Value.Enabled && nextScene.name == "GameCore")
                MonsterGenerator.Create();
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode) { }

        public void OnSceneUnloaded(Scene scene) { }
        #endregion

        #region Custom events
        private static void MenuLoadFresh()
        {
            SaveDataManager.instance.Setup();
            ExperienceSystem.instance.Setup();
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
            new UnityTask(ExperienceSystem.instance.ApplyExperience());
            CurrentSceneState = SceneState.Menu;
        }

        private static void OnGameSceneActive()
        {
            if (CurrentSceneState == SceneState.Game) return;
            CurrentSceneState = SceneState.Game;
        }
        #endregion
    }
}