// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using UnityEngine;
using InGameScene.UI;

namespace InGameScene {
    //===========================================================
    // 적 생성, 스테이지 이동등의 게임 흐름을 제어하는 프로세스.
    //===========================================================
    public class ProcessManager {
        
        private Player _player;

        private UIManager _uiManager;
        private GameObject _enemyPrefab;
        private GameObject _dropItemPrefab;

        private int _currentStageNum = 0;

        private EnemyObject _enemyItem;
        
        public void Init(Player player, UIManager uiManager) {
            _player = player;
            _uiManager = uiManager;
            _currentStageNum = 0;
            SetStage();

            _enemyPrefab = Resources.Load<GameObject>("Prefabs/InGameScene/EnemyObject");
            _dropItemPrefab = Resources.Load<GameObject>("Prefabs/InGameScene/DropItemObject");

        }

        public void StartGame() {
            StartNextStage();
        }

        public EnemyObject GetEnemy() {
            return _enemyItem;
        }

        // 현재 내 게임 레벨과 스테이지 권장 레벨을 비교하여 현재 스테이지 정보를 
        private void SetStage() {
            for (int i = 0; i < StaticManager.Backend.Chart.Stage.List.Count; i++) {
                if (StaticManager.Backend.GameData.UserData.Level > StaticManager.Backend.Chart.Stage.List[i].Level) {
                    _currentStageNum = i;
                }
                else {
                    break;
                }
            }
        }

        //적을 처치하고 난 이후 다른 적을 생성하는 함수
        private void RespawnNextEnemy() {
            // 설정된 스테이지값을 넘는 하드코어 유저가 존재할 경우, 마지막 스테이지 정보로 계속 유지
            if (_currentStageNum > StaticManager.Backend.Chart.Stage.List.Count) {
                RespawnRandomEnemy();
                return;
            }

            // 설정된 스테이지값을 넘는 하드코어 유저가 존재할 경우, 마지막 스테이지 정보로 계속 유지
            if (StaticManager.Backend.GameData.UserData.Level > StaticManager.Backend.Chart.Stage.List[_currentStageNum].Level) {
                _currentStageNum++;
                GoNextStage();
            }
            else {
                RespawnRandomEnemy();
            }
        }

        //현재 스테이지의 EnemyInfoList에 있는 랜덤한 적을 불러와 생성하는 함수
        private void RespawnRandomEnemy() {
            int randomMin = 0;
            int randomMax = StaticManager.Backend.Chart.Stage.List[_currentStageNum].EnemyInfoList.Count;
            int randomValue = Random.Range(randomMin, randomMax);
            
            BackendData.Chart.Stage.Item.EnemyInfo enemyInfo = StaticManager.Backend.Chart.Stage.List[_currentStageNum].EnemyInfoList[randomValue];
            CreateEnemy(enemyInfo);
        }

        // 다음 스테이지로 가는 연출을 보여주는 함수 1.(플레이어가 오른쪽 화면 밖으로 가고 페이드 아웃되기까지)
        private void GoNextStage() {
            _player.SetMove(Player.MoveState.MoveToNextStage,
                () => {
                    StaticManager.UI.FadeUI.FadeStart(FadeUI.FadeType.ChangeToBlack,
                        () => StaticManager.UI.FadeUI.FadeStart(FadeUI.FadeType.ChangeToTransparent,
                            StartNextStage));
                });
        }

        // 다음 스테이지로 가는 연출을 보여주는 함수 2.(플레이어가 왼쪽 화면에서 원래 위치까지 가기)
        private void StartNextStage() {
            _player.SetMove(Player.MoveState.MoveToAttack);
            _uiManager.StageUI.ShowTitleStage(StaticManager.Backend.Chart.Stage.List[_currentStageNum].StageName);
            RespawnRandomEnemy();
        }

        // 적을 생성하는 함수
        // 생성된 적은 화면 밖에서 enemy객체의 생성 함수로 인해 화면 안으로 이동한다.
        private void CreateEnemy(BackendData.Chart.Stage.Item.EnemyInfo stageEnemyInfo) {
            Vector3 enemyRespawnPosition = new Vector3(4, 2.5f, 0);
            Vector3 enemyStayPosition = new Vector3(1, 2.5f, 0);

            // 적 차트정보에서 데이터 불러오기
            BackendData.Chart.Enemy.Item enemyInfo =
                StaticManager.Backend.Chart.Enemy.Dictionary[stageEnemyInfo.EnemyID];
            
            // 적 강화비율
            float multiStat = stageEnemyInfo.MultiStat;

            GameObject enemyObject = GameObject.Instantiate(_enemyPrefab);
            enemyObject.transform.localPosition = enemyRespawnPosition;
            enemyObject.transform.localScale = new Vector3(1, 1, 1);
            EnemyObject enemy = enemyObject.GetComponent<EnemyObject>();
            
            enemy.Init(enemyInfo, multiStat, enemyStayPosition);
            _enemyItem = enemy;
        }

        // 현재 적 상태를 업데이트 해주는 함수
        // 적 객체에서 해당 클래스로 호출한다.
        // 적이 죽을때마다 ui 및 데이터를 업데이트 해주는 기능을 담당하고 있다.
        public void UpdateEnemyStatus(EnemyObject enemyItem) {
            switch (enemyItem.CurrentEnemyState) {
                case EnemyObject.EnemyState.Init:
                    _player.SetNewEnemy(enemyItem);
                    _uiManager.EnemyUI.SetEnemyInfo(enemyItem.Name, enemyItem.MaxHp);
                    _uiManager.EnemyUI.ShowUI(true);
                    break;
                case EnemyObject.EnemyState.Normal:

                    _uiManager.EnemyUI.SetCurrentHp(enemyItem.Hp);
                    break;
                case EnemyObject.EnemyState.Dead:
                    Managers.Game.UpdateUserData(enemyItem.Money, enemyItem.Exp);
                    StaticManager.Backend.GameData.UserData.CountDefeatEnemy();
                    _uiManager.BottomUI.GetUI<InGameUI_Quest>().UpdateUI(BackendData.Chart.Quest.QuestType.DefeatEnemy);
                    _player.SetNewEnemy(null);
                    RespawnNextEnemy();
                    _uiManager.EnemyUI.ShowUI(false);
                    break;
            }
        }
        
        // 적 사망시 아이템을 드롭하는 함수.
        // 적 객체에서 확률을 측정하여 떨어트린다.
        public void DropItem(Vector3 enemyPosition, int itemID) {
            var dropItem = Object.Instantiate(_dropItemPrefab);
            dropItem.transform.position = enemyPosition;
            dropItem.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

            dropItem.GetComponent<SpriteRenderer>().sprite = 
                StaticManager.Backend.Chart.Item.Dictionary[itemID].ImageSprite;
        
            float randomX = Random.Range(50, 100);
            float randomY = Random.Range(100, 150);
            dropItem.GetComponent<Rigidbody2D>().AddForce(new Vector2(randomX %2 == 0 ? randomX : -randomX,randomY),ForceMode2D.Force);
            Object.Destroy(dropItem, 4f);
        }
    }
}