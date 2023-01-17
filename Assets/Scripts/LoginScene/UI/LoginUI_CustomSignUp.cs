using System.Collections;
using System.Collections.Generic;
using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LoginUI_CustomSignUp : BaseUI {
    
    [SerializeField] private TMP_InputField _idInputField;
    [SerializeField] private TMP_InputField _pwInputField;
    [SerializeField] private TMP_InputField _pwConfirmInputField;

    [SerializeField] private Button _signUpButton;
    
    protected override void InitUI(ShowUIFunc showUIFunc) {
        _signUpButton.onClick.AddListener(OnClickSignUpButton);
        _errorTitle = "회원가입 실패";
        
        
        showUIFunc.Invoke();

    }
    
    void OnClickSignUpButton() {
        string id = _idInputField.text;
        string pw = _pwInputField.text;

        if (string.IsNullOrEmpty(id)) {
            ShowAlertUI("아이디가 비어있습니다.");
            return;

        }

        if (string.IsNullOrEmpty(pw)) {
            ShowAlertUI("비밀번호가 비어있습니다.");
            return;

        }

        if (!_pwInputField.text.Equals(_pwConfirmInputField.text)) {
            ShowAlertUI("비밀번호가 일치하지 않습니다.");
            return;
        }
        
        // [뒤끝] 회원가입 함수
        SendQueue.Enqueue(Backend.BMember.CustomSignUp,id, pw, callback => {
            if (IsBackendError(callback)) {

                if (callback.GetStatusCode() == "409") {
                    ShowAlertUI("중복된 아이디입니다.");
                }
                else {
                    ShowAlertUI("예기치 못한 에러가 발생했습니다.\n"+callback.ToString());
                }
            }
            else {
                CloseUI();
                StaticManager.UI.OpenUI<LoginUI_Policy>("Prefabs/LoginScene/UI", LoginSceneManager.Instance.GetLoginUICanvas().transform);
            }
        });
    }
}