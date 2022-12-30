// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

namespace InGameScene.UI {
    //===============================================================
    // 하단의 장비 버튼 클릭시 보여지는 UI
    //===============================================================
    public class InGameUI_Equip : InGameUI_BottomUIBase {
        [SerializeField] private GameObject _weaponEquipParentObject;
        [SerializeField] private GameObject _weaponEquipItemPrefab;

        [SerializeField] private InGameUI_EquipChangePopup _weaponEquipChangeUI;

        private Dictionary<string, InGameUI_EquipItem> _bottomWeaponEquipDic = new();

        public override void Init() {
            _weaponEquipChangeUI.Init();
            _weaponEquipChangeUI.gameObject.SetActive(false);
            
            // 현재 가지고 있는 무기를 가져와 아이템으로 만든다.
            foreach (var weapon in StaticManager.Backend.GameData.WeaponInventory.Dictionary) {
                AddWeaponObjectInInventoryUI(weapon.Value.MyWeaponId);
                UpdateUI(null, weapon.Value.MyWeaponId);
            }
        }

        public override void Open() {
        }

        // 무기를 추가하는 함수
        public void AddWeaponObjectInInventoryUI(string myWeaponId) {
            if (_bottomWeaponEquipDic.ContainsKey(myWeaponId)) {
                Debug.LogWarning($"이미 존재하는 {myWeaponId} 입니다.");
                return;
            }

            // myWeaponId의 정보로 검색하여 데이터를 받아온다.
            BackendData.GameData.WeaponInventory.Item weaponInventoryItem =
                StaticManager.Backend.GameData.WeaponInventory.Dictionary[myWeaponId];

            // 오브젝트  생성
            var obj = Instantiate(_weaponEquipItemPrefab, _weaponEquipParentObject.transform, true);
            obj.transform.localPosition = new Vector3(0, 0, 0);
            obj.transform.localScale = new Vector3(1, 1, 1);

            // 아이템 내부 데이터 초기화
            var equipItem = obj.GetComponent<InGameUI_EquipItem>();
            equipItem.Init(this, weaponInventoryItem.GetWeaponChartData().WeaponSprite, weaponInventoryItem);
            _bottomWeaponEquipDic.Add(myWeaponId, equipItem.GetComponent<InGameUI_EquipItem>());
        }

        // 무기 장착 안내 UI 생성
        public void OpenEquipChangeUI(BackendData.GameData.WeaponInventory.Item gameData) {
            _weaponEquipChangeUI.Open(gameData);
        }

        // 장착중이었던 무기의 버튼은 다시 '장착'버튼으로 바꾸고, 변경된 무기는 '장착중'으로 변경하는 함수
        // SetButtonEquiped가 true면 장착됨, false 그냥 장착 표시
        public void UpdateUI(string prevEquipWeaponId, string changeWeaponId) {
            // prevEquipWeaponId에 값이 존재한다면 해당 무기 아이템의 장착 표시 해제
            if (string.IsNullOrEmpty(prevEquipWeaponId) == false) {
                _bottomWeaponEquipDic[prevEquipWeaponId].SetButtonEquiped(false);
            }

            _bottomWeaponEquipDic[changeWeaponId].SetButtonEquiped(true);
        }
    }
}