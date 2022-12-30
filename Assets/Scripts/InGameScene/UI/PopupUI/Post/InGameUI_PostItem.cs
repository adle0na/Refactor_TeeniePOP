// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace InGameScene.UI.PopupUI {
    //===========================================================
    // 우편 팝업 UI에 각 우편 아이템을 보여줄 아이템 클래스
    //===========================================================
    public class InGameUI_PostItem : MonoBehaviour {
        [SerializeField] private TMP_Text postTitleText;
        [SerializeField] private TMP_Text postContentText;

        [SerializeField] private TMP_Text postRewardText;
        [SerializeField] private Button postReceiveButton;
        [SerializeField] private TMP_Text expirationDateText;

        private BackendData.Post.PostItem _postItem;

        public delegate void ReceivePostFunc(string inDate);

        private ReceivePostFunc _receivePostFunc;

        // List에 있는 PostItem의 데이터를 이용하여 우편 아이템 생성, 우편 수령 시 성공할 경우 UI에서 우편 제거하는 버튼 연결
        public void Init(BackendData.Post.PostItem postItem, ReceivePostFunc func) {
            try {
                _postItem = postItem;

                postTitleText.text = _postItem.title;
                postContentText.text = _postItem.content;
                
                if ((_postItem.expirationDate - DateTime.UtcNow).Days > 0) {
                    expirationDateText.text = (_postItem.expirationDate - DateTime.UtcNow).Days + "일 남음";
                }
                else {
                    expirationDateText.text = (_postItem.expirationDate - DateTime.UtcNow).Hours + "시간 남음";

                }
                
                string itemString = string.Empty;
                foreach (var item in _postItem.items) {
                    itemString += $"{item.itemName}\n";
                }

                if (itemString.Length > 0) {
                    itemString.TrimEnd('|');
                }

                postRewardText.text = itemString;
                _receivePostFunc = func;

                postReceiveButton.onClick.AddListener(Receive);
            }
            catch (Exception e) {
                throw new Exception($"{GetType().Name} : {MethodBase.GetCurrentMethod()?.ToString()} : {e.ToString()}");
            }
        }

        // 아이템을 받는 함수
        void Receive() {
            try {
                // PostItem 객체에서 우편 받기 함수 수령후 결과값 전송
                _postItem.ReceiveItem((isSuccess) => {
                    if (isSuccess) {
                        // 성공 시 InGameUI_Post에서 해당 우편 제거
                        _receivePostFunc.Invoke(_postItem.inDate);
                    }
                });
            }
            catch (Exception e) {
                StaticManager.UI.AlertUI.OpenErrorUI(GetType().Name,MethodBase.GetCurrentMethod()?.ToString(), e.ToString());
            }
        }
    }
}