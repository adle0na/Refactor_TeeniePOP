// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using LitJson;
using Unity.VisualScripting;

namespace BackendData.Chart.LevelData {
    //===============================================================
    // LevelData 차트의 각 row 데이터 클래스
    //===============================================================
    public class Item
    {
        #region KeyIndexs

        public int    level_Id        { get; private set; }
        public int    spawnPing_1     { get; private set; }
        public int    spawnPing_2     { get; private set; }
        public int    spawnPing_3     { get; private set; }
        public int    spawnPing_4     { get; private set; }
        public int    spawnPing_5     { get; private set; }
        public int    spawnPing_6     { get; private set; }
        public int    spawnPing_Goal1 { get; private set; }
        public int    spawnPing_Goal2 { get; private set; }
        
        public int    spawnPing_Goal3 { get; private set; }
        public int    spawnPing_Goal4 { get; private set; }
        public int    Play_drag       { get; private set; }
        public int    CatchMode_type  { get; private set; }
        public int    CatchMode_time  { get; private set; }
        public int    CatchMode_Touch { get; private set; }
        public string CatchMode_ping  { get; private set; }
        
        #endregion

        #region Json Parsing
        
        public Item(JsonData json)
        {
            level_Id        = int.Parse(json["level_Id"].ToString());
            spawnPing_1     = int.Parse(json["spawnPing_1"].ToString());
            spawnPing_2     = int.Parse(json["spawnPing_2"].ToString());
            spawnPing_3     = int.Parse(json["spawnPing_3"].ToString());
            spawnPing_4     = int.Parse(json["spawnPing_4"].ToString());
            spawnPing_5     = int.Parse(json["spawnPing_5"].ToString());
            spawnPing_6     = int.Parse(json["spawnPing_6"].ToString());
            spawnPing_Goal1 = int.Parse(json["spawnPing_Goal1"].ToString());
            spawnPing_Goal2 = int.Parse(json["spawnPing_Goal2"].ToString());
            spawnPing_Goal3 = int.Parse(json["spawnPing_Goal3"].ToString());
            spawnPing_Goal4 = int.Parse(json["spawnPing_Goal4"].ToString());
            Play_drag       = int.Parse(json["Play_drag"].ToString());
            CatchMode_type  = int.Parse(json["CatchMode_type"].ToString());
            CatchMode_time  = int.Parse(json["CatchMode_time"].ToString());
            CatchMode_Touch = int.Parse(json["CatchMode_Touch"].ToString());
            
            CatchMode_ping  = (json["CatchMode_ping"].ToString());
        }
        
        #endregion
    }
}