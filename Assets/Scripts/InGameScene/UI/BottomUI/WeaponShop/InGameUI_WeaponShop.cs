// Copyright 2013-2022 AFI, INC. All rights reserved.

using UnityEngine;

namespace InGameScene.UI {
    //===========================================================
    // 무기 상점 UI
    //===========================================================
    public class InGameUI_WeaponShop : InGameUI_BottomUIBase {
        [SerializeField] private GameObject _weaponShopParentObject;
        [SerializeField] private GameObject _weaponShopItemPrefab;

        // 무기 상점 아이템 생성
        public override void Init() {
            foreach (var weapon in StaticManager.Backend.Chart.Weapon.Dictionary) {
                Sprite sprite = weapon.Value.WeaponSprite;

                var newWeapon = Instantiate(_weaponShopItemPrefab, _weaponShopParentObject.transform, true);
                newWeapon.transform.localPosition = new Vector3(0, 0, 0);
                newWeapon.transform.localScale = new Vector3(1, 1, 1);

                newWeapon.GetComponent<InGameUI_WeaponShopItem>().Init(sprite, weapon.Value);
            }
        }
    }
}