// Copyright 2013-2022 AFI, INC. All rights reserved.

using BackEnd;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

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

    void Start() {
        SetTouchStartButton(); // 버튼 비활성화
        
        var bro = Backend.Initialize(true);
        
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
                .Builder()
            .RequestServerAuthCode(false)
            .RequestEmail()
            .RequestIdToken()
            .Build();
        
        // 커스텀 된 정보로 GPGS 초기화
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = false;

        PlayGamesPlatform.Activate();
        GoogleAuth();
    }
    
    private void GoogleAuth()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated == false)
        {
            Social.localUser.Authenticate(success =>
            {
                if (success == false)
                {
                    Debug.Log("구글 로그인 실패");
                    return;
                }
                
                // 로그인 성공
                Debug.Log("GetIDToken - " + PlayGamesPlatform.Instance.GetIdToken());
                Debug.Log("Email - " + ((PlayGamesLocalUser)Social.localUser).Email);
                Debug.Log("GoogleId - " + Social.localUser.id);
                Debug.Log("UserName - " + Social.localUser.userName);
                Debug.Log("UserName - " + PlayGamesPlatform.Instance.GetUserDisplayName());
            });
        }
    }
    
    public void OnClickGPGSLogin()
    {
        BackendReturnObject BRO =
            Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google, "GPGS로 만든 계정");

        if (BRO.IsSuccess())
        {
            Debug.Log("구글 토큰으로 뒤끝서버 로그인 성공 - 동기 방식-");
            if (string.IsNullOrEmpty(Backend.UserNickName)) {
                StaticManager.UI.OpenUI<LoginUI_Nickname>("Prefabs/LoginScene/UI", GetLoginUICanvas().transform);
                LoginWithBackendToken();
            }
        }
        else
        {
            switch (BRO.GetStatusCode())
            {
                case "200":
                    Debug.Log("이미 회원가입된 회원");
                    break;
                case "403":
                    Debug.Log("차단된 사용자 입니다. 차단 사유 : " + BRO.GetErrorCode());
                    break;
                
                default:
                    Debug.Log("서버 공통 에러 발생" + BRO.GetMessage());
                    break;
            }
        }
    }
    
    public void GPGSLogin()
    {
        // 이미 로그인 된 경우
        if (Social.localUser.authenticated == true)
        {
            BackendReturnObject BRO = Backend.BMember.AuthorizeFederation( GetTokens(), FederationType.Google, "gpgs" );
        }
        else
        {
            Social.localUser.Authenticate((bool success) => {
                if (success){
                    // 로그인 성공 -> 뒤끝 서버에 획득한 구글 토큰으로 가입 요청
                    BackendReturnObject BRO = Backend.BMember.AuthorizeFederation( GetTokens(), FederationType.Google, "gpgs" );
                }
                else
                {
                    // 로그인 실패
                    Debug.Log("Login failed for some reason");
                }
            });
        }
    }
    
    private string GetTokens()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // 유저 토큰 첫번째 방법
            string _IDtoken = PlayGamesPlatform.Instance.GetIdToken();
            // 두번째 방법
            // string _IDtoken = ((PlayGamesLocalUser).Social.localUser).GetIdToken();
            return _IDtoken;
        }
        else
        {
            Debug.Log("접속되어 있지 않습니다. 잠시 후 다시 시도 하세요.");
            GoogleAuth();
            return null;
        }
    }
    // 터치하여 시작 버튼 활성화
    // 터치 시, 해당 UI는 사라지며 자동 로그인 함수를 호출한다.
    private void SetTouchStartButton() {
        _touchStartButton.GetComponent<Button>().onClick.AddListener(() => {
            Destroy(_touchStartButton);
            _touchStartButton = null;
            LoginWithBackendToken();
        });
    }
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
    public void GoNextScene() {
        StaticManager.Instance.ChangeScene("LoadingScene");
    }
}