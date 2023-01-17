// Copyright 2013-2022 AFI, INC. All rights reserved.

using UnityEngine;

namespace InGameScene.UI {
    
    //===========================================================
    // 상점 UI
    //===========================================================
    public class InGameUI_Shop : InGameUI_BottomUIBase {
        [SerializeField] private GameObject _shopParentObject;
        [SerializeField] private GameObject _shopItemPrefab;

        public override void Init() {
            foreach (var shopChartItem in StaticManager.Backend.Chart.Shop.List) {
                var newShopItem = Instantiate(_shopItemPrefab, _shopParentObject.transform, true);
                newShopItem.transform.localPosition = new Vector3(0, 0, 0);
                newShopItem.transform.localScale = new Vector3(1, 1, 1);

                newShopItem.GetComponent<InGameUI_ShopItem>().Init(shopChartItem);
            }
        }
    }
}