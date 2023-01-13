// Copyright 2013-2022 AFI, INC. All rights reserved.

using BackEnd;
using UnityEngine;
using UnityEngine.UI;

public class LoginSceneManager : MonoBehaviour {

    private static LoginSceneManager _instance;

    public static LoginSceneManager Instance {
        get {
            return _instance;
        }
    }
    
    [SerializeField] private Canvas _loginUICanvas;
    [SerializeField] private GameObject _touchStartButton;

    public Canvas GetLoginUICanvas() {
        return _loginUICanvas;
    }
    
    void Awake() {
        if (_instance == null) {
            _instance = this;
        }

        // StaticManager가 없을 경우 새로 생성
        if (FindObjectOfType(typeof(StaticManager)) == null) {
            var obj = Resources.Load<GameObject>("Prefabs/StaticManager");
            Instantiate(obj);
        }
    }

    // void Start() {
    //     SetTouchStartButton(); // 버튼 비활성화
    // }
    
    // 터치하여 시작 버튼 활성화
    // 터치 시, 해당 UI는 사라지며 자동 로그인 함수를 호출한다.
    // private void SetTouchStartButton() {
    //     _touchStartButton.GetComponent<Button>().onClick.AddListener(() =>
    //     {
    //         Destroy(_touchStartButton);
    //         _touchStartButton = null;
    //         LoginWithBackendToken();
    //     });
    // }

    // 자동로그인 함수 호출. 이후 처리는 AuthorizeProcess 참고
    private void LoginWithBackendToken() {
        SendQueue.Enqueue(Backend.BMember.LoginWithTheBackendToken, callback => {
            Debug.Log($"Backend.BMember.LoginWithTheBackendToken : {callback}");

            if (callback.IsSuccess()) {
                // 닉네임이 없을 경우
                if (string.IsNullOrEmpty(Backend.UserNickName)) {
                    StaticManager.UI.OpenUI<LoginUI_Nickname>("Prefabs/LoginScene/UI", GetLoginUICanvas().transform);
                }
                else {
                    GoNextScene();
                }
            }
        });
    }

    // 게스트로그인 함수 호출. 이후 처리는 AuthorizeProcess 참고
    private void GuestLogin() {
        SendQueue.Enqueue(Backend.BMember.GuestLogin, AuthorizeProcess);
    }

    // 로그인 함수 후 처리 함수
    private void AuthorizeProcess(BackendReturnObject callback) {
        Debug.Log($"Backend.BMember.AuthorizeProcess : {callback}");

        // 에러가 발생할 경우 리턴
        // 로그인 버튼 활성화
        if (callback.IsSuccess() == false) {
            return;
        }
        
        // 새로 가입인 경우에는 statusCode가 201, 기존 로그인일 경우에는 200이 리턴된다.
        if (callback.GetStatusCode() == "201") {
            GetPolicy();
        }
        else {
            GoNextScene();
        }
    }

    // 개인정보보호정책 UI 생성
    private void GetPolicy() {
        StaticManager.UI.OpenUI<LoginUI_Policy>("Prefabs/LoginScene/UI", GetLoginUICanvas().transform);
    }

    public void GoNextScene() {
        StaticManager.Instance.ChangeScene("LoadingScene");

    }
}
