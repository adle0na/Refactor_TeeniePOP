// Copyright 2013-2022 AFI, INC. All rights reserved.

namespace BackendData.GameData.WeaponInventory {
    //===============================================================
    // WeaponInventory 테이블의 Dictionary에 저장될 각 무기 정보 클래스
    //===============================================================
    public class Item {

        // 현재 무기 스텟
        public class CurrentStat {
            public float Atk { get; private set; }
            public float Spd { get; private set; }
            public float Delay { get; private set; }
            public long UpgradePrice { get; private set; }
            
            public CurrentStat(int level, Chart.Weapon.Item item) {
                LevelUp(level, item);
            }

            // 차트에서 성장 스텟만큼 level과 곱한다
            public void LevelUp(int level, Chart.Weapon.Item weaponChartChart) {
                Atk = weaponChartChart.Atk + (level * weaponChartChart.GrowingAtk);
                Spd = weaponChartChart.Spd + (level * weaponChartChart.GrowingSpd);
                Delay = weaponChartChart.Delay + (level * weaponChartChart.GrowingDelay);
                UpgradePrice = weaponChartChart.Price;
            }
        }

        // 동일한 아이템 구매가 가능하기 때문에 itemId가 아닌 구별하기 위한 나만의 고유한 무기 아이디
        public string MyWeaponId { get; private set; }
        // 무기 레벨
        public int WeaponLevel { get; private set; } 
        // 무기의 차트 아이디(weaponId)
        public int WeaponChartId { get; private set; }

        // 기본 깡스텟 <- 계산할 때에는 버프스텟에 영향을 받는다
        private CurrentStat _normalStat;

        //Param에 클래스를 넣을 경우, public 되어있는 정보가 자동으로 삽입된다.
        //뒤끝 Dictionary<class> 삽입 시, 데이터를 제외시키려면 함수로 구현해야한다.
        private Chart.Weapon.Item _weaponChartData;

        // 데이터 저장 시 저장할 데이터를 뒤끝에 맞게 파싱하는 함수
        public Item(string myWeaponId, int weaponLevel,
            Chart.Weapon.Item weaponData) {
            MyWeaponId = myWeaponId;
            WeaponLevel = weaponLevel;
            WeaponChartId = weaponData.WeaponID;
            _weaponChartData = weaponData;
            _normalStat = new CurrentStat(weaponLevel, weaponData);
        }

        // 해당 무기의 차트 정보
        public Chart.Weapon.Item GetWeaponChartData() {
            return _weaponChartData;
        }

        // 현재 무기 스텟
        public CurrentStat GetCurrentWeaponStat() {
            return _normalStat;
        }

        // 레벨업
        public void LevelUp() {
            WeaponLevel++;
            _normalStat.LevelUp(WeaponLevel, _weaponChartData);
        }
    }
}