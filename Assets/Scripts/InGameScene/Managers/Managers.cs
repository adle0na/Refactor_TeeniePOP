// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Reflection;
using BackEnd;
using Unity.VisualScripting;
using UnityEngine;
using InGameScene.UI;
using UnityEngine.SceneManagement;

namespace InGameScene {
    //===========================================================
    // 인게임씬의 모든 Manager 클래스를 제어하는 대표 클래스
    //===========================================================
    public class Managers : MonoBehaviour {

        
        [Header("UI")] 
        [SerializeField] private InGameUI_User _userUI;
        [SerializeField] private InGameUI_Enemy _enemyUI;
        [SerializeField] private InGameUI_Stage _stageUI;
        [SerializeField] private InGameUI_BottomUI _bottomUI;

        public static readonly ItemManager Item = new();
        public static readonly ProcessManager Process = new();
        public static readonly GameManager Game = new();
        private readonly UIManager _uiManager = new();

        public static BuffManager Buff;

        private Player _player;
        private CloudManager _cloudManager;
        private GameObject _bulletPrefab;

        private void Awake() {
            // 시작한 씬이 inGameScene일 경우에는 LoadScene으로 가서 데이터를 로딩
            if (Backend.IsInitialized == false) {
                SceneManager.LoadScene("LoginScene");
            }
        }

        // 각 매니저를 초기화
        void Start() {
            try {
                _cloudManager = FindObjectOfType<CloudManager>();
                _player = FindObjectOfType<Player>();

                _bulletPrefab = Resources.Load<GameObject>("Prefabs/InGameScene/BulletObject");
                
                _cloudManager.Init();
                _player.Init(_bulletPrefab);
                
                Item.Init(_bulletPrefab);
                
                _uiManager.Init(_userUI, _bottomUI, _enemyUI, _stageUI);
                Game.Init(_player, _uiManager);
                
                Process.Init(_player, _uiManager);
                
                var buffObject = new GameObject();
                buffObject.transform.SetParent(this.transform);
                Buff = buffObject.GetOrAddComponent<BuffManager>();
                Buff.Init();
                
                //페이드인
                StaticManager.UI.FadeUI.FadeStart(FadeUI.FadeType.ChangeToTransparent, Process.StartGame);
                // 코루틴을 통한 정기 데이터 업데이트 시작
                StaticManager.Backend.StartUpdate();

                // 우편의 갯수가 1개 이상이라면 우편 아이콘 표시
                if (StaticManager.Backend.Post.Dictionary.Count > 0) {
                    FindObjectOfType<RightButtonGroupManager>().SetPostIconAlert(true);
                }
                else {
                    FindObjectOfType<RightButtonGroupManager>().SetPostIconAlert(false);

                }
            }
            catch (Exception e) {
                StaticManager.UI.AlertUI.OpenErrorUI(GetType().Name,MethodBase.GetCurrentMethod()?.ToString(), e.ToString());
            }
        }


    }
}