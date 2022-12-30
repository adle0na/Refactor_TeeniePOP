// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using BackendData.Chart.Enemy;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGameScene {
    //===============================================================
    // 게임에서 보여주는 적 관련 클래스
    //===============================================================
    public class EnemyObject : MonoBehaviour {
        private Rigidbody2D _rigidbody2D;
        private Vector3 _stayPosition;

        public string Name { get; private set; }
        public float MaxHp { get; private set; }

        public float Hp { get; private set; }
        public float Money { get; private set; }
        public float Exp { get; private set; }

        private const float _moveSpeed = 3f;

        // 적의 상태
        public enum EnemyState {
            Init,
            Normal,
            Dead
        }

        public EnemyState CurrentEnemyState { get; private set; }
        private BackendData.Chart.Enemy.Item _currentEnemyChartItem; // 적의 차트 정보

        void Update() {
            switch (CurrentEnemyState) {
                case EnemyState.Init:
                    InitUpdate();
                    break;
                case EnemyState.Normal:
                    NormalUpdate();
                    break;
                case EnemyState.Dead:
                    DeadUpdate();
                    break;
            }
        }
        //적 생성 이후 지정된 좌표로 올때까지 호출되는 함수.
        void InitUpdate() {
            transform.localPosition =
                Vector3.MoveTowards(transform.localPosition, _stayPosition, _moveSpeed * Time.deltaTime);

            if (transform.localPosition.Equals(_stayPosition)) {
                Debug.Log("적 초기화 완료");

                //Normal이 되기 전에 호출하여야합니다.
                Managers.Process.UpdateEnemyStatus(this);
                CurrentEnemyState = EnemyState.Normal;
            }
        }
        //지정된 좌표로 오고 죽을때까지 호출되는 함수.
        void NormalUpdate() {
            transform.localPosition =
                Vector3.MoveTowards(transform.localPosition, _stayPosition, _moveSpeed * Time.deltaTime);
        }
        
        //죽고나서 날라가는 함수
        void DeadUpdate() {
            transform.Rotate(Vector3.back * (100f * Time.deltaTime));
        }

        //적의 정보로 초기화하는 함수
        public void Init(BackendData.Chart.Enemy.Item enemyInfo, float multiStat, Vector3 stayPosition) {
            _currentEnemyChartItem = enemyInfo;

            Name = enemyInfo.EnemyName;
            MaxHp = enemyInfo.Hp * multiStat;
            Hp = MaxHp;
            Money = enemyInfo.Money * multiStat;
            Exp = enemyInfo.Exp * multiStat;

            gameObject.GetComponent<SpriteRenderer>().sprite = _currentEnemyChartItem.EnemySprite;

            _stayPosition = stayPosition;
            CurrentEnemyState = EnemyState.Init;
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D col) {
            if (CurrentEnemyState != EnemyState.Normal) {
                return;
            }

            // 만약 화면 밖 회색 박스와 부딪히면 소멸
            if (col.transform.CompareTag("BulletDestroyer")) {
                Dead();
                Destroy(gameObject);
            }

            // 총알에 맞을 경우
            if (col.transform.CompareTag("Bullet")) {
                float damage = col.gameObject.GetComponent<BulletObject>().GetDamage();
                Hp -= damage;

                // Hp가 0이 될 경우
                if (Hp <= 0) {
                    Dead();
                }

                // 맞을 때마다 현재 자신의 hp 정보를 업데이트
                Managers.Process.UpdateEnemyStatus(this);
            }
        }

        // 죽었을때 호출되는 함수
        private void Dead() {
            CurrentEnemyState = EnemyState.Dead;

            SetDropItem();

            // 오른쪽 위로 날라감
            _rigidbody2D.AddForce(new Vector2(200, 200), ForceMode2D.Force);
            gameObject.GetComponent<BoxCollider2D>().isTrigger = true;

            Destroy(gameObject, 5);
        }

        // 죽을 경우 일정 확률로 아이템을 떨어트리는 함수
        private void SetDropItem() {
            foreach (var dropItem in _currentEnemyChartItem.DropItemList) {
                double dropPercent = Math.Round((double)Random.Range(0, 100), 2);
                // 확률이 50%일 경우, 100중에 50미만만 나오면 된다
                // 확률이 10%일 경우, 100중에 1,2,3,4,5,6,7,8,9만 나와야한다
                // 확률이 1%일 경우, 100중에, 0.9 미만의 수만 나와야한다.
                if (dropItem.DropPercent > dropPercent) {
                    Managers.Game.UpdateItemInventory(dropItem.ItemID, 1);
                    Managers.Process.DropItem(transform.position, dropItem.ItemID);
                }
            }
        }
    }
}