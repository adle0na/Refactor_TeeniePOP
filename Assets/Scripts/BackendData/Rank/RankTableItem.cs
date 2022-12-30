// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using BackEnd;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

namespace BackendData.Rank {
    public enum RankType {
        user,
        guild
    }

    //===============================================================
    //  서버에서 불러온 랭킹 리스트의 테이블 데이터
    //===============================================================
    public class RankTableItem {
        public RankType rankType { get; private set; }
        public string date { get; private set; }
        public string uuid { get; private set; }
        public string order { get; private set; }
        public bool isReset { get; private set; }
        public string title { get; private set; }
        public string table { get; private set; }
        public string column { get; private set; }

        //일회성 랭킹에만 존재
        public DateTime rankStartDateAndTime { get; private set; }
        public DateTime rankEndDateAndTime { get; private set; }

        //추가 항목이 있을 경우에만 존재
        public string extraDataColumn { get; private set; }
        public string extraDataType { get; private set; }

        public int totalUserCount { get; private set; }

        public DateTime UpdateTime { get; private set; }
        public DateTime MyRankUpdateTime { get; private set; }
        public RankUserItem MyRankItem { get; private set; }

        private List<RankUserItem> _userList = new();
        private IReadOnlyList<RankUserItem> UserList => (IReadOnlyList<RankUserItem>)_userList.AsReadOnlyList();

        // 랭킹을 불러온 후에 바뀐 List값을 리턴하는 대리자 함수
        public delegate void GetListFunc(bool isSuccess, IReadOnlyList<RankUserItem> rankList);

        // 랭킹 리스트를 전달하는 함수
        public void GetRankList(GetListFunc getListFunc) {
            // 갱신한지 5분이 지나지 않았으면 캐싱된 값을 리턴
            if ((DateTime.UtcNow - UpdateTime).Minutes < 5) {
                getListFunc(true, UserList);
                return;
            }
            string className = GetType().Name;
            string funcName = MethodBase.GetCurrentMethod()?.Name;
            // 5분이 지났을 경우에는 랭킹 함수 호출
            // [뒤끝] 랭킹 리스트 불러오기 함수
            SendQueue.Enqueue(Backend.URank.User.GetRankList, uuid, 10, callback => {
                try {
                    Debug.Log($"Backend.URank.User.GetRankList({uuid}) : {callback}");
                    if (callback.IsSuccess()) {
                        UpdateTime = DateTime.UtcNow;

                        JsonData rankJson = callback.GetFlattenJSON();

                        totalUserCount = int.Parse(rankJson["totalCount"].ToString());
                        _userList.Clear();
                        foreach (JsonData tempJson in rankJson["rows"]) {
                            _userList.Add(new RankUserItem(tempJson, table, extraDataColumn));
                        }

                        getListFunc(true, UserList);
                    }
                    else {
                        getListFunc(false, UserList);
                    }
                }
                catch (Exception e) {
                    StaticManager.UI.AlertUI.OpenErrorUI(className, funcName, e);
                    getListFunc(false, UserList);
                }
            });
        }

        public delegate void GetMyRankFunc(bool isSuccess, RankUserItem rankItem);

        // 내 랭킹을 불러오기 위해 갱신을 한번 했는지 여부
        private bool _isTwiceRepeat = false;

        // 내 랭킹이 갱신되지 않았을 경우에는 Update를 한번 호출
        public void GetMyRank(GetMyRankFunc getMyRankFunc) {
            // 5분이 지나지 않았을 경우에는 캐싱된 값 리턴
            if ((DateTime.UtcNow - MyRankUpdateTime).Minutes < 5) {
                getMyRankFunc(true, MyRankItem);
                return;
            }

            string className = GetType().Name;
            string funcName = MethodBase.GetCurrentMethod()?.Name;
            
            SendQueue.Enqueue(Backend.URank.User.GetMyRank, uuid, callback => {
                try {
                    Debug.Log($"Backend.URank.User.GetMyRank({uuid}) : {callback}");
                    if (callback.IsSuccess()) {
                        // 갱신 주기 갱신
                        MyRankUpdateTime = DateTime.UtcNow;

                        // 내 데이터 생성
                        MyRankItem = new RankUserItem(callback.FlattenRows()[0], table, extraDataColumn);

                        // 생성된 데이터로 리턴
                        getMyRankFunc(true, MyRankItem);
                    }
                    else {
                        // 에러가 발생하였을 경우

                        // 만약 내 랭킹이 갱신되어있지 않을 경우
                        if (callback.GetMessage().Contains("userRank not found")) {
                            // 갱신을 하였는대도 다시 여기를 호출할 경우
                            if (_isTwiceRepeat) {
                                // 무한 루프의 위험이 있기 때문에 bool값으로 한번만 호출하게 제어한다
                                _isTwiceRepeat = false;
                                getMyRankFunc(false, MyRankItem);
                                return;
                            }

                            _isTwiceRepeat = false;

                            StaticManager.Backend.UpdateUserRankScore(uuid, (afterUpdateCallback) => {
                                if (afterUpdateCallback == null) {
                                    throw new Exception("afterUpdateCallback가 null입니다.");
                                }
                                if (afterUpdateCallback.IsSuccess()) {
                                    // 성공하였을 경우 다시한번 내 랭킹 불러오기 호출
                                    _isTwiceRepeat = true;
                                    GetMyRank(getMyRankFunc);
                                }
                                else {
                                    StaticManager.UI.AlertUI.OpenWarningUI("랭킹 불러오기 불가 안내",
                                        "랭킹을 불러올 수 없습니다.\n5분 뒤에 다시 시도해주세요");
                                    getMyRankFunc(false, MyRankItem);
                                }
                            });
                        }
                        else {
                            // "userRank not found" 에러 외 다른 에러일 경우, 그냥 리턴
                            // 에러는 StaticManager.Backend.UpdateUserRankScore에서 기록한다.
                            getMyRankFunc(false, MyRankItem);
                        }
                    }
                }
                catch (Exception e) {
                    StaticManager.UI.AlertUI.OpenErrorUI(className, funcName, e);
                    getMyRankFunc(false, MyRankItem);
                }
            });
        }

        // Backend.URank.User.GetRankTableList()의 리턴데이터 파싱
        public RankTableItem(JsonData gameDataJson) {
            date = gameDataJson["date"].ToString();
            uuid = gameDataJson["uuid"].ToString();
            order = gameDataJson["order"].ToString();
            isReset = gameDataJson["isReset"].ToString() == "true" ? true : false;
            title = gameDataJson["title"].ToString();
            table = gameDataJson["table"].ToString();
            column = gameDataJson["column"].ToString();

            if (gameDataJson.ContainsKey("rankStartDateAndTime")) {
                rankStartDateAndTime = DateTime.Parse(gameDataJson["rankStartDateAndTime"].ToString());
                rankEndDateAndTime = DateTime.Parse(gameDataJson["rankEndDateAndTime"].ToString());
            }

            if (gameDataJson.ContainsKey("extraDataColumn")) {
                extraDataColumn = gameDataJson["extraDataColumn"].ToString();
                extraDataType = gameDataJson["extraDataType"].ToString();
            }

            if (!Enum.TryParse<RankType>(gameDataJson["rankType"].ToString(), out var tempRankType)) {
                throw new Exception($"[{uuid}] - 지정되지 않은 RankType 입니다.");
            }

            rankType = tempRankType;
            MyRankUpdateTime = DateTime.MinValue;
            UpdateTime = DateTime.MinValue;
        }
    }
}