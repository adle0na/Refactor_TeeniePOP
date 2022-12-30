// Copyright 2013-2022 AFI, INC. All rights reserved.

using LitJson;

namespace BackendData.Rank {
    //===============================================================
    //  서버에서 불러온 랭킹 리스트의 각각의 데이터
    //===============================================================
    public class RankUserItem
    {
        public string gamerInDate { get; private set; }
        public string nickname { get; private set; }
        public string score { get; private set; }
        public string index { get; private set; }
        public string rank { get; private set; }
        public string extraData  { get; private set; }
        public string tableName  { get; private set; }

        public RankUserItem(JsonData gameDataJson, string table, string extraDataColumnName) {
            tableName = table;

            gamerInDate = gameDataJson["gamerInDate"].ToString();
            nickname = gameDataJson.ContainsKey("nickname") ? gameDataJson["nickname"].ToString() : "-";
            score = gameDataJson["score"].ToString();
            index = gameDataJson["index"].ToString();
            rank = gameDataJson["rank"].ToString();
            
            if (!string.IsNullOrEmpty(extraDataColumnName) && gameDataJson.ContainsKey(extraDataColumnName)) {
               extraData = gameDataJson[extraDataColumnName].ToString();
            }
        }
    }
}