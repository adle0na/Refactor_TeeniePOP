// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGameScene {
    //===============================================================
    // 뒷배경의 구름을 관리하는 클래스
    //===============================================================
    public class CloudManager : MonoBehaviour {
        [SerializeField] private GameObject _cloudObject;

        private float _createTime = 0f;
        private const float _createPosX = 7f;

        private float _currentTime = 0;

        private bool _isStart = false;

        public void Init() {
            _isStart = true;
        }

        // 일정 시간마다 구름을 생성하는 함수
        void Update() {
            if (!_isStart) {
                return;
            }

            _currentTime += Time.deltaTime;
            if (_currentTime > _createTime) {
                CreateCloud();

                _currentTime = 0;
                _createTime = Random.Range(1f, 3f);
            }
        }

        // 구름을 생성하는 함수
        void CreateCloud() {
            int randomCloudCount = Random.Range(1, 3);

            for (int i = 0; i < randomCloudCount; i++) {
                var cloud = Instantiate(_cloudObject, gameObject.transform, true);
                cloud.transform.localPosition = new Vector3(_createPosX + Random.Range(0f, 5f), Random.Range(0f, 5f), 0);

                float rndScale = Random.Range(0.5f, 0.7f);
                cloud.transform.localScale = new Vector3(rndScale, rndScale, 0);
            }
        }
    }
}