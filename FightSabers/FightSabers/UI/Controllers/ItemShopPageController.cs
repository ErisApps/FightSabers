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
using UnityEngine;
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

        private string _itemOwnedHint;
        [UIValue("modal-item-owned-hint")]
        public string ItemOwnedHint
        {
            get { return _itemOwnedHint; }
            private set
            {
                _itemOwnedHint = value;
                NotifyPropertyChanged();
            }
        }

        private string _itemOwnedGlow = "transparent";
        [UIValue("modal-item-owned-glow")]
        public string ItemOwnedGlow
        {
            get { return _itemOwnedGlow; }
            private set
            {
                _itemOwnedGlow = value;
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

        private string _dialogDesc;
        [UIValue("dialog-desc")]
        public string DialogDesc
        {
            get { return _dialogDesc; }
            private set
            {
                _dialogDesc = value;
                NotifyPropertyChanged();
            }
        }

        private string _dialogRemainingMoney;
        [UIValue("dialog-remaining-money")]
        public string DialogRemainingMoney
        {
            get { return _dialogRemainingMoney; }
            private set
            {
                _dialogRemainingMoney = value;
                NotifyPropertyChanged();
            }
        }

        private bool _dialogRemainingMoneyState = true;
        [UIValue("dialog-remaining-money-state")]
        public bool DialogRemainingMoneyState
        {
            get { return _dialogRemainingMoneyState; }
            private set
            {
                _dialogRemainingMoneyState = value;
                NotifyPropertyChanged();
            }
        }

        private bool _dialogConfirmState = true;
        [UIValue("dialog-confirm-state")]
        public bool DialogConfirmState
        {
            get { return _dialogConfirmState; }
            private set
            {
                _dialogConfirmState = value;
                NotifyPropertyChanged();
            }
        }

        private string _dialogCancelText = "Cancel";
        [UIValue("dialog-cancel-text")]
        public string DialogCancelText
        {
            get { return _dialogCancelText; }
            private set
            {
                _dialogCancelText = value;
                NotifyPropertyChanged();
            }
        }

        private int _dialogHeight = 55;
        [UIValue("dialog-height")]
        public int DialogHeight
        {
            get { return _dialogHeight; }
            private set
            {
                _dialogHeight = value;
                NotifyPropertyChanged();
            }
        }

        [UIParams] private BSMLParserParams parserParams;

        [UIComponent("shop-list")]     private CustomCellListTableData _shopList;
        [UIValue("shop-list-content")] private List<object>            _shopListContent = new List<object>();

        public RewardItem currentRewardItem;
        private ShopElementItem currentShopElementItem;

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

        [UIAction("close-modal")]
        private void CloseModal()
        {
            parserParams.EmitEvent("close-modal");
            flowCoordinatorOwner.DisplayShopItemPreview(false);
            currentRewardItem = null;
        }

        [UIAction("show-confirm-dialog")]
        private void DisplayConfirmDialog()
        {
            parserParams.EmitEvent("close-modal");
            DialogDesc = $"Are you sure you want to buy <color=#ffa500ff>{currentRewardItem?.name}</color>?";
            var coinValue = Convert.ToInt32(currentRewardItem.unlockValue);
            DialogRemainingMoney = $"Remaining FightCoins after purchasing: <color=#ffa500ff>{SaveDataManager.instance.SaveData.fightCoinsAmount - coinValue}</color>";
            parserParams.EmitEvent("show-confirm-dialog");
        }

        [UIAction("close-confirm-dialog")]
        private void CloseConfirmModal()
        {
            parserParams.EmitEvent("close-dialog-modal");
            DialogCancelText = "Cancel";
            DialogConfirmState = true;
            DialogHeight = 55;
            parserParams.EmitEvent("show-new");
        }

        [UIAction("confirm-buy-dialog")]
        private void ConfirmBuyModal()
        {
            currentRewardItem.UnlockItem();
            var coinValue = Convert.ToInt32(currentRewardItem.unlockValue);
            SaveDataManager.instance.Pay(coinValue);
            DialogDesc = "Thank you for your purchase!";
            DialogCancelText = "Go back";
            DialogRemainingMoneyState = DialogConfirmState = false;
            DialogHeight = 40;
            //TODO: Maybe some refactor below?
            currentShopElementItem.itemPriceText.text = "<color=#00ff00ff><i>Unlocked</i></color>";
            ItemPrice = currentRewardItem.unlockState ? "<color=#00ff00ff><i>Already unlocked!</i></color>" : "Price: " + coinValue;
            ItemOwnedLabel = currentRewardItem.unlockState ? "Owned" : "Buy";
            var hasEnoughMoney = SaveDataManager.instance.SaveData.fightCoinsAmount >= coinValue;
            ItemOwnedGlow = currentRewardItem.unlockState ? "transparent" : hasEnoughMoney ? "green" : "red";
            ItemOwnedState = hasEnoughMoney && !currentRewardItem.unlockState;
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            base.DidDeactivate(deactivationType);
            currentRewardItem = null;
            parserParams.EmitEvent("close-modal");
            parserParams.EmitEvent("close-dialog-modal");
        }

        [UIAction("back-act")]
        private void ShowSabersShop()
        {
            flowCoordinatorOwner.ActivatePage(FightSabersFlowCoordinator.PageStatus.Shop);
        }

        internal class ShopElementItem : INotifiableHost
        {
            public event PropertyChangedEventHandler PropertyChanged;

            [UIComponent("item")] public Button itemBtn;
            [UIComponent("item-name")] public TextMeshProUGUI itemNameText;
            [UIComponent("item-price")] public TextMeshProUGUI itemPriceText;

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
                _controllerOwner.currentShopElementItem = this;
                _controllerOwner.currentRewardItem = _rewardItem;
                _controllerOwner.ModalItemName = _rewardItem.name;
                _controllerOwner.ModalItemDesc = _rewardItem.description;
                _controllerOwner.ItemPrice = _rewardItem.unlockState ? "<color=#00ff00ff><i>Already unlocked!</i></color>" : "Price: " + itemPriceText.text;
                _controllerOwner.ItemOwnedLabel = _rewardItem.unlockState ? "Owned" : "Buy";
                var coinValue = Convert.ToInt32(_rewardItem.unlockValue);
                var hasEnoughMoney = SaveDataManager.instance.SaveData.fightCoinsAmount >= coinValue;
                _controllerOwner.ItemOwnedGlow = _rewardItem.unlockState ? "transparent" : hasEnoughMoney ? "green" : "red";
                _controllerOwner.ItemOwnedHint =  hasEnoughMoney || _rewardItem.unlockState ? "" : "<color=#ff2a2aff>Not enough FightCoins!</color>";
                _controllerOwner.ItemOwnedState = hasEnoughMoney && !_rewardItem.unlockState;
                if (_rewardItem is SaberReward || _rewardItem is PlatformReward)
                    _controllerOwner.flowCoordinatorOwner.DisplayShopItemPreview(true, _rewardItem);
                _controllerOwner.parserParams.EmitEvent("show-new");
            }

            [UIAction("#post-parse")]
            internal void Setup()
            {
                ShopMainPageController.DisableBigButtonIcon(itemBtn);
                itemNameText.text = _rewardItem.name;
                itemPriceText.text = $"<color=#{(_rewardItem.unlockState ? "00ff00ff" : "ffa500ff")}>{(_rewardItem.unlockState ? "<i>Unlocked</i>" : _rewardItem.unlockValue + " FightCoins")}</color>";
            }
        }
    }
}