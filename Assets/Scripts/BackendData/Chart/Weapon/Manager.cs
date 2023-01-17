// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

namespace BackendData.Chart.Weapon {
    //===============================================================
    // Weapon 차트의 데이터를 관리하는 클래스
    //===============================================================
    public class Manager : Base.Chart {
        
        // 각 차트의 row 정보를 담는 Dictionary
        readonly Dictionary<int, Item> _dictionary = new ();
        
        // 다른 클래스에서 Add, Delete등 수정이 불가능하도록 읽기 전용 Dictionary
        public IReadOnlyDictionary<int, Item> Dictionary => (IReadOnlyDictionary<int, Item>)_dictionary.AsReadOnlyCollection();
        
        //이미지 캐싱 처리용 Dictionary
        readonly Dictionary<string, Sprite> _weaponImages = new ();
        readonly Dictionary<string, Sprite> _bulletImages = new ();
        readonly Dictionary<string, Sprite> _effectImages = new ();

        // 차트 파일 이름 설정 함수
        // 차트 불러오기를 공통적으로 처리하는 BackendChartDataLoad() 함수에서 해당 함수를 통해 차트 파일 이름을 얻는다.
        public override string GetChartFileName() {
            return "weaponChart";
        }

        // Backend.Chart.GetChartContents에서 각 차트 형태에 맞게 파싱하는 클래스
        // 차트 정보 불러오는 함수는 BackendData.Base.Chart의 BackendChartDataLoad를 참고해주세요
        protected override void LoadChartDataTemplate(JsonData json) {
            foreach (JsonData eachItem in json) {
                Item info = new Item(eachItem);

                _dictionary.Add(info.WeaponID, info);
                info.WeaponSprite = base.AddOrGetImageDictionary(_weaponImages, "Sprite/Weapon/", info.WeaponImageName);
                info.EffectSprite = base.AddOrGetImageDictionary(_effectImages, "Sprite/Effect/", info.EffectImageName);
                info.BulletSprite = base.AddOrGetImageDictionary(_bulletImages, "Sprite/Bullet/", info.BulletImageName);
            }
        }


    }
}