// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using BackEnd;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

namespace BackendData.Rank {
    //===============================================================
    //  서버에서 불러온 랭킹 데이터를 제어하는 클래스
    //===============================================================
    public class Manager : Base.Normal {
        
        List<RankTableItem> rankTableItemList = new ();
        public IReadOnlyList<RankTableItem> List => (IReadOnlyList<RankTableItem>)rankTableItemList.AsReadOnlyList();

        
        public override void BackendLoad(AfterBackendLoadFunc afterBackendLoadFunc) {

            bool isSuccess = false;
            string className = GetType().Name;
            string funcName = MethodBase.GetCurrentMethod()?.Name;
            string errorInfo = string.Empty;
            
            //[뒤끝] 모든 유저 랭킹 설정값 정보 조회 함수 호출
            SendQueue.Enqueue(Backend.URank.User.GetRankTableList, callback => {
                try {
                    Debug.Log($"Backend.URank.User.GetRankTableList : {callback}");

                    if (callback.IsSuccess()) {
                                        
                        JsonData rankTableListJson = callback.FlattenRows();

                        // 성공 시 리턴된 값을 이용하여 파싱.
                        for (int i = 0; i < rankTableListJson.Count; i++) {
                            RankTableItem rankTableItem = new RankTableItem(rankTableListJson[i]);
                            rankTableItemList.Add(rankTableItem);
                        }
                    }
                    else {
                        if (callback.GetMessage().Contains("rank not found")) {
                            StaticManager.UI.AlertUI.OpenWarningUI("랭킹이 존재하지 않습니다.", "랭킹을 찾을  수 없습니다.\n게임 데이터가 생성된 이후 해당 데이터를 이용하여 랭킹을 생성해주시기 바랍니다.");

                        }
                        else {
                            throw new Exception(callback.ToString());
                        }
                    }
                    isSuccess = true;
                }
                catch (Exception e) {
                    errorInfo = e.ToString();
                }
                finally {
                    afterBackendLoadFunc(isSuccess, className, funcName, errorInfo);
                }
            });
            
 
        }
    }
}