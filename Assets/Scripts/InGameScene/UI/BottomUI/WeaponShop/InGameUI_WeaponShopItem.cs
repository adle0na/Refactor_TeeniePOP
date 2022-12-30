// Copyright 2013-2022 AFI, INC. All rights reserved.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene.UI {
    //===========================================================
    // 무기 상점 UI의 아이템 클래스
    //===========================================================
    public class InGameUI_WeaponShopItem : MonoBehaviour {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _weaponName;
        [SerializeField] private TMP_Text _weaponNormalStat;
        [SerializeField] private Button _weaponBuyButton;
        [SerializeField] private TMP_Text _weaponPrice;

        private BackendData.Chart.Weapon.Item _weaponInfo;

        // 무기 차트의 정보에 따라 초기화
        public void Init(Sprite sprite, BackendData.Chart.Weapon.Item weaponInfo) {
            _weaponInfo = weaponInfo;

            _image.sprite = sprite;
            _weaponName.text = weaponInfo.WeaponName;

            string stat = string.Empty;
            stat += $"atk {weaponInfo.Atk}(+ {weaponInfo.GrowingAtk})\n";
            stat += $"spd {weaponInfo.Spd}(+ {weaponInfo.GrowingSpd})\n";
            stat += $"delay {weaponInfo.Delay}(+ {weaponInfo.GrowingDelay})\n";
            _weaponNormalStat.text = stat;

            _weaponPrice.text = weaponInfo.Price.ToString();

            _weaponBuyButton.onClick.AddListener(BuyButton);
        }

        // 구매 버튼 클릭시 현재 자금 무기를 비교하여 구매 여부 판단
        // 구매 가능 시, UserData를 뺄셈하고 무기 지급.
        void BuyButton() {
            if (StaticManager.Backend.GameData.UserData.Money < _weaponInfo.Price) {
                StaticManager.UI.AlertUI.OpenWarningUI("구매 불가", "현재 자금이 부족하여 해당 아이템을 구매할 수 없습니다");
                return;
            }

            InGameScene.Managers.Game.UpdateUserData(-_weaponInfo.Price, 0);
            StaticManager.UI.AlertUI.OpenAlertUI("구매 완료", _weaponInfo.WeaponName + "이(가) 구매 완료되었습니다.");
            InGameScene.Managers.Game.UpdateWeaponInventory(_weaponInfo.WeaponID);
        }
    }
}