// Copyright 2013-2022 AFI, INC. All rights reserved.

using UnityEngine;

namespace InGameScene {
    //===============================================================
    // 왼쪽으로 흘러가는 구름 관련 클래스
    //===============================================================
    public class CloudItem : MonoBehaviour
    {
        private const float _speed = 3.0f;
        private const float _destroyPositionX = -7f;
    
        // Update is called once per frame
        void Update()
        {
            transform.Translate(Vector3.left * _speed * Time.deltaTime);
            if (transform.localPosition.x < _destroyPositionX) {
                Destroy(this.gameObject);
            }
        }
    }
}

