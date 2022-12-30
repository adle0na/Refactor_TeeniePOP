// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

namespace InGameScene.UI {
    //===========================================================
    // 아이템 인벤토리에 대한 UI
    //===========================================================
    public class InGameUI_ItemInventory : InGameUI_BottomUIBase {
        [SerializeField] private GameObject _ItemParentObject;
        [SerializeField] private GameObject _useItemPrefab;

        // 중복 아이템을 count로 보여주기 위해 Dictionary로 구현
        private Dictionary<int, InGameUI_UseItem> _useItemDic = new Dictionary<int, InGameUI_UseItem>();

        // ItemInventory에 있는 
        public override void Init() {
            foreach (var shopChartItem in StaticManager.Backend.GameData.ItemInventory.Dictionary) {
                var newItem = Instantiate(_useItemPrefab, _ItemParentObject.transform, true);
                newItem.transform.localPosition = new Vector3(0, 0, 0);
                newItem.transform.localScale = new Vector3(1, 1, 1);

                var useItem = newItem.GetComponent<InGameUI_UseItem>();
                useItem.Init(this, shopChartItem.Key, shopChartItem.Value);

                _useItemDic.Add(shopChartItem.Key, useItem);
            }
        }

        // 아이템이 존재할 경우, 해당 아이템에서 보여주는 갯수를 1증가
        // 아이템이 존재하지 않을 경우, 새로운 아이템 클래스 추가
        public void UpdateUI(int item, int count) {
            if (_useItemDic.ContainsKey(item)) {
                _useItemDic[item].Update();
            }
            else {
                var newItem = Instantiate(_useItemPrefab, _ItemParentObject.transform, true);
                newItem.transform.localPosition = new Vector3(0, 0, 0);
                newItem.transform.localScale = new Vector3(1, 1, 1);

                var useItem = newItem.GetComponent<InGameUI_UseItem>();
                useItem.Init(this, item, count);
                _useItemDic.Add(item, useItem);
            }
        }

        // 아이템을 다 사용하였을 때, 갯수가 0이 될 경우, 리스트에서 제거(InGameUI_UseItem 클래스에서 사용)
        public void DeleteUseItem(int itemID) {
            Destroy(_useItemDic[itemID].gameObject);
            _useItemDic.Remove(itemID);
        }
    }
}