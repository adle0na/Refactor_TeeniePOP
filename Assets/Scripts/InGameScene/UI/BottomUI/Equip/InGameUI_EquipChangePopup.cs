// Copyright 2013-2022 AFI, INC. All rights reserved.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene.UI {
    //===============================================================
    // 장비 UI의 아이템 클래스에서 장착 버튼 클릭시 어디로 장착할지 설정하는 UI
    //===============================================================
    public class InGameUI_EquipChangePopup : MonoBehaviour {
        [Header("무기들의 부모 객체")] 
        [SerializeField] private GameObject _positionButtonsParentObject;

        [Header("변경 전 무기 정보")] 
        [SerializeField] private Image _prevWeaponImage;
        [SerializeField] private TMP_Text _prevWeaponStat;


        [Header("변경 후 무기 정보")] 
        [SerializeField] private Image _changeWeaponImage;
        [SerializeField] private TMP_Text _changeWeaponStat;

        [Header("버튼")] 
        [SerializeField] private Button _equipButton;
        [SerializeField] private Button _closeButton;



        // 선택한 무기 자리
        private int selectPosition = -1;

        // 바꿀 무기 정보
        private BackendData.GameData.WeaponInventory.Item _changeWeapon;
        
        //무기 자리가 index인 배열 정보
        // 각 버튼과 동일한 index를 공유하는 무기 정보 데이터
        private Button[] _weaponPositionButtons;
        private BackendData.GameData.WeaponInventory.Item[] _equipWeapons;
        
        public void Init() {
            _weaponPositionButtons = _positionButtonsParentObject.GetComponentsInChildren<Button>();
            _equipWeapons = new BackendData.GameData.WeaponInventory.Item[_weaponPositionButtons.Length];

            _equipButton.onClick.AddListener(Equip);
            _closeButton.onClick.AddListener(Close);

            _prevWeaponImage.gameObject.SetActive(false);

        }

        // 장착 UI에 있는 설정들을 초기화하는 함수
        private void Reset() {
            selectPosition = -1;
            
            // 각 버튼과 동일한 index를 공유하는 무기 정보 데이터 초기화
            for (int i = 0; i < _equipWeapons.Length; i++) {
                _equipWeapons[i] = null;
            }

            _prevWeaponImage.sprite = null;
            _changeWeaponImage.sprite = null;

            _prevWeaponStat.text = string.Empty;
            _changeWeaponStat.text = string.Empty;

            // 각 버튼에 할당된 이미지와 무기정보 모두 제거
            for (int i = 0; i < _weaponPositionButtons.Length; i++) {
                int index = i;
                _weaponPositionButtons[index].transform.GetChild(0).GetComponent<Image>().sprite = null;
                _weaponPositionButtons[index].transform.GetChild(0).gameObject.SetActive(false);
                _weaponPositionButtons[index].onClick.RemoveAllListeners();
                // 각 버튼별로 자신의 position 배정
                _weaponPositionButtons[index].onClick.AddListener(() => SelectPosition(index));
            }
        }

        public void Open(BackendData.GameData.WeaponInventory.Item changeWeapon) {
            Reset();

            // 장착 버튼을 누른 무기 데이터
            _changeWeapon = changeWeapon;
            _changeWeaponImage.sprite = changeWeapon.GetWeaponChartData().WeaponSprite;
            
            string statText = $"atk : {changeWeapon.GetCurrentWeaponStat().Atk}\n";
            statText += $"spd : {changeWeapon.GetCurrentWeaponStat().Spd}\n";
            statText += $"delay : {changeWeapon.GetCurrentWeaponStat().Delay}";

            _changeWeaponStat.text = statText;

            // weaponEquip.Value는 해당 무기의 장착 위치
            foreach (var weaponEquip in StaticManager.Backend.GameData.WeaponEquip.Dictionary) {
                int position = weaponEquip.Value;

                // weaponEquip.key는 내 무기의 고유값
                BackendData.GameData.WeaponInventory.Item stat = StaticManager.Backend.GameData.WeaponInventory
                    .Dictionary[weaponEquip.Key];
                
                // 장착 위치에 무기 정보 삽입
                _equipWeapons[position] = stat;
                _weaponPositionButtons[position].transform.GetChild(0).gameObject.SetActive(true);
                _weaponPositionButtons[position].transform.GetChild(0).GetComponent<Image>().sprite =
                    _equipWeapons[position].GetWeaponChartData().WeaponSprite;
            }

            gameObject.SetActive(true);
        }

        // 각 무기 그림이 그려진 버튼을 클릭할 경우, 오른쪽의 현재 무기 사진과 정보를 출력하는 함수
        private void SelectPosition(int position) {
            
            if (selectPosition >= 0) {
                _weaponPositionButtons[selectPosition].image.color = Color.white;
            }

            selectPosition = position;

            _weaponPositionButtons[selectPosition].image.color = Color.gray;

            string statText = string.Empty;

            // 장착되어 있는 칸이라면
            if (_equipWeapons[position] != null) {
                _prevWeaponImage.gameObject.SetActive(true);
                _prevWeaponImage.sprite = _equipWeapons[position].GetWeaponChartData().WeaponSprite;
                BackendData.GameData.WeaponInventory.Item.CurrentStat stat = _equipWeapons[position]
                    .GetCurrentWeaponStat();

                statText = $"atk : {stat.Atk}\n";
                statText += $"spd : {stat.Spd}\n";
                statText += $"delay : {stat.Delay}";
            }
            else {
                // 장착되어 있지 않은 칸이라면
                _prevWeaponImage.gameObject.SetActive(false);

            }

            _prevWeaponStat.text = statText;
        }

        // 변경 버튼
        private void Equip() {
            // 선택된 무기가 없을 경우(희귀 케이스)
            if (selectPosition < 0) {
                StaticManager.UI.AlertUI.OpenWarningUI("무기 변경 안내", "무기를 선택해주세요.");
            }
            else {
                // 변경된 정보로 변경
                InGameScene.Managers.Game.UpdateWeaponEquip(selectPosition, _equipWeapons[selectPosition]?.MyWeaponId,
                    _changeWeapon.MyWeaponId);
            }

            Close();
        }

        private void Close() {
            gameObject.SetActive(false);
        }
    }
}