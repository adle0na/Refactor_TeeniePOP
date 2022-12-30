// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene.UI {
    //===========================================================
    // 아래 버튼 및 각 버튼별 UI를 교체하는 아랫쪽 UI를 제어하는 클래스
    //===========================================================
    public class InGameUI_BottomUI : MonoBehaviour {
        private InGameUI_BottomUIBase[] _bottomUIs;
        private Button[] _bottomUIButtons;

        [SerializeField] private GameObject _UIChangeButtonParentObject;

        //===========================================================
        // 씬에서 다른 바텀 UI들은 전부 활성화가 되어있어야한다.
        //===========================================================
        
        
        public void Init() {
            _bottomUIs = transform.GetComponentsInChildren<InGameUI_BottomUIBase>();

            // BottomUI 정보 불러와 초기화
            foreach (var ui in _bottomUIs) {
                ui.Init();
            }

            //바텀UI의 버튼 배열 
            _bottomUIButtons = _UIChangeButtonParentObject.GetComponentsInChildren<Button>();
            
            // 각 버튼별 클릭시 활성화되는 UI 배정
            for (int i = 0; i < _bottomUIButtons.Length; i++) {
                int index = i;
                _bottomUIButtons[index].onClick.AddListener(() => ChangeUI(index));
            }

            // 2번 UI 현재 장비로 초기 설정
            ChangeUI(2);
        }

        // 바텀 내 각 BottomUIBase를 가지고 있는 UI 클래스에 접근
        public T GetUI<T>() where T : InGameUI_BottomUIBase {
            for (int i = 0; i < _bottomUIs.Length; i++) {
                if (typeof(T) == _bottomUIs[i].GetType()) {
                    return (T)_bottomUIs[i];
                }
            }

            throw new Exception($"{typeof(T)}가 존재하지 않습니다.");
        }

        // 버튼을 누를경우 해당 UI로 변경
        void ChangeUI(int index) {
            try {
                for (int i = 0; i < _bottomUIButtons.Length; i++) {
                    _bottomUIButtons[i].image.color = Color.white;
                }

                _bottomUIButtons[index].image.color = Color.gray;

                Type type = _bottomUIs[index].GetType();
                
                // 배열을 순회하면서 해당 UI에 맞는 클래스에 존재할 경우 활성화, 나머지는 비활성화
                for (int i = 0; i < _bottomUIs.Length; i++) {
                    
                    if (_bottomUIs[i].GetType() == type) {
                        _bottomUIs[i].gameObject.SetActive(true);

                        _bottomUIs[i].Open();
                    }
                    else {
                        _bottomUIs[i].gameObject.SetActive(false);
                    }
                }
            }
            catch (Exception e) {
                throw new Exception(
                    $"활성되지 않은 Bottom UI가 존재합니다.\n시도된 UI : {index}번\n전체 Bottom UI 개수 : {_bottomUIs.Length}\n\n{e}");
            }
        }
    }
}