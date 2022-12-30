using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LoginUI_CustomLogin : BaseUI {
    [SerializeField] private TMP_InputField _idInputField;
    [SerializeField] private TMP_InputField _pwInputField;

    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _signUpButton;
    
    protected override void InitUI(ShowUIFunc showUIFunc) {
        _loginButton.onClick.AddListener(OnClickLoginButton);
        _signUpButton.onClick.AddListener(OnClickSignUpButton);
        
        showUIFunc.Invoke();

    }
    
    void OnClickLoginButton() {
        string id = _idInputField.text;
        string pw = _pwInputField.text;

        if (string.IsNullOrEmpty(id)) {
            ShowAlertUI("아이디가 비어있습니다.");
            return;
        }

        if (string.IsNullOrEmpty(pw)) {
            ShowAlertUI("패스워드가 비어있습니다.");
            return;
        }
        
        // [뒤끝] 커스텀로그인 함수
        SendQueue.Enqueue(Backend.BMember.CustomLogin,id, pw, callback => {
            try {
                if (IsBackendError(callback)) {
                    if (callback.GetStatusCode() == "401") {
                        if (callback.GetMessage().Contains("bad customId")) {
                            ShowAlertUI($"존재하지 않는 아이디입니다.");

                        }
                        else if (callback.GetMessage().Contains("bad customPassword")) {
                            ShowAlertUI($"비밀번호가 올바르지 않습니다.");

                        }
                        else if (callback.GetMessage().Contains("maintenance")) {
                            ShowAlertUI($"서버 점검중입니다.");

                        }
                        else {
                            
                        }
                    }
                    else if (callback.GetStatusCode() == "403") {
                        if (callback.GetMessage().Contains("forbidden block user")) {
                            ShowAlertUI($"해당 계정이 차단당했습니다\n차단사유 : {callback.GetErrorCode().ToString()}");
                        }
                    }
                    else {
                        ShowAlertUI($"로그인에 실패하였습니다\n{callback.ToString()}");
                        return;
                    }
                }
                else {
                    // 닉네임이 없을 경우, 닉네임 생성 UI 생성
                    if (string.IsNullOrEmpty(Backend.UserNickName)) {
                        StaticManager.UI.OpenUI<LoginUI_Nickname>("Prefabs/LoginScene/UI", LoginSceneManager.Instance.GetLoginUICanvas().transform);
                    }
                    else {
                        LoginSceneManager.Instance.GoNextScene();
                    }
                }
                
            }
            catch (Exception e) {
                ShowAlertUI(e.ToString());
            }
        });
    }

    void OnClickSignUpButton() {
        StaticManager.UI.OpenUI<LoginUI_CustomSignUp>("Prefabs/LoginScene/UI", LoginSceneManager.Instance.GetLoginUICanvas().transform);
        CloseUI();
    }

}