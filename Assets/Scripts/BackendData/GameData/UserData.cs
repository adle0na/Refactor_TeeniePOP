// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using BackEnd;
using LitJson;
using UnityEngine;

namespace BackendData.GameData {
    //===============================================================
    // UserData 테이블의 데이터를 담당하는 클래스(변수)
    //===============================================================
    public partial class UserData {
        public int HighLevel { get; private set; }
        public int Gold { get; private set; }
        
        public int Energy { get; private set; }
        
        public string CollectedPings { get; private set; }
        
        public string LastLoginTime { get; private set; }
        
        public float DayUsingGold { get; set; }
        public float WeekUsingGold { get; set; }
        public float MonthUsingGold { get; set; }
    }

    //===============================================================
    // UserData 테이블의 데이터를 담당하는 클래스(함수)
    //===============================================================
    public partial class UserData : Base.GameData {
        
        // 데이터가 존재하지 않을 경우, 초기값 설정
        protected override void InitializeData() {
            HighLevel = 0;
            Gold = 0;
            Energy = 50;
        }

        // Backend.GameData.GetMyData 호출 이후 리턴된 값을 파싱하여 캐싱하는 함수
        // 서버에서 데이터를 불러오늖 함수는 BackendData.Base.GameData의 BackendGameDataLoad() 함수를 참고해주세요
        protected override void SetServerDataToLocal(JsonData gameDataJson) {
            HighLevel      = int.Parse(gameDataJson["HighLevel"].ToString());
            Gold           = int.Parse(gameDataJson["Gold"].ToString());
            CollectedPings = gameDataJson["LastLoginTime"].ToString();
            LastLoginTime  = gameDataJson["LastLoginTime"].ToString();
            
            DayUsingGold   = float.Parse(gameDataJson["DayUsingGold"].ToString());
            WeekUsingGold  = float.Parse(gameDataJson["WeekUsingGold"].ToString());
            MonthUsingGold = float.Parse(gameDataJson["MonthUsingGold"].ToString());
        }

        // 테이블 이름 설정 함수
        public override string GetTableName() {
            return "UserData";
        }

        // 컬럼 이름 설정 함수
        public override string GetColumnName() {
            return null;
        }

        // 데이터 저장 시 저장할 데이터를 뒤끝에 맞게 파싱하는 함수
        public override Param GetParam() {
            Param param = new Param();

            param.Add("Level", HighLevel);
            param.Add("Money", Gold);
            
            param.Add("LastLoginTime", string.Format("{0:MM-DD:HH:mm:ss.fffZ}", DateTime.Now.ToString(CultureInfo.InvariantCulture)));
            
            param.Add("DayUsingGold", DayUsingGold);
            param.Add("WeekUsingGold", WeekUsingGold);
            param.Add("MonthUsingGold", MonthUsingGold);

            return param;
        }

        // 유저의 정보를 변경하는 함수
        public void UpdateUserData(int gold, int energy) {
            IsChangedData = true;

            Energy += energy;
            
            Gold += gold;

            if (gold < 0) {
                float tempMoney = Math.Abs(gold);
                DayUsingGold += tempMoney;
                WeekUsingGold += tempMoney;
                MonthUsingGold += tempMoney;
            }
        }

    }
}