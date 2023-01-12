// Copyright 2013-2022 AFI, INC. All rights reserved.

using LitJson;
using UnityEngine;

namespace BackendData.Chart.Item
{
    //===============================================================
    // Item 차트의 각 row 데이터 클래스
    //===============================================================
    public class Item
    {
        public Item(JsonData json)
        {
            ItemID      = int.Parse(json["ItemID"].ToString());
            ItemName    = json["ItemName"].ToString();
            ItemContent = json["ItemContent"].ToString();
            ImageName   = json["ImageName"].ToString();
        }
        
        public int ItemID         { get; private set; }
        public string ItemName    { get; private set; }
        public string ItemContent { get; private set; }
        public string ImageName   { get; private set; }

        public Sprite ItemImage;
    }
}

