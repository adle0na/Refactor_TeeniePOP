// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using BackEnd;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


namespace InGameScene.UI.PopupUI {
    
    //===========================================================
    // 친구,길드, 이벤트등 세부 팝업 UI를 열 수 있는 UI에 관한 클래스
    //===========================================================
    public class InGameUI_Etc : InGamePopupUI {
        [SerializeField] private Button _friendButton;
        [SerializeField] private Button _guildButton;
        [SerializeField] private Button _eventButton;
        [SerializeField] private Button _couponButton;
        [SerializeField] private Button _gachaUIButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _logoutUIButton;

        public override void Init() {
            _friendButton.onClick.AddListener(FriendButton);
            _guildButton.onClick.AddListener(GuildButton);
            _eventButton.onClick.AddListener(EventButton);
            _couponButton.onClick.AddListener(CouponButton);
            _gachaUIButton.onClick.AddListener(GachaButton);
            _saveButton.onClick.AddListener(SaveButton);
            _logoutUIButton.onClick.AddListener(LogOutButton);
        }

        public override void Open() {
            // 서버 호출이 필요없으므로 별도 작업 없음
        }

        private void FriendButton() {
            StaticManager.UI.AlertUI.OpenWarningUI("미구현 안내", "친구 기능은 준비중입니다.");
        }

        private void GuildButton() {
            StaticManager.UI.AlertUI.OpenWarningUI("미구현 안내", "길드 기능은 준비중입니다.");

        }
        private void EventButton() {
            StaticManager.UI.AlertUI.OpenWarningUI("미구현 안내", "이벤트/공지사항은 준비중입니다.");

        }
        private void CouponButton() {
            StaticManager.UI.AlertUI.OpenWarningUI("미구현 안내", "쿠폰은 준비중입니다.");

        }
        private void GachaButton() {
            StaticManager.UI.AlertUI.OpenWarningUI("미구현 안내", "확률 뽑기은 준비중입니다.");

        }
        
        private void SaveButton() {
            StaticManager.UI.AlertUI.OpenWarningUI("저장 안내", "확인을 누르면 수동저장이 진행됩니다.", () => {
                StaticManager.UI.SetLoadingIcon(true);
                //[뒤끝] 일정주기마다 자동저장하는기능을 수동으로 호출
                StaticManager.Backend.UpdateAllGameData(callback => {
                    StaticManager.UI.SetLoadingIcon(false);

                    if (callback == null) {
                        StaticManager.UI.AlertUI.OpenAlertUI("저장 데이터 미존재", "저장할 데이터가 존재하지 않습니다.");
                        return;
                    }
                    
                    if (callback.IsSuccess()) {
                        StaticManager.UI.AlertUI.OpenAlertUI("저장 성공", "저장에 성공했습니다.");
                    }
                    else {
                        StaticManager.UI.AlertUI.OpenErrorUIWithText("수동 저장 실패",
                            $"수동 저장에 실패했습니다.\n{callback.ToString()}");
                    }

                });
            });
            
        }
        
        private void LogOutButton() {
            StaticManager.UI.AlertUI.OpenWarningUI("로그아웃 안내", "확인을 누르면 로그아웃이 진행됩니다.", () => {
                StaticManager.UI.SetLoadingIcon(true);
                //[뒤끝] 로그아웃 함수
                SendQueue.Enqueue(Backend.BMember.Logout, callback => {
                    Debug.Log($"Backend.BMember.Logout : {callback}");

                    if (callback.IsSuccess()) {
                        StaticManager.Instance.ChangeScene("LoginScene");
                    }
                    else {
                        StaticManager.UI.AlertUI.OpenErrorUI(GetType().Name,MethodBase.GetCurrentMethod()?.ToString(), callback.ToString());
                    }
                    StaticManager.UI.SetLoadingIcon(false);
                });
            });
        }


    }
}