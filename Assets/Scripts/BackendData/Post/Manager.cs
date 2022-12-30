// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using BackEnd;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

namespace BackendData.Post {
    //===============================================================
    //  우편 데이터를 관리하는 클래스
    //===============================================================
    public class Manager : Base.Normal {
        Dictionary<string, PostItem> _dictionary = new();
        public IReadOnlyDictionary<string, PostItem> Dictionary => (IReadOnlyDictionary<string, PostItem>)_dictionary.AsReadOnlyCollection();


        private DateTime _rankPostUpdateTime;

        // 로딩씬에서 호출되는 함수
        public override void BackendLoad(AfterBackendLoadFunc afterBackendLoadFunc) {
            string className = GetType().Name;
            string funcName = MethodBase.GetCurrentMethod()?.Name;
            
            // 관리자 우편 불러오기
            GetPostList(PostType.Admin, (isSuccess, errorInfo) => {
                afterBackendLoadFunc.Invoke(isSuccess, className, funcName, errorInfo);
            });
        }

        public void BackendLoadForRank(AfterBackendLoadFunc afterBackendLoadFunc) {
            _rankPostUpdateTime = DateTime.MinValue;
            
            string className = GetType().Name;
            string funcName = MethodBase.GetCurrentMethod()?.Name;
            
            // 관리자 우편 불러오기
            GetPostList(PostType.Rank, (isSuccess, errorInfo) => {
                afterBackendLoadFunc.Invoke(isSuccess, className, funcName, errorInfo);
            });
        }

        public delegate void AfterGetPostFunc(bool isSuccess, string errorInfo);

        // 우편 리스트 불러오는 함수
        public void GetPostList(PostType postType, AfterGetPostFunc afterPostLoadingFunc) {
            bool isSuccess = false;
            string errorInfo = string.Empty;

            // 랭킹 우편의 경우, UI를 열때마다 갱신.
            // UI 개봉시 10분이 지나지 않았을 경우에는 캐싱된 값을 리턴
            if (postType == PostType.Rank) {
                if ((DateTime.UtcNow - _rankPostUpdateTime).Minutes < 10) {
                    afterPostLoadingFunc(true, string.Empty);
                    return;
                }
            }

            //[뒤끝] 우편 목록 불러오기 함수
            SendQueue.Enqueue(Backend.UPost.GetPostList, postType, callback => {
                try {
                    Debug.Log($"Backend.UPost.GetPostList({postType}) : {callback}");

                    if (callback.IsSuccess() == false) {
                        throw new Exception(callback.ToString());
                    }
                    
                    // 랭킹 우편 시간 최근시간으로 갱신
                    _rankPostUpdateTime = DateTime.UtcNow;
                    
                    JsonData postListJson = callback.GetReturnValuetoJSON()["postList"];

                    for (int i = 0; i < postListJson.Count; i++) {

                        if (_dictionary.ContainsKey(postListJson[i]["inDate"].ToString())) {
                            //새로 불러온 우편에 아직 안받은 우편의 데이터가 있을 경우 패스
                        }
                        else {
                            // 새로운 아이템 등록
                            PostItem postItem = new PostItem(postType, postListJson[i]);
                            _dictionary.Add(postItem.inDate, postItem);
                        }
                    }
                    isSuccess = true;
                }
                catch (Exception e) {
                    errorInfo = e.ToString();
                }
                finally {
                    afterPostLoadingFunc.Invoke(isSuccess, errorInfo);
                }
            });
        }

        public void RemovePost(string inDate) {
            _dictionary.Remove(inDate);
        }
    }
}