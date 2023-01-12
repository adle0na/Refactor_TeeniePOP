using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Transactions;
using BackEnd;
using BackendData.Base;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BackendManager : MonoBehaviour
{
    //뒤끝 콘솔에 업로드한 차트 데이터만 모아놓은 클래스
    public class BackendChart {
        public readonly BackendData.Chart.AllChart ChartInfo = new(); // 모든 차트
        public readonly BackendData.Chart.Item.Manager Item = new(); // 아이템 차트
        public readonly BackendData.Chart.Shop.Manager Shop = new(); // 샵 차트
        public readonly BackendData.Chart.Quest.Manager Quest = new(); // 퀘스트 차트
    }
}
