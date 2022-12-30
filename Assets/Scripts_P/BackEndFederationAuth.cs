 using BackEnd;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.SceneManagement;

 public class BackEndFederationAuth : MonoBehaviour
{
    void Start()
    {
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
    
    // 구글 토큰 받아오기
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

    public void OnClickGPGSLogin()
    {
        BackendReturnObject BRO =
            Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google, "GPGS로 만든 계정");

        if (BRO.IsSuccess())
        {
            Debug.Log("구글 토큰으로 뒤끝서버 로그인 성공 - 동기 방식-");
            SceneManager.LoadScene(1);
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

    public void OnClickUpdateEmail()
    {
        BackendReturnObject BRO = Backend.BMember.UpdateFederationEmail(GetTokens(), FederationType.Google);
        
        if (BRO.IsSuccess())
            Debug.Log("이메일 주소 저장 완료");
        
        else
        if (BRO.GetStatusCode() == "404") Debug.Log("federationId not found, federationId을(를) 찾을 수 없습니다");
        
    }

    public void OnClickCheckUserAuthenticate()
    {
        BackendReturnObject BRO = Backend.BMember.CheckUserInBackend(GetTokens(), FederationType.Google);
        if (BRO.GetStatusCode() == "200")
        {
            Debug.Log("가입 중인 계정입니다.");
            
            // 해당 계정 정보
            Debug.Log(BRO.GetReturnValue());
        }
        else
            Debug.Log("가입된 계정이 아닙니다");
    }

    public void OnClickChangeCustomToFederation()
    {
        BackendReturnObject BRO = Backend.BMember.ChangeCustomToFederation(GetTokens(), FederationType.Google);

        if (BRO.IsSuccess())
        {
            Debug.Log("페더레이션 계정으로 변경 완료");
        }
        else
        {
            switch (BRO.GetStatusCode())
            {
                case "400":
                    if (BRO.GetErrorCode() == "BadPameterException")
                    {
                        Debug.Log("이미 ChangeCustomToFederation 완료 되었는데 다시 시도한 경우");
                    }
                    else if (BRO.GetErrorCode() == "UndefinedParameterException")
                    {
                        Debug.Log("customLogin 하지 않은 상황에서 시도한 경우");
                    }
                    break;
                case "409":
                    Debug.Log("Duplicated federationId, 중복된 federation 입니다");
                    break;
                default:
                    break;
            }
        }
    }
}
