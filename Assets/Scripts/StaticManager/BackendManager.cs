// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Transactions;
using BackEnd;
using BackendData.Base;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class BackendManager : MonoBehaviour {
    // 1. 로그인씬에서 초기화 진행
    // 2. 로딩씬에서 조회 후 캐싱
    // 3. 인게임씬에서 캐싱된 데이터로 사용 및 주기적인 로직으로 갱신


    //뒤끝 콘솔에 업로드한 차트 데이터만 모아놓은 클래스
    public class BackendChart {
        public readonly BackendData.Chart.AllChart ChartInfo = new(); // 모든 차트
        public readonly BackendData.Chart.Weapon.Manager Weapon = new(); // Weapon 차트
        public readonly BackendData.Chart.Enemy.Manager Enemy = new(); // enemyChart 차트
        public readonly BackendData.Chart.Stage.Manager Stage = new(); // Stage 차트
        public readonly BackendData.Chart.Item.Manager Item = new(); // 아이템 차트
        public readonly BackendData.Chart.Shop.Manager Shop = new(); // 샵 차트
        public readonly BackendData.Chart.Quest.Manager Quest = new(); // 퀘스트 차트
    }

    // 게임 정보 관리 데이터만 모아놓은 클래스
    public class BackendGameData {
        public readonly BackendData.GameData.WeaponInventory.Manager WeaponInventory = new(); // WeaponInventory 테이블 데이터
        public readonly BackendData.GameData.WeaponEquip.Manager WeaponEquip = new(); // WeaponEquip 테이블 데이터
        public readonly BackendData.GameData.QuestAchievement.Manager QuestAchievement = new(); // QuestAchievement 테이블 데이터
        public readonly BackendData.GameData.UserData UserData = new(); // UserData 테이블 데이터
        public readonly BackendData.GameData.ItemInventory ItemInventory = new(); // ItemInventory 테이블 데이터
        
        public readonly Dictionary<string, BackendData.Base.GameData>
            GameDataList = new Dictionary<string, GameData>();

        public BackendGameData() {
            GameDataList.Add("내 무기 정보", WeaponInventory);
            GameDataList.Add("내 무기 장비", WeaponEquip);
            GameDataList.Add("내 퀘스트 정보", QuestAchievement);
            GameDataList.Add("내 유저 정보", UserData);
            GameDataList.Add("내 아이템 정보", ItemInventory);
        }
    }

    public BackendChart   Chart = new(); // 차트 모음 클래스 생성
    public BackendGameData GameData = new(); // 게임 모음 클래스 생성
    public BackendData.Rank.Manager Rank = new(); // 랭킹 관리 클래스 생성
    public BackendData.Post.Manager Post = new(); // 우편 클래스 생성

 // 게임 데이터의 저장, 조회등 일괄적으로 처리하기 위한 List

    private bool _isErrorOccured = false; // 치명적인 에러 발생 여부 

    
    // 뒤끝 매니저 초기화 함수
    public void Init() {
        var initializeBro = Backend.Initialize(true);

        // 초기화 성공시
        if (initializeBro.IsSuccess()) {
            Debug.Log("뒤끝 초기화가 완료되었습니다.");
            CreateSendQueueMgr();
            SetErrorHandler();
        }
        //초기화 실패시
        else {
            StaticManager.UI.AlertUI.OpenErrorUI(GetType().Name,MethodBase.GetCurrentMethod()?.ToString(), initializeBro.ToString());
        }
    }

    //비동기 함수를 메인쓰레드로 보내어 UI에 용이하게 접근하도록 도와주는 Poll 함수
    void Update() {
        if (Backend.IsInitialized) {
            Backend.AsyncPoll();
            Backend.ErrorHandler.Poll();
        }
    }

    // 모든 뒤끝 함수에서 에러 발생 시, 각 에러에 따라 호출해주는 핸들러
    private void SetErrorHandler() {
        Backend.ErrorHandler.InitializePoll(true);

        // 서버 점검 에러 발생 시
        Backend.ErrorHandler.OnMaintenanceError = () => {
            Debug.Log("점검 에러 발생!!!");
            StaticManager.UI.AlertUI.OpenErrorUIWithText("서버 점검 중", "현재 서버 점검중입니다.\n타이틀로 돌아갑니다.");
        };
        // 403 에러 발생시
        Backend.ErrorHandler.OnTooManyRequestError = () => {
            StaticManager.UI.AlertUI.OpenErrorUIWithText("비정상적인 행동 감지", "비정상적인 행동이 감지되었습니다.\n타이틀로 돌아갑니다.");
        };
        // 액세스토큰 만료 후 리프레시 토큰 실패 시
        Backend.ErrorHandler.OnOtherDeviceLoginDetectedError = () => {
            StaticManager.UI.AlertUI.OpenErrorUIWithText("다른 기기 접속 감지", "다른 기기에서 로그인이 감지되었습니다.\n타이틀로 돌아갑니다.");
        };
    }

    // 로딩씬에서 할당할 뒤끝 정보 클래스 초기화
    public void InitInGameData() {

        Chart = new();
        GameData = new();
        Rank = new();
        Post = new();
    }

    //SendQueue를 관리해주는 SendQueue 매니저 생성
    private void CreateSendQueueMgr() {
        var obj = new GameObject();
        obj.name = "SendQueueMgr";
        obj.transform.SetParent(this.transform);
        obj.AddComponent<SendQueueMgr>();
    }
    
    // 일정주기마다 데이터를 저장/불러오는 코루틴 시작(인게임 시작 시)
    public void StartUpdate() {
        StartCoroutine(UpdateGameDataTransaction());
        StartCoroutine(UpdateRankScore());
        StartCoroutine(GetAdminPostList());
    }

    // 호출 시, 코루틴 내 함수들의 동작을 멈추게 하는 함수
    public void StopUpdate() {
        Debug.Log("자동 저장을 중지합니다.");
        _isErrorOccured = false;
    }


    // 일정주기마다 내 게임정보 데이터를 묶어서 저장하는 코루틴 함수
    private IEnumerator UpdateGameDataTransaction() {
        var seconds = new WaitForSeconds(300);
        yield return seconds;

        while (_isErrorOccured) {
            UpdateAllGameData(null);

            yield return seconds;
        }
    }

    // 업데이트가 발생한 이후에 호출에 대한 응답을 반환해주는 대리자 함수
    public delegate void AfterUpdateFunc(BackendReturnObject callback);
    
    // 값이 바뀐 데이터가 있는지 체크후 바뀐 데이터들은 바로 저장 혹은 트랜잭션에 묶어 저장을 진행하는 함수
    public void UpdateAllGameData(AfterUpdateFunc afterUpdateFunc) {
        string info = string.Empty;
        
        
        // 바뀐 데이터가 몇개 있는지 체크
        List<GameData> gameDatas = new List<GameData>();
        
        foreach (var gameData in GameData.GameDataList) {
            if (gameData.Value.IsChangedData) {
                info += gameData.Value.GetTableName() + "\n";
                gameDatas.Add(gameData.Value);
            }
        }
        
        if (gameDatas.Count <= 0) {
            afterUpdateFunc(null); // 지정한 대리자 함수 호출

            // 업데이트할 목록이 존재하지 않습니다.
        }
        else if (gameDatas.Count == 1) {
            
            //하나라면 찾아서 해당 테이블만 업데이트
            foreach (var gameData in gameDatas) {
                if (gameData.IsChangedData) {
                    gameData.Update(callback => {
                        
                        //성공할경우 데이터 변경 여부를 false로 변경
                        if (callback.IsSuccess()) {
                            gameData.IsChangedData = false;
                        }
                        else {
                            SendBugReport(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), callback.ToString() + "\n" + info);
                        }
                        Debug.Log($"UpdateV2 : {callback}\n업데이트 테이블 : \n{info}");
                        if (afterUpdateFunc == null) {
                            
                        }
                        else {
                            afterUpdateFunc(callback); // 지정한 대리자 함수 호출
                        }
                    });
                }
            }
        }
        else {
            // 2개 이상이라면 트랜잭션에 묶어서 업데이트
            // 단 10개 이상이면 트랜잭션 실패 주의
            List<TransactionValue> transactionList = new List<TransactionValue>();
            
            // 변경된 데이터만큼 트랜잭션 추가
            foreach (var gameData in gameDatas) {
                transactionList.Add(gameData.GetTransactionUpdateValue());
            }

            SendQueue.Enqueue(Backend.GameData.TransactionWriteV2, transactionList, callback => {
                Debug.Log($"Backend.BMember.TransactionWriteV2 : {callback}");

                if (callback.IsSuccess()) {
                    foreach (var data in gameDatas) {
                        data.IsChangedData = false;
                    }
                }
                else {
                    SendBugReport(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), callback.ToString() + "\n" + info);
                }
                
                Debug.Log($"TransactionWriteV2 : {callback}\n업데이트 테이블 : \n{info}");
                
                if (afterUpdateFunc == null) {
                            
                }
                else {

                    afterUpdateFunc(callback);  // 지정한 대리자 함수 호출
                }
            });
        }
    }

    // 일정 주기마다 랭킹 데이터 업데이트 호출
    private IEnumerator UpdateRankScore() {
        var seconds = new WaitForSeconds(650);

        yield return seconds;

        // 에러 발생시 true가 될때까지
        while (_isErrorOccured) {
            
            foreach (var li in Rank.List) {
                UpdateUserRankScore(li.uuid, null);
            }

            yield return seconds;
        }
    }

    public void UpdateUserRankScore(string uuid, AfterUpdateFunc afterUpdateFunc) {
        // 쓰기 비용의 부담이 클 경우에는 각 랭킹별로 Param을 업데이트 하도록 설정.(현재는 일괄 처리)
        // 바뀐 데이터가 몇개 있는지 체크
        List<GameData> gameDatas = new List<GameData>();
        
        foreach (var gameData in GameData.GameDataList) {
            gameDatas.Add(gameData.Value);
            
        }
        
        foreach (var li in Rank.List) {
            
            //업데이트하고자 하는 uuid 존재하는지 확인
            if (li.uuid.Equals(uuid)) {
                
                // 랭크 리스트에 있는 테이블 이름과 현재 테이블 이름이 있는지 확인하고 존재한다면 해당 게임테이블을 전체 업데이트한다
                int index = gameDatas.FindIndex(item => item.GetTableName().Equals(li.table));
                if (index < 0) {
                    afterUpdateFunc?.Invoke(null);
                }
                SendQueue.Enqueue(Backend.URank.User.UpdateUserScore, li.uuid, li.table,
                    gameDatas[index].GetInDate(), gameDatas[index].GetParam(),
                    callback => {
                        Debug.Log($"Backend.URank.User.UpdateUserScore({li.uuid}, {li.table}, {gameDatas[index].GetInDate()}) : {callback}");
                        if (!callback.IsSuccess()) {
                            SendBugReport(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), callback.ToString());
                        }

                        if (afterUpdateFunc != null) {
                            afterUpdateFunc.Invoke(callback);
                        }
                    });
            }
        }
    }

    // 일정 주기마다 우편을 불러오는 코루틴 함수
    private IEnumerator GetAdminPostList() {
        var seconds = new WaitForSeconds(600);
        yield return seconds;

        while (_isErrorOccured) {
            // 현재 post 함수 체크
            int postCount = StaticManager.Backend.Post.Dictionary.Count;
            
            //랭크보상은 수동으로 체크하도록 구성
            // 관리자우편은 자동으로 일정주기마다 호출하도록 구성
            
            Post.GetPostList(PostType.Admin, (success, info) => {
                if (success) {
                    //호출하기 전 우편의 갯수와 동일하지 않다면 우편 아이콘 오른쪽에 표시
                    if (postCount != Post.Dictionary.Count) {
                        if (StaticManager.Backend.Post.Dictionary.Count > 0) {
                            FindObjectOfType<InGameScene.RightButtonGroupManager>().SetPostIconAlert(true);
                        }
                    }
                }
                else {
                    //에러가 발생할 경우 버그 리포트 발송
                    SendBugReport(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), info);
                }
            });
            yield return seconds;
        }
    }

    // 에러 발생시 게임로그를 삽입하는 함수
    public void SendBugReport(string className, string functionName, string errorInfo, int repeatCount = 3) {
        
        // 에러가 실패할 경우 재귀함수를 통해 최대 3번까지 호출을 시도한다.
        if (repeatCount <= 0) {
            return;
        }

        // 아직 로그인되지 않을 경우 뒤끝 함수 호출이 불가능하여 UI에 띄운다.
        if (string.IsNullOrEmpty(Backend.UserInDate)) {
            StaticManager.UI.AlertUI.SetYetLoginErrorText();
            return;
        }

        Param param = new Param();
        param.Add("className", className);
        param.Add("functionName", functionName);
        param.Add("errorPath", errorInfo);

        // [뒤끝] 로그 삽입 함수
        Backend.GameLog.InsertLog("error", param, 7, callback => {
            // 에러가 발생할 경우 재귀
            if (callback.IsSuccess() == false) {
                SendBugReport(className, functionName, errorInfo, repeatCount - 1);
            }
        });
    }
}