// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using Unity.VisualScripting;


namespace BackendData.GameData.QuestAchievement {
    
    //===============================================================
    // WeaponInventory 테이블의 데이터를 관리하는 클래스
    //===============================================================
    public class Manager : Base.GameData {
        
        // QuestAchievement의 각 아이템을 담는 Dictionary
        private Dictionary<int, Item> _dictionary = new();

        // 다른 클래스에서 Add, Delete등 수정이 불가능하도록 읽기 전용 Dictionary
        public IReadOnlyDictionary<int, Item> Dictionary =>
            (IReadOnlyDictionary<int, Item>)_dictionary.AsReadOnlyCollection();

        // 테이블 이름 설정 함수
        public override string GetTableName() {
            return "QuestAchievement";
        }
        
        // 컬럼 이름 설정 함수
        public override string GetColumnName() {
            return "QuestAchievement";
        }
                
        // 데이터가 존재하지 않을 경우, 초기값 설정
        protected override void InitializeData() {
            foreach (var chartData in StaticManager.Backend.Chart.Quest.Dictionary) {
                int questId = chartData.Value.QuestID;
                _dictionary.Add(questId, new Item(questId, false));
            }
        }
        
        // 데이터 저장 시 저장할 데이터를 뒤끝에 맞게 파싱하는 함수
        // Dictionary 하나만 삽입
        public override Param GetParam() {
            Param param = new Param();
            param.Add(GetColumnName(), _dictionary);

            return param;
        }

        // Backend.GameData.GetMyData 호출 이후 리턴된 값을 파싱하여 캐싱하는 함수
        // 서버에서 데이터를 불러오늖 함수는 BackendData.Base.GameData의 BackendGameDataLoad() 함수를 참고해주세요
        protected override void SetServerDataToLocal(JsonData gameDataJson) {
            for (int i = 0; i < gameDataJson.Count; i++) {
                int questId = int.Parse(gameDataJson[i]["QuestId"].ToString());
                bool isQuestAchieve = Boolean.Parse(gameDataJson[i]["IsAchieve"].ToString());

                _dictionary.Add(questId, new Item(questId, isQuestAchieve));
            }
        }

        // 특정 퀘스트를 달성처리하는 함수
        public void SetAchieve(int questId) {
            IsChangedData = true;
            _dictionary[questId].SetAchieve();
        }
    }
}