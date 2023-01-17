// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using BackEnd;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

namespace BackendData.Chart.Shop {
    //===============================================================
    // Shop 차트의 각 row 데이터 클래스
    //===============================================================
    public class Item {

        public enum ItemTypeEnum {
            Item
        }
    
        public enum ItemPriceTypeEnum {
            Gold,
            Jewel
        }
        public ItemTypeEnum ItemType { get; private set; }
        public int ItemID { get; private set; }
        public float ItemPrice { get; private set; }
        public ItemPriceTypeEnum ItemPriceType { get; private set; }
    
        public Item(JsonData json) {
            ItemID = int.Parse(json["ItemID"].ToString());
            ItemPrice = float.Parse(json["ItemPrice"].ToString());
            ItemType = StringToEnum<ItemTypeEnum>(json["ItemType"].ToString());
            ItemPriceType = StringToEnum<ItemPriceTypeEnum>(json["ItemPriceType"].ToString());
        }
    
        private T StringToEnum<T>(string data){

            return (T)Enum.Parse(typeof(T), data); 
        }
    }
}