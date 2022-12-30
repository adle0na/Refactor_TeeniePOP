// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace InGameScene.UI.PopupUI {
    //===========================================================
    // 랭킹 팝업 UI
    //===========================================================
    public class InGameUI_Rank : InGamePopupUI {
        [SerializeField] private GameObject _rankUUIDSelectButtonGroup;
        [SerializeField] private GameObject _rankItemParentGroup;
        
        [SerializeField] private GameObject _rankUIItemPrefab;
        [SerializeField] private InGameUI_RankItem _myInGameUI_RankItem;

        private int selectedUUIDNum = 0; // 선택한 랭킹의 UUID
        private Button[] selectedUUIDButtons; // 선택가능한 버튼의 배열


        private List<InGameUI_RankItem> _userRankingItemList = new List<InGameUI_RankItem>();

        //try catch는 RightButtonGroupManager에서 관리
        public override void Init() {
            // UI에 있는 랭킹 이름의 버튼 객체 가져오기
            GameObject button = _rankUUIDSelectButtonGroup.GetComponentInChildren<Button>().gameObject;

            // 랭킹 갯수만큼 버튼의 크기를 줄여 한줄로 보여줄 수 있도록 크기 조정
            _rankUUIDSelectButtonGroup.GetComponent<GridLayoutGroup>().cellSize =
                new Vector2(
                    _rankUUIDSelectButtonGroup.GetComponent<RectTransform>().sizeDelta.x /
                    StaticManager.Backend.Rank.List.Count, 100);

            // 랭킹 갯수만큼 버튼 배열 할당
            selectedUUIDButtons = new Button[StaticManager.Backend.Rank.List.Count];

            // 랭킹 버튼 하나씩 생성
            for (int i = 0; i < StaticManager.Backend.Rank.List.Count; i++) {
                GameObject obj;

                // 첫버튼은 이미 존재하므로 그냥 배정
                if (i == 0) {
                    obj = button;
                }
                else {
                    // 두번째 버튼부터는 새로 생성하여 배정
                    obj = Instantiate(button, _rankUUIDSelectButtonGroup.transform, true);
                }

                int index = i;

                // 랭킹 이름 수정
                obj.GetComponentInChildren<TMP_Text>().text = StaticManager.Backend.Rank.List[i].title;
                // 버튼 클릭 시 해당 랭킹의 uuid로 랭킹 정보를 불러오는 함수 버튼으로 설정
                obj.GetComponent<Button>().onClick.AddListener(() => ChangeButton(index));
                // 버튼 배정
                selectedUUIDButtons[index] = obj.GetComponent<Button>();
            }

            // 최대 10개까지만 랭킹을 보여줄거기 때문에 10개만 생성. 이후 랭킹에 10명 이하일 경우 비활성화.
            for (int i = 0; i < 10; i++) {
                var obj = Instantiate(_rankUIItemPrefab, _rankItemParentGroup.transform, true);
                _userRankingItemList.Add(obj.GetComponent<InGameUI_RankItem>());
                obj.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        public override void Open() {
            ChangeButton(0);
        }

        //보여줄 랭킹 uuid를 설정하는 함수
        private void ChangeButton(int selectNum) {
            selectedUUIDButtons[selectedUUIDNum].GetComponent<Image>().color = Color.white;
            selectedUUIDButtons[selectNum].GetComponent<Image>().color = Color.gray;

            selectedUUIDNum = selectNum;

            ChangeRankList(selectNum);
        }

        // uuid에 따라 랭킹을 불러오는 함수.
        // GetRankList 함수에서 n분 이하일 경우에는 서버에서 데이터를 불러오지 않고 현재 가지고 있는 데이터로 즉시 전달한다.
        private void ChangeRankList(int index) {
            StaticManager.UI.SetLoadingIcon(true);
            // n분이 지날경우 서버 불러온 후 callback 함수 실행, 지나지 않았을 경우 캐싱된 데이터 리턴
            StaticManager.Backend.Rank.List[index].GetRankList((isSuccess, list) => {
                if (isSuccess) {
                    // 리스트에 있는 유저수만큼 랭킹 아이템 활성화
                    for (int i = 0; i < list.Count; i++) {
                        _userRankingItemList[i].gameObject.SetActive(true);
                        _userRankingItemList[i].Init(list[i]);

                        // 많을 경우에는 취소
                        if (i > _userRankingItemList.Count) {
                            break;
                        }
                    }

                    // 지정한 10개보다 데이터가 적을 경우에는 남은 데이터를 안보이게 수정
                    for (int i = list.Count; i < _userRankingItemList.Count; i++) {
                        _userRankingItemList[i].gameObject.SetActive(false);
                    }

                    //정상적으로 처리가 된 이후에는 내 랭킹도 갱신
                    UpdateMyRank(index);
                }
                else {
                    StaticManager.UI.SetLoadingIcon(false);
                }
            });
        }

        // 내 랭킹 불러오기 함수
        // 만약 랭킹에 없을 경우, Update를 시도
        private void UpdateMyRank(int index) {
            StaticManager.Backend.Rank.List[index].GetMyRank((isSuccess, myRank) => {
                if (isSuccess) {
                    if (myRank != null) {
                        _myInGameUI_RankItem.Init(myRank);
                    }
                    else {
                        StaticManager.Backend.SendBugReport(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(),"myRank is null");
                    }
                }
                else {
                    Debug.Log("랭킹이 정상적으로 로드되지 않았습니다.");
                }

                StaticManager.UI.SetLoadingIcon(false);
            });
        }
    }
}