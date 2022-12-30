// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using BackEnd;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;


namespace BackendData.GameData {
    //===============================================================
    // ItemInventory 테이블의 데이터를 담당하는 클래스
    //===============================================================
    public class ItemInventory : Base.GameData {
        
        // key의 int는 itemID, value의 int는 아이템 개수 
        private Dictionary<int, int> _dictionary = new ();
        public IReadOnlyDictionary<int, int> Dictionary => (IReadOnlyDictionary<int, int>)_dictionary.AsReadOnlyCollection();

        // 데이터가 존재하지 않을 경우, 초기값 설정
        protected override void InitializeData() {
            _dictionary.Clear();
        }
        
        // Backend.GameData.GetMyData 호출 이후 리턴된 값을 파싱하여 캐싱하는 함수
        // 서버에서 데이터를 불러오늖 함수는 BackendData.Base.GameData의 BackendGameDataLoad() 함수를 참고해주세요
        protected override void SetServerDataToLocal(JsonData gameDataJson) {
            
            var keys = gameDataJson.Keys;
            foreach (var key in keys) {
            
                _dictionary.Add(int.Parse(key), int.Parse(gameDataJson[key].ToString()));
            }
        }
        
        // 테이블 이름 설정 함수
        public override string GetTableName() {
            return "ItemInventory";
        }

        // 컬럼 이름 설정 함수
        public override string GetColumnName() {
            return "ItemInventory";
        }

        // 데이터 저장 시 저장할 데이터를 뒤끝에 맞게 파싱하는 함수
        public override Param GetParam() {
            Param param = new Param();
        
            param.Add(GetColumnName(), _dictionary);
            return param;
        }

        // 아이템을 추가하는 함수
        public void AddItem(int itemID, int count) {
            IsChangedData = true;
            if (_dictionary.ContainsKey(itemID)) {
                _dictionary[itemID] += count;
            }
            else {
                _dictionary.Add(itemID, count);
            }
        }

    }
}
