// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using BackEnd;
using UnityEngine;

namespace BackendData.Base {
    //===================================================================
    // 차트, 게임정보등 일괄적으로 동일한 로직을 수행하기 위해 만드는 베이스 클래스
    //==================================================================
    public class Normal {
        
        // LoadingScene에서 사용하는 불러오기가 완료된 이후에 호출되는 함수
        public delegate void AfterBackendLoadFunc(bool isSuccess, string className, string functionName, string errorInfo);

        // 기본적인 형식
        public virtual void BackendLoad(AfterBackendLoadFunc afterBackendLoadFunc) {
            string className = GetType().Name;
            string funcName = MethodBase.GetCurrentMethod()?.Name;
            afterBackendLoadFunc(true,className, funcName, String.Empty);
        }
    }
}
