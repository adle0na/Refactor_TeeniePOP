// Copyright 2013-2022 AFI, INC. All rights reserved.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene.UI {
    //===========================================================
    // 상점 UI의 아이템 클래스
    //===========================================================
    public class InGameUI_ShopItem : MonoBehaviour {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _itemNameText;
        [SerializeField] private TMP_Text _itemInfoText;
        [SerializeField] private TMP_Text _itemPriceText;
        [SerializeField] private Button _itemBuyButton;

        private BackendData.Chart.Shop.Item _shopItemInfo;

        //구매 시 실행되는 함수
        private delegate void BuyProcessFunc();

        //Shop에 정보에 맞춰 아이템 생성
        public void Init(BackendData.Chart.Shop.Item shopItemInfo) {
            _shopItemInfo = shopItemInfo;


            string priceString = shopItemInfo.ItemPrice.ToString();

            // 재화가 골드인지 쥬얼인지 구별(현재 쥬얼은 제공X)
            switch (_shopItemInfo.ItemPriceType) {
                case BackendData.Chart.Shop.Item.ItemPriceTypeEnum.Gold:
                    priceString += "골드";
                    break;
                case BackendData.Chart.Shop.Item.ItemPriceTypeEnum.Jewel:
                    priceString += "쥬얼";
                    break;
            }
            
            _itemPriceText.text = priceString;

            // 판매하는 아이템이 Item 개열인지 혹은 다른 개열인지 판단
            // 현재는 item 만 제공
            switch (_shopItemInfo.ItemType) {
                case BackendData.Chart.Shop.Item.ItemTypeEnum.Item:
                    SetItem(_shopItemInfo.ItemID);
                    break;
            }
        }

        
        // 각 아이템에 맞는 차트를 불러와 데이터 적용
        void SetItem(int itemID) {
            BackendData.Chart.Item.Item data = StaticManager.Backend.Chart.Item.Dictionary[itemID];

            _image.sprite = data.ImageSprite;
            _itemNameText.text = data.ItemName;
            _itemInfoText.text = data.ItemContent;

            // 구매 버튼 클릭 시 아이템 인벤토리에 아이템 넣기
            _itemBuyButton.onClick.AddListener(() => BuyButton(() => {
                InGameScene.Managers.Game.UpdateItemInventory(itemID, 1);
            }));
        }

        // 구매 버튼 클릭시, UserData의 money와 아이템의 가격을 비교
        // 구매 가능할 경우 각 버튼에 커스텀으로 할당된 BuyProcessFunc 실행.
        // UserData에 아이템 가격만큼 감소
        void BuyButton(BuyProcessFunc func) {
            switch (_shopItemInfo.ItemPriceType) {
                case BackendData.Chart.Shop.Item.ItemPriceTypeEnum.Gold:
                    if (StaticManager.Backend.GameData.UserData.Money < _shopItemInfo.ItemPrice) {
                        StaticManager.UI.AlertUI.OpenWarningUI("구매 불가", "골드가 부족하여 해당 아이템을 구매할 수 없습니다");
                    }

                    break;
                case BackendData.Chart.Shop.Item.ItemPriceTypeEnum.Jewel:
                    if (StaticManager.Backend.GameData.UserData.Jewel < _shopItemInfo.ItemPrice) {
                        StaticManager.UI.AlertUI.OpenWarningUI("구매 불가", "쥬얼이 부족하여 해당 아이템을 구매할 수 없습니다");
                    }

                    break;
            }
            func.Invoke();
            InGameScene.Managers.Game.UpdateUserData(-_shopItemInfo.ItemPrice, 0);
            StaticManager.UI.AlertUI.OpenAlertUI("구매 완료", _itemNameText.text + "이(가) 구매 완료되었습니다.");
        }
    }
}