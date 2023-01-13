using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LoginUI_Nickname : BaseUI {
    [SerializeField] private TMP_InputField _nickNameInputField;
    [SerializeField] private Button _nicknameCreateButton;
    
    protected override void InitUI(ShowUIFunc showUIFunc) {
        _nicknameCreateButton.onClick.AddListener(OnClickCreateNicknameButton);
        showUIFunc.Invoke();
    }
    
    void OnClickCreateNicknameButton() {
        string nickname = _nickNameInputField.text;

        if (string.IsNullOrEmpty(nickname)) {
            ShowAlertUI("닉네임이 비어있습니다.");
            return;
        }

        //[뒤끝] 닉네임 업데이트 함수
        SendQueue.Enqueue(Backend.BMember.UpdateNickname,nickname, callback => {
            try {
                if (IsBackendError(callback)) {
                    if (callback.GetStatusCode() == "400") {
                        if (callback.GetMessage().Contains("undefined nickname")) {
                            ShowAlertUI($"닉네임이 비어있습니다.");

                        }
                        else if (callback.GetMessage().Contains("bad nickname is too long")) {
                            ShowAlertUI($"20자 이상은 입력할 수 없습니다.");

                        }
                        else if (callback.GetMessage().Contains("bad beginning or end")) {
                            ShowAlertUI($"닉네임이 앞 혹은 뒤에 공백이 존재합니다");
                        }
                        else  {
                            ShowAlertUI($"알 수 없는 에러입니다.");
                        }
                    }
                    else if (callback.GetStatusCode() == "409") {
                        ShowAlertUI($"중복된 닉네임입니다.");

                    }
                }
                else {
                    LoginSceneManager.Instance.GoNextScene();
                }
                
            }
            catch (Exception e) {
                ShowAlertUI(e.ToString());
            }
        });
    }
}