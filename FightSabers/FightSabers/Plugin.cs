using BS_Utils.Utilities;
using FightSabers.Core;
using FightSabers.Models;
using FightSabers.Settings;
using FightSabers.UI;
using FightSabers.Utilities;
using IPA;
using IPA.Config;
using IPA.Utilities;
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

        public static SceneState CurrentSceneState { get; private set; } = SceneState.Menu;

        public static string Name    => "FightSabers";
        public static string Version => "0.1.0a";

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
            BSEvents.menuSceneActive += OnMenuSceneActive;
            BSEvents.gameSceneActive += OnGameSceneActive;
        }

        public void OnApplicationQuit() { }

        public void OnFixedUpdate() { }

        public void OnUpdate() { }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (nextScene.name == "GameCore")
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
            ExperienceSystem.instance.ApplyExperienceFinished += delegate {
                SaveDataManager.instance.ApplyToFile();
            };
            FightSabersProgress.instance.Setup();
            //ExperienceSystem.instance.Invoke("TestLevel", 5f); //TODO: Remove later, FPFC testing
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