// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using LitJson;

namespace BackendData.Chart.Quest
{
    public enum QuestType
    {
        Score,
        UseGold,
        UseHeart
    }

    public enum QuestRepeatType
    {
        Once,
        Day,
        Week
    }


    public enum RewardItemType
    {
        Item,
    }

    //===============================================================
    // Quest 차트의 각 row 데이터 클래스
    //===============================================================
    public class Item
    {
        public class RewardStatClass
        {
            public float Gold { get; private set; }
            public float CupCake { get; private set; }

            public RewardStatClass(float gold, float CupCake)
            {
                Gold = gold;
                CupCake = CupCake;
            }
        }

        public class RewardItemClass
        {

            public RewardItemType RewardItemType { get; private set; }
            public int Id { get; private set; }
            public float Count { get; private set; }

            public RewardItemClass(string type, int id, float count)
            {
                if (!Enum.TryParse<RewardItemType>(type, out var rewardItemType))
                {
                    throw new Exception("지정되지 않은 RewardItemType 입니다.");
                }

                RewardItemType = rewardItemType;
                Id = id;
                Count = count;
            }
        }

        public int QuestID { get; private set; }
        public string QuestContent { get; private set; }
        public float RequestCount { get; private set; }
        public QuestType QuestType { get; private set; }
        public QuestRepeatType QuestRepeatType { get; private set; }

        public List<RewardStatClass> RewardStat { get; private set; }
        public List<RewardItemClass> RewardItem { get; private set; }

        public Dictionary<string, string> ExtraData { get; private set; }

        public Item(JsonData json)
        {
            QuestID = int.Parse(json["QuestID"].ToString());
            QuestContent = json["QuestContent"].ToString();
            RequestCount = float.Parse(json["RequestCount"].ToString());

            if (!Enum.TryParse<QuestType>(json["QuestType"].ToString(), out var questType))
            {
                throw new Exception($"Q{QuestID} - 지정되지 않은 QuestType 입니다.");
            }

            this.QuestType = questType;

            if (!Enum.TryParse<QuestRepeatType>(json["QuestRepeatType"].ToString(), out var questRepeatType))
            {
                throw new Exception($"Q{QuestID} - 지정되지 않은 QuestRepeatTypeEnum 입니다.");
            }

            QuestRepeatType = questRepeatType;

            try
            {
                string rewardStatString = json["RewardStat"].ToString();

                if (string.IsNullOrEmpty(rewardStatString) == false)
                {
                    RewardStat = new List<RewardStatClass>();
                    JsonData rewardStatJson = JsonMapper.ToObject(rewardStatString);

                    for (int i = 0; i < rewardStatJson.Count; i++)
                    {
                        float exp = float.Parse(rewardStatJson[i]["Exp"].ToString());
                        float money = float.Parse(rewardStatJson[i]["Money"].ToString());

                        RewardStat.Add(new RewardStatClass(exp, money));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"{GetType().Name} : {MethodBase.GetCurrentMethod()?.ToString()} : {e.ToString()}");
            }

            try
            {
                string rewardItemString = json["RewardItem"].ToString();

                if (string.IsNullOrEmpty(rewardItemString) == false)
                {
                    RewardItem = new List<RewardItemClass>();
                    JsonData rewardStatJson = JsonMapper.ToObject(rewardItemString);

                    foreach (JsonData tempJson in rewardStatJson)
                    {
                        string type = tempJson["Type"].ToString();
                        int id = int.Parse(tempJson["Id"].ToString());
                        float count = float.Parse(tempJson["Count"].ToString());

                        RewardItem.Add(new RewardItemClass(type, id, count));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Q{QuestID} - RewardItem 파싱 도중 에러가 발생했습니다.\n{e.StackTrace}");
            }

        }
    }
}