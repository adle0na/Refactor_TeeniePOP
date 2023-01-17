// Copyright 2013-2022 AFI, INC. All rights reserved.


using System;
using System.Collections.Generic;
using System.Reflection;
using BackEnd;
using LitJson;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour {

    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private Slider loadingSlider;

    private int _maxLoadingCount; // 총 뒤끝 함수를 호출할 갯수

    private int _currentLoadingCount; // 현재 뒤끝 함수를 호출한 갯수
    
    private delegate void BackendLoadStep();
    private readonly Queue<BackendLoadStep> _initializeStep = new Queue<BackendLoadStep>();

    void Awake() {
        if (Backend.IsInitialized == false) {
            SceneManager.LoadScene("LoginScene");
        }
    }
    
    void Start() {
        //Queue에 함수 Insert
        Init();
        
        // 뒤끝 데이터 초기화
        StaticManager.Backend.InitInGameData();
        
        //Queue에 저장된 함수 순차적으로 실행
        NextStep(true,string.Empty,string.Empty, string.Empty);
    }

    void Init() {
        _initializeStep.Clear();
        // 트랜잭션으로 불러온 후, 안불러질 경우 각자 Get 함수로 불러오는 함수 *중요*
        _initializeStep.Enqueue(() => {ShowDataName("트랜잭션 시도 함수"); TransactionRead(NextStep); });
        
        // 차트정보 불러오기 함수 Insert
        _initializeStep.Enqueue(() => { ShowDataName("모든 차트 정보"); StaticManager.Backend.Chart.ChartInfo.BackendLoad(NextStep); });
        _initializeStep.Enqueue(() => { ShowDataName("코인 보유량"); StaticManager.Backend.Chart.Stage.BackendChartDataLoad(NextStep); });
        _initializeStep.Enqueue(() => { ShowDataName("아이템 보유량"); StaticManager.Backend.Chart.Shop.BackendChartDataLoad(NextStep); });
        // 우편 정보 불러오기 함수 Insert
        //_initializeStep.Enqueue(() => { ShowDataName("관리자 우편 정보 불러오기"); StaticManager.Backend.Post.BackendLoad(NextStep); });
        //_initializeStep.Enqueue(() => { ShowDataName("랭킹 우편 정보 불러오기"); StaticManager.Backend.Post.BackendLoadForRank(NextStep); });

        //다음 씬으로 넘어가는 함수 Insert


        //게이지 바 지정
        _maxLoadingCount = _initializeStep.Count;
        loadingSlider.maxValue = _maxLoadingCount;

        _currentLoadingCount = 0;
        loadingSlider.value = _currentLoadingCount;

        // 로딩아이콘 활성화
        StaticManager.UI.SetLoadingIcon(true);
    }

    private void ShowDataName(string text) {
        string info = $"{text} 불러오는 중...({_currentLoadingCount}/{_maxLoadingCount})";
        loadingText.text = info;
        Debug.Log(info);
    }

    // 각 뒤끝 함수를 호출하는 BackendGameDataLoad에서 실행한 결과를 처리하는 함수
    // 성공하면 다음 스텝으로 이동, 실패하면 에러 UI를 띄운다.
    private void NextStep(bool isSuccess, string className, string funcName, string errorInfo) {
        if (isSuccess) {
            _currentLoadingCount++;
            loadingSlider.value = _currentLoadingCount;

            if (_initializeStep.Count > 0) {
                _initializeStep.Dequeue().Invoke();
            }
            else {
                InGameStart();
            }
        }
        else {
            Debug.Log("테이블값 안맞는거 있긴한데 넘겨드림");
            InGameStart();
        }
    }

    // 트랜잭션 읽기 호출 함수
    private void TransactionRead(BackendData.Base.Normal.AfterBackendLoadFunc func) {
        bool isSuccess = false;
        string className = GetType().Name;
        string functionName = MethodBase.GetCurrentMethod()?.Name;
        string errorInfo = string.Empty;
        
        //트랜잭션 리스트 생성
        List<TransactionValue> transactionList = new List<TransactionValue>();
        
        // 게임 테이블 데이터만큼 트랜잭션 불러오기
        foreach (var gameData in StaticManager.Backend.GameData.GameDataList) {
            transactionList.Add(gameData.Value.GetTransactionGetValue());
        }
        
        // [뒤끝] 트랜잭션 읽기 함수
        SendQueue.Enqueue(Backend.GameData.TransactionReadV2, transactionList, callback => {
            try {
                Debug.Log($"Backend.GameData.TransactionReadV2 : {callback}");
                
                // 데이터를 모두 불러왔을 경우
                if (callback.IsSuccess()) {
                    JsonData gameDataJson = callback.GetFlattenJSON()["Responses"];

                    int index = 0;
                    
                    foreach (var gameData in StaticManager.Backend.GameData.GameDataList) {
                        
                        _initializeStep.Enqueue(() => {
                            ShowDataName(gameData.Key);
                            // 불러온 데이터를 로컬에서 파싱
                            gameData.Value.BackendGameDataLoadByTransaction(gameDataJson[index++], NextStep);
                        });
                        _maxLoadingCount++;

                    }
                    // 최대 작업 개수 증가
                    loadingSlider.maxValue = _maxLoadingCount;
                    isSuccess = true;
                }
                else {
                    // 트랜잭션으로 데이터를 찾지 못하여 에러가 발생한다면 개별로 GetMyData로 호출
                    foreach (var gameData in StaticManager.Backend.GameData.GameDataList) {
                        _initializeStep.Enqueue(() => {
                            ShowDataName(gameData.Key);
                            // GetMyData 호출
                            gameData.Value.BackendGameDataLoad(NextStep);
                        });
                        _maxLoadingCount++;
                    }
                    // 최대 작업 개수 증가
                    loadingSlider.maxValue = _maxLoadingCount;
                    isSuccess = true;
                }
            }
            catch (Exception e) {
                errorInfo = e.ToString();
            }
            finally {
                func.Invoke(isSuccess, className,functionName, errorInfo);
            }
        });
    }

    // 인게임씬으로 이동가는 함수
    private void InGameStart(){
        StaticManager.UI.SetLoadingIcon(false);
        loadingText.text = "게임 시작하는 중";
        _initializeStep.Clear();
        StaticManager.Instance.ChangeScene("Scenes_P/InGameScene");

    }
}