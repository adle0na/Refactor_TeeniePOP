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
        public int Level { get; private set; }
        public float Money { get; private set; }
        public string LastLoginTime { get; private set; }

        public float Exp { get; private set; }

        public float MaxExp { get; private set; }
        public float Jewel { get; private set; }

        public float DayUsingGold { get; set; }
        public float WeekUsingGold { get; set; }
        public float MonthUsingGold { get; set; }

        public int DayDefeatEnemyCount { get; private set; }
        public int WeekDefeatEnemyCount { get; private set; }
        public int MonthDefeatEnemyCount { get; private set; }
    }

    //===============================================================
    // UserData 테이블의 데이터를 담당하는 클래스(함수)
    //===============================================================
    public partial class UserData : Base.GameData {
        
        // 데이터가 존재하지 않을 경우, 초기값 설정
        protected override void InitializeData() {
            Level = 1;
            Money = 10000;
            MaxExp = 100;
        }

        // Backend.GameData.GetMyData 호출 이후 리턴된 값을 파싱하여 캐싱하는 함수
        // 서버에서 데이터를 불러오늖 함수는 BackendData.Base.GameData의 BackendGameDataLoad() 함수를 참고해주세요
        protected override void SetServerDataToLocal(JsonData gameDataJson) {
            Level = int.Parse(gameDataJson["Level"].ToString());
            Exp = float.Parse(gameDataJson["Exp"].ToString());
            MaxExp = float.Parse(gameDataJson["MaxExp"].ToString());
            Money = float.Parse(gameDataJson["Money"].ToString());
            LastLoginTime = gameDataJson["LastLoginTime"].ToString();
            
            DayUsingGold = float.Parse(gameDataJson["DayUsingGold"].ToString());
            WeekUsingGold = float.Parse(gameDataJson["WeekUsingGold"].ToString());
            MonthUsingGold = float.Parse(gameDataJson["MonthUsingGold"].ToString());
            
            DayDefeatEnemyCount = int.Parse(gameDataJson["DayDefeatEnemyCount"].ToString());            
            WeekDefeatEnemyCount = int.Parse(gameDataJson["WeekDefeatEnemyCount"].ToString());
            MonthDefeatEnemyCount = int.Parse(gameDataJson["MonthDefeatEnemyCount"].ToString());
            
            Jewel = gameDataJson.ContainsKey("Jewel") ? float.Parse(gameDataJson["Jewel"].ToString()) : 0;
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

            param.Add("Level", Level);
            param.Add("Money", Money);
            param.Add("Exp", Exp);
            param.Add("MaxExp", MaxExp);
            param.Add("LastLoginTime", string.Format("{0:MM-DD:HH:mm:ss.fffZ}", DateTime.Now.ToString(CultureInfo.InvariantCulture)));
            
            param.Add("DayUsingGold", DayUsingGold);
            param.Add("WeekUsingGold", WeekUsingGold);
            param.Add("MonthUsingGold", MonthUsingGold);

            param.Add("DayDefeatEnemyCount", DayDefeatEnemyCount);
            param.Add("WeekDefeatEnemyCount", WeekDefeatEnemyCount);
            param.Add("MonthDefeatEnemyCount", MonthDefeatEnemyCount);

            return param;
        }
        
        // 적 처치 횟수를 갱신하는 함수
        public void CountDefeatEnemy() {
            DayDefeatEnemyCount++;
            WeekDefeatEnemyCount++;
            MonthDefeatEnemyCount++;
        }

        // 유저의 정보를 변경하는 함수
        public void UpdateUserData(float money, float exp) {
            IsChangedData = true;
            
            Exp += exp;
            Money += money;

            if (money < 0) {
                float tempMoney = Math.Abs(money);
                DayUsingGold += tempMoney;
                WeekUsingGold += tempMoney;
                MonthUsingGold += tempMoney;
            }

            if (Exp > MaxExp) {
                while (Exp > MaxExp) {
                    LevelUp();
                }
            }
        }

        // 레벨업하는 함수
        private void LevelUp() {
            //Exp가 MaxExp를 초과했을 경우를 대비하여 빼기
            Exp -= MaxExp;

            //기존 경험치에서 1.1배
            MaxExp = (float)Math.Truncate(MaxExp * 1.1);

            Level++;
        }
    }
}