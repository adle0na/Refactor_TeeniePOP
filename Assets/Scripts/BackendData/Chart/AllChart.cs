// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using BackEnd;
using BackendData.Base;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

namespace BackendData.Chart {
   //===============================================================
   // Weapon, Stage등의 차트에 대한 차트 정보를 불러오는 클래스
   //===============================================================
   public class AllChart : Normal {
   
      // 차트의 파일 id를 관리하는 Dictionary
      private readonly Dictionary<string, string> _chartDictionary = new();
      // 다른 클래스에서 Add, Delete등 수정이 불가능하도록 읽기 전용 Dictionary
      public IReadOnlyDictionary<string, string> Dictionary => (IReadOnlyDictionary<string, string>)_chartDictionary.AsReadOnlyCollection();

      // 서버에서 데이터를 불러와 파싱하는 함수
      public override void BackendLoad(AfterBackendLoadFunc afterBackendLoadFunc) {

         bool isSuccess = false;
         string errorInfo = string.Empty;
         string className = GetType().Name;
         string funcName = MethodBase.GetCurrentMethod()?.Name;
      
         // [뒤끝] 차트 정보 불러오기 함수
         SendQueue.Enqueue(Backend.Chart.GetChartList, callback => {
            try {
               Debug.Log($"Backend.Chart.GetChartList : {callback}");

               if (!callback.IsSuccess()) {
                  throw new Exception(callback.ToString());
               }

               JsonData json = callback.FlattenRows();

               for (int i = 0; i < json.Count; i++) {
                  string chartName = json[i]["chartName"].ToString(); // 차트 이름 파싱
                  string selectedChartFileId = json[i]["selectedChartFileId"].ToString(); // 차트 파일 아이디 파싱
                  
                  if (_chartDictionary.ContainsKey(chartName)) {
                     Debug.LogWarning($"동일한 차트 키 값이 존재합니다 : {chartName} - {selectedChartFileId}"); // 희귀케이스
                  }
                  else {
                     _chartDictionary.Add(chartName, selectedChartFileId);
                  }
               }

               isSuccess = true;
            }
            catch (Exception e) {
               errorInfo = e.Message;
            }
            finally {
               afterBackendLoadFunc(isSuccess, className, funcName, errorInfo);
            }
         });
      }
   }
}