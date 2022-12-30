// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BackendData.Chart.Quest;

namespace InGameScene.UI {
    //===========================================================
    // 퀘스트 아이템 UI
    //===========================================================
    public class InGameUI_QuestItem : MonoBehaviour {
        [SerializeField] private TMP_Text _questReqeatTypeText;
        [SerializeField] private TMP_Text _questContentText;
        [SerializeField] private TMP_Text _questRewardText;
        [SerializeField] private TMP_Text _questRequestText;
        [SerializeField] private TMP_Text _myRequestAchieveText;

        [SerializeField] private Button _requestAchieveButton;
        [SerializeField] private TMP_Text _isAchieveText;

        private Item _questItemInfo;

        // 퀘스트 타입 리턴하는 함수
        public QuestRepeatType GetRepeatType() {
            return _questItemInfo.QuestRepeatType;
        }

        // 퀘스트 보상 리스트
        private List<string> _rewardList = new ();

        public void Init(Item questItemInfo) {
            _questItemInfo = questItemInfo;

            switch (_questItemInfo.QuestRepeatType) {
                case QuestRepeatType.Day:
                    _questReqeatTypeText.text = "일일";
                    break;
                case QuestRepeatType.Week:
                    _questReqeatTypeText.text = "주간";
                    break;
                case QuestRepeatType.Month:
                    _questReqeatTypeText.text = "월간";
                    break;
                case QuestRepeatType.Once:
                    _questReqeatTypeText.text = "업적";
                    break;
            }

            _questContentText.text = _questItemInfo.QuestContent;

            // Exp, Money를 주는 RewardStat이 존재할 경우, 보상을 알려주는 text에 삽입
            if (_questItemInfo.RewardStat != null) {
                foreach (var item in _questItemInfo.RewardStat) {
                    // exp가 보상일 경우
                    if (item.Exp > 0) {
                        _rewardList.Add($"{item.Exp} Exp");
                    }
                    // money가 보상일 경우
                    if (item.Money > 0) {
                        _rewardList.Add($"{item.Money} Gold");
                    }
                }
            }
            
            // 아이템, 무기를 주는 RewardItem이 존재할 경우, 보상을 알려주는 text에 삽입
            if (_questItemInfo.RewardItem != null) {
                foreach (var item in _questItemInfo.RewardItem) {
                    switch (item.RewardItemType) {
                        case RewardItemType.Item: // 보상이 아이템일 경우 아이템 이름
                            _rewardList.Add(StaticManager.Backend.Chart.Item.Dictionary[item.Id].ItemName);
                            break;
                        case RewardItemType.Weapon:// 보상이 무기일 경우 무기 이름
                            _rewardList.Add(StaticManager.Backend.Chart.Weapon.Dictionary[item.Id].WeaponName);
                            break;
                    }
                }
            }

            
            // 보상을 담은 list 전부 한줄로 표현
            StringBuilder rewardString = new StringBuilder();
            for (int i = 0; i < _rewardList.Count; i++) {
                if (i > 0) {
                    rewardString.Append(" | ");
                }

                rewardString.Append(_rewardList[i]);
            }

            _questRewardText.text = rewardString.ToString();
            _questRequestText.text = _questItemInfo.RequestCount.ToString();
            _myRequestAchieveText.text = 0.ToString();
            
            //보상 달성 시 지급하는 Achieve 함수 연결
            _requestAchieveButton.onClick.AddListener(Achieve);
        }


        // 퀘스트 차트에 있는 도달 횟수가 넘었는지 확인.
        public void UpdateUI(float count) {
            bool isAchieve = StaticManager.Backend.GameData.QuestAchievement.Dictionary[_questItemInfo.QuestID]
                .IsAchieve;
            if (isAchieve) { // 이미 달성된 상태라면
                _isAchieveText.text = "완료";
                _requestAchieveButton.onClick.RemoveAllListeners();
                _requestAchieveButton.interactable = false;
                _requestAchieveButton.GetComponent<Image>().color = Color.gray;
            }
            else { // 달성이 되었다면
                if (_questItemInfo.RequestCount <= count) {
                    _isAchieveText.text = "달성";
                    _requestAchieveButton.interactable = true;
                    _requestAchieveButton.GetComponent<Image>().color = new Color32(255,236,144,255);
                }
                else { // 아직 count가 부족하다면
                    _isAchieveText.text = "미달성";
                    _requestAchieveButton.interactable = false;
                    _requestAchieveButton.GetComponent<Image>().color = Color.gray;
                }
            }
            
            // 현재 진행중인 횟수
            _myRequestAchieveText.text = count.ToString();
        }


        // 퀘스트 달성 버튼 클릭시 호출되는 함수
        public void Achieve() {
            // 게임 데이터에서 퀘스트 달성 여부를 저장
            StaticManager.Backend.GameData.QuestAchievement.SetAchieve(_questItemInfo.QuestID);

            // 보상 지급
            Reward();

            // 퀘스트 UI를 완료로 변경하고 버튼 변경
            _isAchieveText.text = "완료";
            _requestAchieveButton.onClick.RemoveAllListeners();
            _requestAchieveButton.interactable = false;
            _requestAchieveButton.GetComponent<Image>().color = Color.gray;
        }

        // 아이템이 있는지 확인
        public void CheckItem(RequestItemType requestItemType, int itemId) {
            
            if (_questItemInfo.RequestItem == null) {
                throw new Exception("RequestItem이 비어있습니다.");
            }
            
            // itemID를 공용으로 쓰고 있기 때문에 weapon과 item을 꼭 구별해야한다.
            // 아이템 종류가 다르면 패스
            if (requestItemType != _questItemInfo.RequestItem.RequestItemType) {
                return;
            }

            // 업데이트한 아이템이 존재하면 업데이트
            if (itemId == _questItemInfo.RequestItem.Id) {
                UpdateUI(1);
            }
        }

        // 각 보상 아이템 별로 보상을 지급하는 함수
        private void Reward() {
            if (_questItemInfo.RewardStat != null) 
            {
                // 차트 정보에  money, exp를 주는 rewardStat이 존재한다면
                foreach (var item in _questItemInfo.RewardStat) {
                    InGameScene.Managers.Game.UpdateUserData(item.Money, item.Exp);
                }
            }

            // 차트 정보에 아이템, 무기를 주는 RewardItem이 존재한다면
            if (_questItemInfo.RewardItem != null) {
                foreach (var item in _questItemInfo.RewardItem) {
                    switch (item.RewardItemType) {
                        case RewardItemType.Item: // 아이템일 경우 아이템의 id를 가져와 업데이트
                            InGameScene.Managers.Game.UpdateItemInventory(item.Id, (int)item.Count);

                            break;
                        case RewardItemType.Weapon: // 무기일 경우, 무기의 id를 가져와 업데이트
                            InGameScene.Managers.Game.UpdateWeaponInventory(item.Id);
                            break;
                    }
                }
            }

            // 받은 보상을 UI로 표현
            StringBuilder rewardString = new StringBuilder();
            rewardString.Append("다음 보상을 획득했습니다.\n");
            for (int i = 0; i < _rewardList.Count; i++) {
                if (i > 0) {
                    rewardString.Append("\n");
                }

                rewardString.Append(_rewardList[i]);
            }

            StaticManager.UI.AlertUI.OpenAlertUI("퀘스트 완료", rewardString.ToString());
        }
    }
}