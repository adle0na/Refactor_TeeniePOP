// Copyright 2013-2022 AFI, INC. All rights reserved.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene.UI {
    //===========================================================
    // 아이템 인벤토리에서 사용하는 아이템의 아이템 클래스
    //===========================================================
    public class InGameUI_UseItem : MonoBehaviour {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _itemCountText;

        [SerializeField] private TMP_Text _itemNameText;
        [SerializeField] private TMP_Text _itemInfoText;

        [SerializeField] private Button _itemUseButton;

        private BackendData.Chart.Item.Item _useItemInfo;
        private InGameUI_ItemInventory _inventory;
        private int _itemID = 0;

        // 자신을 관리해주는 아이템 인벤토리 UI를 가져와서 해당 UI에 있는 기능(아이템리스트에서 제거) 사용
        public void Init(InGameUI_ItemInventory inventory, int itemID, int count) {
            _inventory = inventory;

            _itemID = itemID;
            _itemCountText.text = count.ToString();

            _useItemInfo = StaticManager.Backend.Chart.Item.Dictionary[itemID];

            _image.sprite = _useItemInfo.ImageSprite;
            _itemNameText.text = _useItemInfo.ItemName;
            _itemInfoText.text = _useItemInfo.ItemContent;

            _itemUseButton.onClick.AddListener(UseItemButton);
        }

        // 
        public void Update() {
            // Dictionary[_itemID] = _item의 갯수
            int count = StaticManager.Backend.GameData.ItemInventory.Dictionary[_itemID];

            // count가 0이 되면 리스트에서 삭제
            if (count <= 0) {
                _inventory.DeleteUseItem(_itemID);
            }
            else {
                // 새로운 count로 변경
                _itemCountText.text = count.ToString();
            }
        }

        void UseItemButton() {
            InGameScene.Managers.Item.Use(_itemID);
        }
    }
}