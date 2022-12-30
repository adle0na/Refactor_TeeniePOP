// Copyright 2013-2022 AFI, INC. All rights reserved.

using LitJson;
using UnityEngine;

namespace BackendData.Chart.Weapon {
    //===============================================================
    // Weapon 차트의 각 row 데이터 클래스
    //===============================================================
    public class Item {
        public Item(JsonData json) {
            WeaponID = int.Parse(json["WeaponID"].ToString());
            WeaponName = json["WeaponName"].ToString();
            Atk = float.Parse(json["Atk"].ToString());
            GrowingAtk = float.Parse(json["GrowingAtk"].ToString());
            Delay = float.Parse(json["Delay"].ToString());
            GrowingDelay = float.Parse(json["GrowingDelay"].ToString());
            Spd = float.Parse(json["Spd"].ToString());
            GrowingSpd = float.Parse(json["GrowingSpd"].ToString());
            WeaponImageName = json["WeaponImageName"].ToString();
            BulletImageName = json["BulletImageName"].ToString();
            EffectImageName = json["EffectImageName"].ToString();
            MaxLevel = int.Parse(json["MaxLevel"].ToString());
            Price = long.Parse(json["Price"].ToString());
            UpgradePrice = long.Parse(json["UpgradePrice"].ToString());
        }

        public int WeaponID { get; private set; }
        public string WeaponName { get; private set; }
        public float Atk { get; private set; }
        public float GrowingAtk { get; private set; }
        public float Delay { get; private set; }
        public float GrowingDelay { get; private set; }
        public float Spd { get; private set; }
        public float GrowingSpd { get; private set; }
        public string WeaponImageName { get; private set; }
        public string BulletImageName { get; private set; }
        public string EffectImageName { get; private set; }
        public int MaxLevel { get; private set; }
        public long Price { get; private set; }
        public long UpgradePrice { get; private set; }

        public Sprite BulletSprite;
        public Sprite WeaponSprite;
        public Sprite EffectSprite;
    }
}