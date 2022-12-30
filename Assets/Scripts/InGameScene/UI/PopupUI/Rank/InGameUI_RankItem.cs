// Copyright 2013-2022 AFI, INC. All rights reserved.

using LitJson;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BackendData.Rank;


namespace InGameScene.UI.PopupUI {
    //===========================================================
    // 랭킹팝업 UI에 각 유저 리스트를 보여줄 아이템 클래스(보기전용)
    //===========================================================
    public class InGameUI_RankItem : MonoBehaviour {

        [SerializeField] private TMP_Text rankText;

        [SerializeField] private TMP_Text nickNameText;
        [SerializeField] private TMP_Text rankScoreText;
        
        public void Init(BackendData.Rank.RankUserItem rankUserItem) {
            rankText.text = rankUserItem.rank;
            nickNameText.text = rankUserItem.nickname;
            rankScoreText.text = "스코어 : " + rankUserItem.score;
        }
    }
}