// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using LitJson;

namespace BackendData.Chart.Stage {
    //===============================================================
    // Stage 차트의 각 row 데이터 클래스
    //===============================================================
    public class Item {
        public class EnemyInfo {
            public int EnemyID { get; private set; }

            public float MultiStat { get; private set; }

            public EnemyInfo(int enemyID, float multiStat) {
                EnemyID = enemyID;
                MultiStat = multiStat;
            }
        }
    
        public int Level { get; private set; }
        public string StageName { get; private set; }
        public List<EnemyInfo> EnemyInfoList  { get; private set; }
        public Item(JsonData json){
            Level = int.Parse(json["Level"].ToString());
            StageName = json["StageName"].ToString();

            EnemyInfoList = new List<EnemyInfo>();

            string stageEnemyListString = json["EnemyList"].ToString();

            JsonData stageEnemyListJson = JsonMapper.ToObject(stageEnemyListString);
        
            foreach (JsonData enemy in stageEnemyListJson) {
                int itemID = int.Parse(enemy["id"].ToString());
                float multiStat = float.Parse(enemy["multiStat"].ToString());

                EnemyInfoList.Add(new EnemyInfo(itemID, multiStat));
            }
        }
    }
}