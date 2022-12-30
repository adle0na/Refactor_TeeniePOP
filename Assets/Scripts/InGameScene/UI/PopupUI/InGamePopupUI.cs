// Copyright 2013-2022 AFI, INC. All rights reserved.

using UnityEngine;

namespace InGameScene.UI.PopupUI {
    //===========================================================
    // 팝업 UI의 베이스 클래스
    //===========================================================
    public abstract class  InGamePopupUI : MonoBehaviour {
        public abstract void Init();
        public abstract void Open();
    }
}