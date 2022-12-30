// Copyright 2013-2022 AFI, INC. All rights reserved.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene.UI {
    //===========================================================
    // 장비 UI에 사용되는 아이템 클래스
    //===========================================================
    public class InGameUI_EquipItem : MonoBehaviour {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _weaponName;
        [SerializeField] private TMP_Text _weaponStat;
        [SerializeField] private Button _weaponUpgradeButton;
        [SerializeField] private Button _weaponEquipButton;

        [SerializeField] private TMP_Text _weaponLevelText;
        [SerializeField] private TMP_Text _weaponUpgradePriceText;

        private BackendData.GameData.WeaponInventory.Item _weaponInfo;

        // 내 게임정보의 장비 목록에서 데이터를 불러와 적용
        public void Init(InGameUI_Equip equipUI, Sprite sprite, BackendData.GameData.WeaponInventory.Item weaponInfo) {
            _weaponInfo = weaponInfo;

            _image.sprite = sprite;
            _weaponName.text = weaponInfo.GetWeaponChartData().WeaponName;

            string stat = string.Empty;
            stat += $"atk {weaponInfo.GetCurrentWeaponStat().Atk}\n";
            stat += $"spd {weaponInfo.GetCurrentWeaponStat().Spd}\n";
            stat += $"delay {weaponInfo.GetCurrentWeaponStat().Delay}\n";
            _weaponStat.text = stat;

            _weaponUpgradePriceText.text = weaponInfo.GetCurrentWeaponStat().UpgradePrice.ToString();
            _weaponLevelText.text = "Lv." + weaponInfo.WeaponLevel.ToString();

            // 업그레이드 버튼 연결
            _weaponUpgradeButton.onClick.AddListener(OnClickUpgradeButton);
            // 장착 버튼을 클릭할 경우, InGameUI_Equip에서 가지고 있는 장비 변경 팝업창을 띄우도록 연결
            _weaponEquipButton.onClick.AddListener(() => equipUI.OpenEquipChangeUI(_weaponInfo));
        }

        // 업그레이드 클릭시 호출되는 함수
        void OnClickUpgradeButton() {
            // 장비 업그레이드 비용
            float upgradePrice = _weaponInfo.GetCurrentWeaponStat().UpgradePrice;

            // 업그레이드 비용이 더 높을 경우
            if (StaticManager.Backend.GameData.UserData.Money < upgradePrice) {
                StaticManager.UI.AlertUI.OpenWarningUI("업그레이드 불가", "보유중인 재화가 부족합니다.");
                return;
            }

            // 돈이 충분할 경우, 레벨업
            StaticManager.Backend.GameData.WeaponInventory.Dictionary[_weaponInfo.MyWeaponId].LevelUp();
            // 레벨 갱신
            _weaponLevelText.text = "Lv." + _weaponInfo.WeaponLevel.ToString();

            // 스텟 갱신
            string stat = string.Empty;
            stat += $"atk {_weaponInfo.GetCurrentWeaponStat().Atk}\n";
            stat += $"spd {_weaponInfo.GetCurrentWeaponStat().Spd}\n";
            stat += $"delay {_weaponInfo.GetCurrentWeaponStat().Delay}\n";
            _weaponStat.text = stat;

            // money 데이터 감소
            InGameScene.Managers.Game.UpdateUserData(-upgradePrice, 0);
        }

        // 창착 여부를 설정하는 함수
        public void SetButtonEquiped(bool isEquip) {
            if (isEquip) {
                _weaponEquipButton.GetComponentInChildren<TMP_Text>().text = "장착됨";
                _weaponEquipButton.GetComponent<Image>().color = Color.gray;
            }
            else {
                _weaponEquipButton.GetComponentInChildren<TMP_Text>().text = "장착";
                _weaponEquipButton.GetComponent<Image>().color = Color.white;
            }
        }
    }
}