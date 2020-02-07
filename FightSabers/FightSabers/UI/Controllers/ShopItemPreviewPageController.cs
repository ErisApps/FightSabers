using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using FightSabers.Core.ExclusiveContent;
using FightSabers.Models.Abstracts;
using FightSabers.Models.Interfaces;
using FightSabers.Models.RewardItems;
using UnityEngine;

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

        [UIParams]
        private BSMLParserParams parserParams;

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            base.DidActivate(firstActivation, type);
            
            PreparePreviewObject();
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            base.DidDeactivate(deactivationType);
            ClearPreview();
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
            switch (rewardItem)
            {
                case SaberReward _:
                    GenerateSaberPreview(rewardItem.name);
                    break;
                case AvatarReward _:
                case NoteReward _:
                case PlatformReward _:
                case WallReward _:
                case RewardItem _:
                    break;
            }
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
