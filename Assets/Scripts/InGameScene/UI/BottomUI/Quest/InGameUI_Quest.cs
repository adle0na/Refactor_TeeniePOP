// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;
using BackendData.Chart.Quest;

namespace InGameScene.UI {
    //===========================================================
    // 퀘스트 UI
    //===========================================================
    public class InGameUI_Quest : InGameUI_BottomUIBase {
        [SerializeField] private GameObject _QuestParentObject;
        [SerializeField] private GameObject _questItemPrefab;

        // 퀘스트 아이템 리스트(key값은 questId)
        private Dictionary<int, InGameUI_QuestItem> _questItemDic = new();

        // 퀘스트 유형별 아이템 리스트(key값은 questType)
        private Dictionary<int, List<InGameUI_QuestItem>> _questItemDicByQuestType = new();
        
        public override void Init() {
            
            // 퀘스트 차트에 있는 모든 정보 불러와 생성
            foreach (var questItem in StaticManager.Backend.Chart.Quest.Dictionary) {
                var newQuestItem = Instantiate(_questItemPrefab, _QuestParentObject.transform, true);
                newQuestItem.transform.localPosition = new Vector3(0, 0, 0);
                newQuestItem.transform.localScale = new Vector3(1, 1, 1);

                var useItem = newQuestItem.GetComponent<InGameUI_QuestItem>();
                useItem.Init(questItem.Value);
                _questItemDic.Add(questItem.Value.QuestID, useItem);

                // 해당 퀘스트 타입
                int typeNum = (int)questItem.Value.QuestType;

                // 퀘스트 타입이 있으면 삽입, 없으면 새로 생성
                // 퀘스트 타입별로 List를 만들어 삽입
                if (!_questItemDicByQuestType.ContainsKey(typeNum)) {
                    _questItemDicByQuestType.Add(typeNum, new List<InGameUI_QuestItem>());
                }

                _questItemDicByQuestType[typeNum].Add(useItem);
            }

            // 퀘스트 타입별로 업데이트(골드 관련 퀘스트는 UserData, 무기 관련은 WeaponInventory등)
            for (int i = 0; i < Enum.GetValues(typeof(QuestType)).Length; i++) {
                UpdateUI((QuestType)i);
            }
        }

        // 각 퀘스트 타입별 업데이트하는 함수
        public void UpdateUI(QuestType questType) {
            switch (questType) {
                case QuestType.LevelUp: // 레벨업 관련 퀘스트일 경우에는 유저 레벨을 이용하여 업데이트
                    foreach (var list in _questItemDicByQuestType[(int)questType]) {
                        list.UpdateUI(StaticManager.Backend.GameData.UserData.Level);
                    }
                    break;
                case QuestType.UseGold: // 골드 사용 퀘스트일 경우에는 UsingGold 변수들을 이용하여 업데이트
                    foreach (var list in _questItemDicByQuestType[(int)questType]) {
                        if (list.GetRepeatType() == QuestRepeatType.Day) {
                            list.UpdateUI(StaticManager.Backend.GameData.UserData.DayUsingGold);
                        }
                        else if (list.GetRepeatType() == QuestRepeatType.Week) {
                            list.UpdateUI(StaticManager.Backend.GameData.UserData.WeekUsingGold);
                        }
                        else if (list.GetRepeatType() == QuestRepeatType.Month) {
                            list.UpdateUI(StaticManager.Backend.GameData.UserData.MonthUsingGold);
                        }
                        else {
                            throw new Exception("확인되지 않은 에러입니다.");
                        }
                    }

                    break;
                case QuestType.DefeatEnemy:// 적 처리 퀘스트는 DefeatEenmyCount를 계산
                    foreach (var list in _questItemDicByQuestType[(int)questType]) {
                        if (list.GetRepeatType() == QuestRepeatType.Day) {
                            list.UpdateUI(StaticManager.Backend.GameData.UserData.DayDefeatEnemyCount);
                        }
                        else if (list.GetRepeatType() == QuestRepeatType.Week) {
                            list.UpdateUI(StaticManager.Backend.GameData.UserData.WeekDefeatEnemyCount);
                        }
                        else if (list.GetRepeatType() == QuestRepeatType.Month) {
                            list.UpdateUI(StaticManager.Backend.GameData.UserData.MonthDefeatEnemyCount);
                        }
                        else {
                            throw new Exception("확인되지 않은 에러입니다.");
                        }
                    }

                    break;
                case QuestType.GetItem: // 아이테 관련 함수엘 경우에는 아이템이 존재하는지 확인
                    foreach (var list in _questItemDicByQuestType[(int)questType]) {
                        list.UpdateUI(0);
                    }

                    // UpdateUIForGetItem으로 대체
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(questType), questType, null);
            }
        }

        // 아이템, 무기의 경우 각 아이템에서 해당 함수 호출하여 아이템이 존재하는지 확인하고 달성 체크
        public void UpdateUIForGetItem(RequestItemType requestItemType, int itemId) {
            foreach (var list in _questItemDicByQuestType[(int)QuestType.GetItem]) {
                list.CheckItem(requestItemType, itemId);
            }
        }
    }
}