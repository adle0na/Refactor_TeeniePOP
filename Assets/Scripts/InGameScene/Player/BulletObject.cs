// Copyright 2013-2022 AFI, INC. All rights reserved.

using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace InGameScene {
    //===============================================================
    // 총알 발사 관련 클래스
    //===============================================================
    public class BulletObject : MonoBehaviour {
        private float _speed = 0;
        private float _atk = 0;

        // 방향과 스피드, 적에게 닿을 시 주는 데미지 지정(방향은 각도로)
        public void Shoot(Sprite bulletSprite, Quaternion destinationTransform, float speed, float atk) {
            gameObject.GetComponent<SpriteRenderer>().sprite = bulletSprite;
            gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            transform.rotation = destinationTransform;
            _speed = speed;
            _atk = atk;
        }

        // rotation을 돌린채로 앞으로 이동
        void Update() {
            transform.Translate(_speed * Vector3.up * Time.deltaTime);
        }

        // 내 데미지
        public float GetDamage() {
            return _atk;
        }

        // 총알이 맞을 경우 해당 총알 객체 파괴
        private void OnCollisionEnter2D(Collision2D col) {
            if (col.transform.CompareTag("BulletDestroyer")) {
                Destroy(this.gameObject);
            }

            if (col.transform.CompareTag("Enemy")) {
                Destroy(this.gameObject);
            }
        }
    }
}