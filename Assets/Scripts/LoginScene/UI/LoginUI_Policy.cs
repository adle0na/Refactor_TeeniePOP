using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI_Policy : BaseUI {
    [SerializeField] private GameObject _policyInfoButtonGroup; // 서비스 이용 약관 or 개인정보 취급방침 안내 선택하는 버튼 묶음 오브젝트
    private Button[] _policyInfoButtonArray;
    private int _selectPolicyInfoIndex = 0;

    [SerializeField] private TMP_Text _policyInfoText; // 서비스 이용약관 or 개인정보 취급방침에 대한 내용
    private string _policyTermsInfoText = string.Empty;
    private string _policyPrivacyInfoText = string.Empty;

    [SerializeField] private GameObject _agreeCheckBoxGroup; // 동의 관련 체크박스 묶음 오브젝트

    [SerializeField] private Button _acceptButton; // 수락 버튼 

    private bool[] _checkBoxAgree;

    protected override void InitUI(ShowUIFunc showUIFunc) {
        SetPolicyInfoButtonGroup();
        SetAgreeCheckBoxGroup();
        SetAcceptButton();
        ClickPolicyInfoButton(0);
        SetPolicyInfoText(showUIFunc);
    }

    // 서비스 이용약관 or 개인정보 취급방침 버튼 할당
    private void SetPolicyInfoButtonGroup() {
        _policyInfoButtonArray = _policyInfoButtonGroup.GetComponentsInChildren<Button>();

        for (int i = 0; i < _policyInfoButtonArray.Length; i++) {
            int index = i;
            _policyInfoButtonArray[index].onClick.AddListener(() => ClickPolicyInfoButton(index));
        }
    }

    //이전에 선택된 버튼은 회색 처리, 누른 버튼은 활성화처리하는 함수
    private void ClickPolicyInfoButton(int index) {
        //전에 선택된 색깔 변경
        _policyInfoButtonArray[_selectPolicyInfoIndex].image.color =
            GetUIColor(UIColor.NonSelectedButtonColor);

        //선택한 버튼 번호 변경
        _selectPolicyInfoIndex = index;

        //변경된 번호의 버튼 색 변경
        _policyInfoButtonArray[_selectPolicyInfoIndex].image.color = GetUIColor(UIColor.SelectButtonColor);

        //버튼의 번호가 0이면 서비스 이용약관, 1이면 개인정보 취급방침에 대한 문장을 표시한다.
        switch (_selectPolicyInfoIndex) {
            case 0:
                _policyInfoText.text = _policyTermsInfoText;
                break;
            case 1:
                _policyInfoText.text = _policyPrivacyInfoText;
                break;
        }
    }

    // GetPolicy 함수 호출 함수
    private void SetPolicyInfoText(ShowUIFunc showUIFunc) {
        SendQueue.Enqueue(Backend.Policy.GetPolicy, callback => {
            try {
                if (IsBackendError(callback)) {
                    _policyTermsInfoText = "오류가 발생했습니다.";
                    StaticManager.Backend.SendBugReport(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(),
                        callback.ToString());
                    return;
                }

                // 서비스 이용약관에 대한 정보 파싱
                if (callback.GetReturnValuetoJSON()["terms"] == null) {
                    _policyTermsInfoText = "입력된 개인처리보호방침이 존재하지 않습니다.";
                }
                else {
                    _policyTermsInfoText = callback.GetReturnValuetoJSON()["terms"].ToString() ;
                    _policyTermsInfoText = _policyTermsInfoText.Replace("&nbsp;", "\n");
                }
                _policyInfoText.text = _policyTermsInfoText;
                
                // 개인정보처리방침에 대한 정보 파싱
                if (callback.GetReturnValuetoJSON()["privacy"] == null) {
                    _policyPrivacyInfoText = "입력된 서비스이용약관이 존재하지 않습니다.";
                }
                else {
                    _policyPrivacyInfoText = callback.GetReturnValuetoJSON()["privacy"].ToString();
                    _policyPrivacyInfoText = _policyPrivacyInfoText.Replace("&nbsp;", "\n");
                }

                showUIFunc.Invoke();
            }
            catch (Exception e) {
                ShowAlertUI(e.ToString());
            }
        });
    }

    // 처리방침 체크박스 세팅 함수
    private void SetAgreeCheckBoxGroup() {
        Toggle[] checkBoxArray = _agreeCheckBoxGroup.GetComponentsInChildren<Toggle>();

        _checkBoxAgree = new bool[checkBoxArray.Length];

        // 전부 false로 변경
        for (int i = 0; i < checkBoxArray.Length; i++) {
            int index = i;
            checkBoxArray[index].isOn = false;
            checkBoxArray[index].onValueChanged
                .AddListener(isChecked => { CheckAgreeButtonIsActive(index, isChecked); });
        }
    }

    private void SetAcceptButton() {
        _acceptButton.interactable = false;
        _acceptButton.image.color = GetUIColor(UIColor.NonSelectedButtonColor);
        _acceptButton.onClick.AddListener(AcceptPolicy);
    }

    private enum AgreeCheckBoxType {
        SERVICES_AGREE = 0,
        PERSONAL_AGREE = 1,
        PUSH_AGREE = 2
    }

    // 동의 버튼을 체크할때마다 조건이 만족되었는지 체크.
    // 만족되었으면 버튼 색을 변경.
    private void CheckAgreeButtonIsActive(int index, bool isChecked) {
        _checkBoxAgree[index] = isChecked;

        if (_checkBoxAgree[(int)AgreeCheckBoxType.SERVICES_AGREE] == true &&
            _checkBoxAgree[(int)AgreeCheckBoxType.PERSONAL_AGREE] == true) {
            _acceptButton.interactable = true;
            _acceptButton.image.color = GetUIColor(UIColor.BaseColor);
        }
        else {
            _acceptButton.interactable = false;
            _acceptButton.image.color = GetUIColor(UIColor.NonActiveColor);
        }
    }

    // 동의 버튼 클릭시 호출되는 함수.
    // 모바일 환경에서는 푸시 여부까지 확인하고 진행
    // 닉네임 화면으로 이동한다.
    private void AcceptPolicy() {
        if (_checkBoxAgree[(int)AgreeCheckBoxType.PUSH_AGREE] == true) {
#if UNITY_ANDROID    
//[뒤끝] 푸시 설정 함수
            SendQueue.Enqueue(Backend.Android.PutDeviceToken, callback => {
                if (IsBackendError(callback)) {
                    // StaticManager.UI.OpenWarning("푸시 알람 미처리 안내",
                    //     "푸시 알람이 정상적으로 처리되지 않았습니다.\n이후 설정에서 푸시 알람을 설정해주시기 바랍니다.", () => { AfterAccept(); });
                }
                else {
                    AfterAccept();
                }
            });
 #elif UNITY_IOS
//[뒤끝] 푸시 설정 함수

            SendQueue.Enqueue(Backend.iOS.PutDeviceToken, callback => {
                if (IsBackendError(callback)) {
                    StaticManager.UI.OpenWarning("푸시 알람 미처리 안내",
                        "푸시 알람이 정상적으로 처리되지 않았습니다.\n이후 설정에서 푸시 알람을 설정해주시기 바랍니다.", () => { AfterAccept(); });
                }
                else {
                    AfterAccept();
                }
            });
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE
            AfterAccept();

#endif
        }
        else {
            AfterAccept();
        }
    }

    private void AfterAccept() {
        CloseUI();
        StaticManager.UI.OpenUI<LoginUI_Nickname>("Prefabs/LoginScene/UI",
            LoginSceneManager.Instance.GetLoginUICanvas().transform);
    }
}