// Copyright 2013-2022 AFI, INC. All rights reserved.

using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene.UI {
    //===========================================================
    // 유저의 레벨, 경험치, 이름을 알려주는 UI
    //===========================================================
    public class InGameUI_User : MonoBehaviour {
        [SerializeField] private TMP_Text _NickNameText;
        [SerializeField] private Slider _ExpSlider;

        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _moneyText;

        public void Init() {
            UpdateUI();
        }

        // 현재 UserData로 UI를 업데이트
        public void UpdateUI() {
            _NickNameText.text = Backend.UserNickName;
            
            _levelText.text = StaticManager.Backend.GameData.UserData.HighLevel.ToString();
            _moneyText.text = StaticManager.Backend.GameData.UserData.Gold.ToString();
        }
    }

}