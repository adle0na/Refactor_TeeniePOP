// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGameScene {
    //===============================================================
    // 플레이어 움직임 관련 클래스
    //===============================================================
    public class Player : MonoBehaviour {
        public enum MoveState {
            None,
            MoveToAttack,
            MoveToNextStage
        }
        
        private Vector3 _destination; // Move 시 도착지점
        private MoveState _moveState = MoveState.None;

        private float _moveSpeed = 10f; // 이동속도
        
        // 이동 후 동작
        public delegate void PlayerAfterMove();
        private PlayerAfterMove _playerAfterMove;
        
        private GameObject _bulletGameObject; // 총알 Prefab
        private WeaponObject[] _weaponArray; // 들고있는 총의 배열 

        public void Init(GameObject bulletPrefab) {
            _weaponArray = GetComponentsInChildren<WeaponObject>();
            _bulletGameObject = bulletPrefab;
            SetWeapon();
        }

        // 총의 방향을 지정할 새로운 적의 위치 지정
        public void SetNewEnemy(EnemyObject newEnemy) {
            foreach (var gun in _weaponArray) {
                if (gun.enabled) {
                    gun.SetEnemy(newEnemy);
                }
            }
        }

        //  
        void Update() {
            // MoveState가 None이 아닌 다른 상태라면 목적지로 이동
            if (_moveState != MoveState.None) {
                transform.localPosition =
                    Vector3.MoveTowards(transform.localPosition, _destination, _moveSpeed * Time.deltaTime);

                // 이동이 완료되었다면
                if (transform.localPosition.Equals(_destination)) {
                    if (_playerAfterMove != null) {
                        // 이동 후에 행동할 함수 호출
                        _playerAfterMove.Invoke();
                    }

                    // 오른쪽 맵 끝으로 갈 경우, 다시 왼쪽 맨끝으로 이동
                    if (_moveState == MoveState.MoveToNextStage) {
                        //다시 원래자리
                        transform.localPosition = new Vector3(-4, 1.7f, 0);
                    }
                    
                    // 이동이 완료될 경우 값 초기화
                    _moveState = MoveState.None;
                    _playerAfterMove = null;
                }
            }
        }

        // 플레이어 움직임 형태 지정
        public void SetMove(MoveState state, PlayerAfterMove playerAfterMove = null) {
            _moveState = state;

            switch (_moveState) {
                case MoveState.None: // 가만히 있을 경우
                    break;
                case MoveState.MoveToAttack: // 공격을 위해 이동할경우(왼쪽 맨끝 -> 중간)
                    _destination = new Vector3(-1.5f, 1.7f, 0);
                    break;
                case MoveState.MoveToNextStage: // 적을 처치하고 다음 스테이지로 넘어갈 경우(중간 -> 오른쪽 끝)
                    _destination = new Vector3(4f, 1.7f, 0);
                    break;
            }

            // 움직임이 변경되고 난 후에 호출할 함수 연결
            _playerAfterMove = playerAfterMove;
        }

        // 보여지는 무기 변경
        public void SetWeapon() {
            
            // 현재 장착중인 무기 데이터 불러오기
            var weaponInventoryDic = StaticManager.Backend.GameData.WeaponInventory.Dictionary;

            // 현재 장착중인 모든 무기 해제
            foreach (var weaponPos in _weaponArray) {
                weaponPos.ReleaseGun();
            }

            // 장착된 무기로 다시 무기 등록
            foreach (var weaponEquip in StaticManager.Backend.GameData.WeaponEquip.Dictionary) {
                string myWeaponId = weaponEquip.Key;
                int position = weaponEquip.Value;

                // 무기가 존재할 경우 해당 무기로 세팅
                if (weaponInventoryDic.ContainsKey(myWeaponId)) {
                    var weaponInventory = weaponInventoryDic[myWeaponId];
                    _weaponArray[weaponEquip.Value].ActiveGun(weaponInventory, _bulletGameObject);
                }
                else {
                    throw new Exception($"인벤토리에 존재하지 않습니다.\n {myWeaponId}");
                }
            }
        }
    }
}