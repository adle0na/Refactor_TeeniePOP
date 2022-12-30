// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using Unity.VisualScripting;

namespace BackendData.GameData.WeaponInventory {

    //===============================================================
    // WeaponInventory 테이블의 데이터를 관리하는 클래스
    //===============================================================
    public class Manager : Base.GameData {
        
        // WeaponInventory의 각 아이템을 담는 Dictionary
        private Dictionary<string, Item> _dictionary = new ();

        // 다른 클래스에서 Add, Delete등 수정이 불가능하도록 읽기 전용 Dictionary
        public IReadOnlyDictionary<string, Item> Dictionary =>
            (IReadOnlyDictionary<string, Item>)_dictionary.AsReadOnlyCollection();

        // 테이블 이름 설정 함수
        public override string GetTableName() {
            return "WeaponInventory";
        }

        // 컬럼 이름 설정 함수
        public override string GetColumnName() {
            return "WeaponInventory";
        }        
        
        // 데이터가 존재하지 않을 경우, 초기값 설정
        protected override void InitializeData() {
            _dictionary.Clear();
            AddWeapon(1);        
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
                int weaponLevel = int.Parse(gameDataJson[i]["WeaponLevel"].ToString());
                string myWeaponId = gameDataJson[i]["MyWeaponId"].ToString();
                int weaponChartId = int.Parse(gameDataJson[i]["WeaponChartId"].ToString());

                var weaponInfo = StaticManager.Backend.Chart.Weapon.Dictionary[weaponChartId];

                _dictionary.Add(myWeaponId,
                    new Item(myWeaponId, weaponLevel, weaponInfo));
            }
        }
        
        // 인벤토리에 무기 추가
        public string AddWeapon(int weaponId) {
            IsChangedData = true;
            
            // 차트에서 무기 정보 검색
            var weaponInfo = StaticManager.Backend.Chart.Weapon.Dictionary[weaponId];

            // 무기의 고유 아이디를 만들기 위해 UnixTime을 생성(현재시간을 숫자로 바꾼 데이터)
            // 0.00001초의 오차없이 데이터를 생성해야만 중복이 되므로, 유저 하나의 데이터로는 유니크 id가 가능하다.
            DateTime now = DateTime.Now;
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            string myWeaponID = Convert.ToString(Convert.ToInt64((now - epoch).TotalMilliseconds));

            _dictionary.Add(myWeaponID, new Item(myWeaponID, 1, weaponInfo));

            return myWeaponID;
        }
        
    }
}