// Copyright 2013-2022 AFI, INC. All rights reserved.

namespace BackendData.GameData.QuestAchievement {
    //===============================================================
    // QuestAchievement 테이블의 Dictionary에 저장될 각 퀘스트 정보 클래스
    //===============================================================
    public class Item {
        public bool IsAchieve { get; private set; } // 달성 여부
        public int QuestId { get; private set; } // 퀘스트 아이디(차트와 연동)

        public Item(int questId, bool isAchieve) {
            IsAchieve = isAchieve;
            QuestId = questId;
        }

        // 해당 퀘스트 달성으로 변경
        public void SetAchieve() {
            IsAchieve = true;
        }
    }
}