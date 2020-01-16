using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using BS_Utils.Gameplay;
using BS_Utils.Utilities;
using FightSabers.Core;
using FightSabers.Models;
using FightSabers.Settings;
using FightSabers.UI;
using FightSabers.UI.Controllers;
using FightSabers.Utilities;
using Harmony;
using IPA;
using IPA.Config;
using IPA.Loader;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using Config = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

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
            FightSabersProgress.instance.Setup();

            //ExperienceSystem.instance.Invoke("TestLevel", 5f); //TODO: Remove later, FPFC testing
            var floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(45, 9.5f), false, new Vector3(0, 3.35f, 2.4f), Quaternion.Euler(-15, 0, 0));
            floatingScreen.SetRootViewController(BeatSaberUI.CreateViewController<OverlayViewController>(), true);
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