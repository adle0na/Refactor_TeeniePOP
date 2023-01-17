// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using LitJson;
using Unity.VisualScripting;

namespace BackendData.Chart.Shop {
    //===============================================================
    // Shop 차트의 데이터를 관리하는 클래스
    //===============================================================
    public class Manager : Base.Chart {
        readonly List<Item> _list = new();

        public IReadOnlyList<Item> List => (IReadOnlyList<Item>)_list.AsReadOnlyList();
    
        // 차트 파일 이름 설정 함수
        // 차트 불러오기를 공통적으로 처리하는 BackendChartDataLoad() 함수에서 해당 함수를 통해 차트 파일 이름을 얻는다.
        public override string GetChartFileName() {
            return "shopChart";
        }
    
        protected override void LoadChartDataTemplate(JsonData json) {
            foreach (JsonData eachItemJson in json) {
                _list.Add(new Item(eachItemJson));
            }
        }
    }
}