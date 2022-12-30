// Copyright 2013-2022 AFI, INC. All rights reserved.

namespace BackendData.GameData.WeaponEquip {
    //===============================================================
    // WeaponEquip 테이블의 Dictionary에 저장될 각 무기 정보 클래스
    //===============================================================
    public class Item {
        public int Position { get; private set; } // 무기 장착 포지션 위치
        public string MyWeaponId { get; private set; } // 내 고유 무기 아이디

        public Item(int position, string myWeaponId) {
            Position = position;
            MyWeaponId = myWeaponId;
        }
    }
}