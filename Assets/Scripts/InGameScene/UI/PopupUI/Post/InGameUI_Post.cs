// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using BackEnd;
using UnityEngine;

namespace InGameScene.UI.PopupUI {
    public class InGameUI_Post : InGamePopupUI {
        [SerializeField] private GameObject _postItemParentGroup;
        [SerializeField] private GameObject _postItemPrefab;
        [SerializeField] private GameObject _noPostAlertText;

        Dictionary<string, GameObject> _postItemDictionary = new();

        public override void Init() {
        }

        public override void Open() {
            StaticManager.UI.SetLoadingIcon(true);

            // 우편함에 우편이 없을 경우 텍스트 출력
            if (StaticManager.Backend.Post.Dictionary.Count <= 0) {
                _noPostAlertText.gameObject.SetActive(true);
            }
            else {
                _noPostAlertText.gameObject.SetActive(false);
            }
            
            //우편 목록 조회 후 결과값을 이용하여 포스트 아이템 생성
            StaticManager.Backend.Post.GetPostList(PostType.Rank, (success, info) => {
                StaticManager.UI.SetLoadingIcon(false);

                if (success) {
                    foreach (var list in StaticManager.Backend.Post.Dictionary) {
                        // indate가 중복일 경우에는 패스
                        if (_postItemDictionary.ContainsKey(list.Value.inDate)) {
                            continue;
                        }
                        
                        // 아이템 생성, 아이템의 수령 버튼에 RemovePost 함수 설정
                        var obj = Instantiate(_postItemPrefab, _postItemParentGroup.transform, true);
                        obj.transform.localScale = new Vector3(1, 1, 1);
                        obj.GetComponent<InGameUI_PostItem>().Init(list.Value, RemovePost);
                        _postItemDictionary.Add(list.Value.inDate, obj);
                    }
                }
                else {
                    StaticManager.UI.AlertUI.OpenErrorUI(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(),
                        info);
                }
            });
        }
            
        // 우편 아이템의 수령 버튼 클릭시 호출
        // UI 리스트에서 해당 우편 제거
        private void RemovePost(string inDate) {
            if (_postItemDictionary.ContainsKey(inDate)) {
                Destroy(_postItemDictionary[inDate]);
                _postItemDictionary.Remove(inDate);
            }
        }
    }
}