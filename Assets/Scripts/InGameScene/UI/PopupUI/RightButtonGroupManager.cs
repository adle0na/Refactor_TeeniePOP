using System;
using System.Reflection;
using InGameScene.UI.PopupUI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene {
    //===========================================================
    // 인게임 상단 UI중 설정, 랭킹등의 아이콘을 담당하는 UI 클래스
    //===========================================================
    public class RightButtonGroupManager : MonoBehaviour {
        [SerializeField] private GameObject _popupParentObject;

        [SerializeField] private Button _settingButton;
        [SerializeField] private Button _rankUIButton;
        [SerializeField] private Button _postUIButton;

        private const string _path = "Prefabs/InGameScene/UI/";

        void Start() {
            _settingButton.onClick.AddListener(OpenSettingUI);
            _rankUIButton.onClick.AddListener(OpenInGameUI_Rank);
            _postUIButton.onClick.AddListener(OpenInGameUI_Post);
        }
        private void OpenSettingUI() {
            OpenUI<InGameUI_Etc>("InGameUI_Etc");
        }
        
        private void OpenInGameUI_Rank() {
            if (StaticManager.Backend.Rank.List.Count <= 0) {
                StaticManager.UI.AlertUI.OpenWarningUI("랭킹 미존재 오류", "랭킹이 존재하지 않습니다.\n랭킹을 생성해주세요.");
                return;
            }
            OpenUI<InGameUI_Rank>("InGameUI_Rank");
        }
        
        private void OpenInGameUI_Post() {
            OpenUI<InGameUI_Post>("InGameUI_Post");
            _postUIButton.transform.GetChild(0).gameObject.SetActive(false);
        }

        // InGamePopupUI이 부모인 UI를 생성하거나 활성화시키는 함수.
        private void OpenUI<T>(string prefabName) where T : InGamePopupUI {
            try {
                T ui = transform.GetComponentInChildren<T>();
                
                if (ui == null) {
                    var obj = Resources.Load<GameObject>(_path + prefabName);
                    ui = Instantiate(obj, _popupParentObject.transform, true).GetComponent<T>();
                    ui.transform.localScale = new Vector3(1, 1, 1);
                    
                    // Init의 경우, 한번만 호출.
                    ui.Init();
                }
                ui.gameObject.SetActive(true);
                ui.Open();
            }
            catch (Exception e) {
                StaticManager.UI.AlertUI.OpenErrorUI(typeof(T).Name,MethodBase.GetCurrentMethod()?.ToString(), e);
            }
        }

        public void SetPostIconAlert(bool isActive) {
            _postUIButton.transform.GetChild(0).gameObject.SetActive(isActive);
        }
    }
}