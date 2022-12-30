// Copyright 2013-2022 AFI, INC. All rights reserved.

namespace InGameScene {
    //===============================================================
    // 버프 관련 클래스
    //===============================================================
    public class Buff {
        public enum BuffStatType {
            Atk,
            Delay,
            Gold,
            Exp
        }

        public enum BuffAdditionType {
            Plus,
            Multi
        }

        public bool IsBuffing { get; private set; } // 버프 상태
        public BuffAdditionType BuffAddition { get; private set; } // 버프가 더하기형인지, 곱셈형인지
        public float Stat{ get; private set; } // 버프로 증가하는 퍼센테이지
        public float Time{ get;  set; } // 남은 시간

        public void UpdateBuff(float stat, float time, BuffAdditionType buffAdditionType) {
            IsBuffing = true;
            Stat = stat;
            Time = time;
            BuffAddition = buffAdditionType;
        }

        public Buff() {
            UpdateBuff(0, 0, 0);
            IsBuffing = false;
        }

        public void TurnOffBuff() {
            Time = 0;
            IsBuffing = false;
        }
    }
}
