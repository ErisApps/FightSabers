using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Notify;
using BeatSaberMarkupLanguage.Parser;
using FightSabers.Models.Abstracts;
using FightSabers.Models.RewardItems;
using FightSabers.Settings;
using FightSabers.UI.FlowCoordinators;
using TMPro;
using UnityEngine.UI;

namespace FightSabers.UI.Controllers
{
    internal class ItemShopPageController : FightSabersViewController
    {
        public enum ShopType
        {
            Sabers,
            Platforms,
            Avatars,
            Notes,
            Walls,
            Items
        }
        public override string ResourceName    => "FightSabers.UI.Views.ItemShopPageView.bsml";
        public override string ContentFilePath => "D:\\Bibliotheques\\Documents\\GitHub\\FightSabers\\FightSabers\\FightSabers\\UI\\Views\\ItemShopPageView.bsml";

        public ShopType shopType;

        private string _headerShop;
        [UIValue("header-shop")]
        public string HeaderShop
        {
            get { return _headerShop; }
            private set
            {
                _headerShop = value;
                NotifyPropertyChanged();
            }
        }

        private string _modalItemName;
        [UIValue("modal-item-name")]
        public string ModalItemName
        {
            get { return _modalItemName; }
            private set
            {
                _modalItemName = value;
                NotifyPropertyChanged();
            }
        }

        private string _modalItemDesc;
        [UIValue("modal-item-desc")]
        public string ModalItemDesc
        {
            get { return _modalItemDesc; }
            private set
            {
                _modalItemDesc = value;
                NotifyPropertyChanged();
            }
        }

        private string _itemOwnedLabel;
        [UIValue("modal-item-owned-label")]
        public string ItemOwnedLabel
        {
            get { return _itemOwnedLabel; }
            private set
            {
                _itemOwnedLabel = value;
                NotifyPropertyChanged();
            }
        }

        private bool _itemOwnedState;
        [UIValue("modal-item-owned-state")]
        public bool ItemOwnedState
        {
            get { return _itemOwnedState; }
            private set
            {
                _itemOwnedState = value;
                NotifyPropertyChanged();
            }
        }

        private string _itemPrice;
        [UIValue("modal-item-price")]
        public string ItemPrice
        {
            get { return _itemPrice; }
            private set
            {
                _itemPrice = value;
                NotifyPropertyChanged();
            }
        }

        [UIParams] private BSMLParserParams parserParams;

        [UIComponent("shop-list")]     private CustomCellListTableData _shopList;
        [UIValue("shop-list-content")] private List<object>            _shopListContent = new List<object>();

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            base.DidActivate(firstActivation, type);
            if (_shopListContent == null)
                _shopListContent = new List<object>();
            else
                _shopListContent.Clear();
            HeaderShop = shopType.ToString("G");
            switch (shopType)
            {
                case ShopType.Sabers:
                    foreach (var item in SaveDataManager.instance.SaveData.rewardableItems.OfType<SaberReward>())
                    {
                        if (item.unlockType != RewardItem.UnlockType.Coins) continue;
                        _shopListContent.Add(new ShopElementItem(this, item));
                        _shopList?.tableView?.ReloadData();
                    }
                    break;
                case ShopType.Platforms:
                    foreach (var item in SaveDataManager.instance.SaveData.rewardableItems.OfType<PlatformReward>())
                    {
                        if (item.unlockType != RewardItem.UnlockType.Coins) continue;
                        _shopListContent.Add(new ShopElementItem(this, item));
                        _shopList?.tableView?.ReloadData();
                    }
                    break;
                case ShopType.Avatars:
                    foreach (var item in SaveDataManager.instance.SaveData.rewardableItems.OfType<AvatarReward>())
                    {
                        if (item.unlockType != RewardItem.UnlockType.Coins) continue;
                        _shopListContent.Add(new ShopElementItem(this, item));
                        _shopList?.tableView?.ReloadData();
                    }
                    break;
                case ShopType.Notes:
                    foreach (var item in SaveDataManager.instance.SaveData.rewardableItems.OfType<NoteReward>())
                    {
                        if (item.unlockType != RewardItem.UnlockType.Coins) continue;
                        _shopListContent.Add(new ShopElementItem(this, item));
                        _shopList?.tableView?.ReloadData();
                    }
                    break;
                case ShopType.Walls:
                    foreach (var item in SaveDataManager.instance.SaveData.rewardableItems.OfType<WallReward>())
                    {
                        if (item.unlockType != RewardItem.UnlockType.Coins) continue;
                        _shopListContent.Add(new ShopElementItem(this, item));
                        _shopList?.tableView?.ReloadData();
                    }
                    break;
                case ShopType.Items: //TODO: WIP
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            base.DidDeactivate(deactivationType);
            parserParams.EmitEvent("close-modal");
        }

        [UIAction("back-act")]
        private void ShowSabersShop()
        {
            flowCoordinatorOwner.ActivatePage(FightSabersFlowCoordinator.PageStatus.Shop);
        }

        internal class ShopElementItem : INotifiableHost
        {
            public event PropertyChangedEventHandler PropertyChanged;

            [UIComponent("item")]       private Button          _itemBtn;
            [UIComponent("item-name")]  private TextMeshProUGUI _itemNameText;
            [UIComponent("item-price")] private TextMeshProUGUI _itemPriceText;

            private ItemShopPageController _controllerOwner;
            private RewardItem _rewardItem;

            public ShopElementItem(ItemShopPageController controllerOwner, RewardItem rewardItem)
            {
                _controllerOwner = controllerOwner;
                _rewardItem = rewardItem;
            }

            [UIAction("#show-new")]
            private void ShowModal()
            {
                _controllerOwner.ModalItemName = _rewardItem.name;
                _controllerOwner.ModalItemDesc = _rewardItem.description;
                _controllerOwner.ItemPrice = _rewardItem.unlockState ? "<color=#00ff00ff><i>Already unlocked!</i></color>" : "Price: " + _itemPriceText.text;
                _controllerOwner.ItemOwnedLabel = _rewardItem.unlockState ? "Owned" : "Buy";
                _controllerOwner.ItemOwnedState = !_rewardItem.unlockState;
                _controllerOwner.parserParams.EmitEvent("show-new");
            }

            [UIAction("#post-parse")]
            internal void Setup()
            {
                ShopMainPageController.DisableBigButtonIcon(_itemBtn);
                _itemNameText.text = _rewardItem.name;
                _itemPriceText.text = "<color=#ffa500ff>" + (_rewardItem.unlockState ? "<i>Unlocked</i>" : _rewardItem.unlockValue + " FightCoins") + "</color>";
            }
        }
    }
}