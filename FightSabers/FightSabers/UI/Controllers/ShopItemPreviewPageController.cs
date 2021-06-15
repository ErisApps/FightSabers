using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BS_Utils.Utilities;
using CustomFloorPlugin;
using FightSabers.Core.ExclusiveContent;
using FightSabers.Models.Abstracts;
using FightSabers.Models.Interfaces;
using FightSabers.Models.RewardItems;
using FightSabers.Utilities;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using ReflectionUtil = BS_Utils.Utilities.ReflectionUtil;

namespace FightSabers.UI.Controllers
{
    internal class ShopItemPreviewPageController : FightSabersViewController
    {
        public override string ResourceName => "FightSabers.UI.Views.ShopItemPreviewPageView.bsml";
        public override string ContentFilePath => "D:\\Bibliotheques\\Documents\\GitHub\\FightSabers\\FightSabers\\FightSabers\\UI\\Views\\ShopItemPreviewPageView.bsml";
        
        public bool IsGeneratingPreview { get; private set; }

        private GameObject _preview;

        // Sabers
        private GameObject _sabers;
        // SaberPositions (Local to the previewer)
        private Vector3 sabersPos     = new Vector3(0, 0, 0);
        private Vector3 saberLeftPos  = new Vector3(0, 0, 0);
        private Vector3 saberRightPos = new Vector3(0, 0.5f, 0);

        //Platforms
        private CustomFloorPlugin.CustomPlatform _previewPlatform;

        [UIParams]
        private BSMLParserParams parserParams;

        private IRewardItem _currentReward;

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            base.DidActivate(firstActivation, type);
            
            PreparePreviewObject();
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            base.DidDeactivate(deactivationType);
            if (_currentReward is SaberReward)
                ClearPreview();
            else if (_currentReward is PlatformReward)
            {
                var asmType = PlatformManager.Instance.GetType();
                var mi = asmType.GetMethod("InternalTempChangeToPlatform", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(int) }, null);
                Logger.log.Debug($"mi is not null: {mi != null}");
                mi?.Invoke(null, new object[] { 0 });
                var list = PlatformManager.Instance.GetPlatforms().ToList();
                list.RemoveAt(list.Count - 1);
                var info = typeof(PlatformManager).GetField("platforms", BindingFlags.NonPublic | BindingFlags.Static);
                info?.SetValue(null, list.ToArray());
            }
            _currentReward = null;
        }

        #region Utils

        private static GameObject InstantiateGameObject(GameObject go, Transform transform = null)
        {
            if (go)
                return transform ? Instantiate(go, transform) : Instantiate(go);
            return null;
        }

        private static void PositionPreview(Vector3 vector, GameObject go)
        {
            if (go)
                go.transform.localPosition = vector;
        }

        private static void DestroyGameObject(ref GameObject go)
        {
            if (!go) return;
            Destroy(go);
            go = null;
        }

        private void ClearPreview()
        {
            DestroyGameObject(ref _preview);
            ClearSabers();
        }

        #endregion

        #region Sabers preview

        public void OpenPreview(IRewardItem rewardItem)
        {
            _currentReward = rewardItem;
            switch (rewardItem)
            {
                case SaberReward _:
                    GenerateSaberPreview(rewardItem.name);
                    break;
                case AvatarReward _:
                case NoteReward _:
                case PlatformReward _:
                    TestStuff();
                    break;
                case WallReward _:
                case RewardItem _:
                    break;
            }
        }

        private static CustomFloorPlugin.CustomPlatform AddPlatformFromEmbedded(string embeddedPath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(embeddedPath))
            {
                var bundle = AssetBundle.LoadFromStream(stream);
                var asm = Assembly.Load("CustomFloorPlugin");
                var platLoaderType = asm.GetType("CustomFloorPlugin.PlatformLoader");
                Logger.log.Debug($"platLoaderType is not null: {platLoaderType != null}");
                var customPlatform = PlatformManager.Instance.GetField("platformLoader")?.
                                                     InvokeMethod("LoadPlatform", bundle, PlatformManager.Instance.transform) as CustomFloorPlugin.CustomPlatform;
                Logger.log.Debug($"customPlatform is not null: {customPlatform != null}");
                if (customPlatform != null)
                {
                    var list = PlatformManager.Instance.GetPlatforms().ToList();
                    list.Add(customPlatform);
                    foreach (var platform in list)
                    {
                        Logger.log.Debug($"p: {platform}");
                    }
                    var info = typeof(PlatformManager).GetField("platforms", BindingFlags.NonPublic | BindingFlags.Static);
                    info?.SetValue(null, list.ToArray());
                }
                return customPlatform;
            }
        }

        private void TestStuff()
        {
            var asmType = PlatformManager.Instance.GetType();
            var plat = AddPlatformFromEmbedded("FightSabers.Rewards.Light Disc.plat");
            Logger.log.Debug($"plat is not null: {plat != null}");
            var mi = asmType.GetMethod("InternalTempChangeToPlatform", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(int) }, null);
            Logger.log.Debug($"mi is not null: {mi != null}");
            mi?.Invoke(null, new object[] { PlatformManager.Instance.GetPlatforms().Length - 1 });
        }

        private void PreparePreviewObject()
        {
            if (!_preview)
            {
                _preview = new GameObject();
                _preview.transform.position = new Vector3(2.2f, 1.3f, 1.0f);
                _preview.transform.Rotate(0.0f, 330.0f, 0.0f);
            }
        }

        private void GenerateSaberPreview(string saberName)
        {
            if (IsGeneratingPreview) return;
            try
            {
                IsGeneratingPreview = true;
                ClearSabers();
                var customSaber = ExclusiveSabersManager.instance.ExclusiveSaberData[saberName];
                if (customSaber == null) return;
                PreparePreviewObject();
                _sabers = CreatePreviewSaber(customSaber.Sabers, _preview.transform, sabersPos);
                PositionPreview(saberLeftPos, _sabers?.transform.Find("LeftSaber").gameObject);
                PositionPreview(saberRightPos, _sabers?.transform.Find("RightSaber").gameObject);
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }
            finally
            {
                IsGeneratingPreview = false;
            }
        }

        private static GameObject CreatePreviewSaber(GameObject saber, Transform transform, Vector3 localPosition)
        {
            var saberObject = InstantiateGameObject(saber, transform);
            PositionPreview(localPosition, saberObject);
            return saberObject;
        }

        private void ClearSabers()
        {
            DestroyGameObject(ref _sabers);
        }

        #endregion
    }
}
