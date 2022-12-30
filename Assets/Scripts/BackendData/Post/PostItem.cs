// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using BackEnd;
using InGameScene;
using LitJson;
using UnityEngine;

namespace BackendData.Post {
    
    // 우편에서 사용하는 차트
    // 새로운 차트로 우편을 보내고자 할때에는 그에 맞는 로직을 추가해야한다.
    public enum ChartType {
        forPost,
        weaponChart,
        itemChart
    }
    //===============================================================
    //  우편의 보상에 대한 클래스
    //===============================================================
    public class PostChartItem {
        public ChartType chartType { get; private set; }
        public int itemID { get; private set; }
        public float itemCount { get; private set; }
        public string itemName { get; private set; }

        private delegate void ReceiveFunc();

        private ReceiveFunc _receiveFunc = null;
        
        // PostItem 클래스의 보상을 담당하는 차트 정보를 파싱하는 클래스
        public PostChartItem(JsonData json) {
            itemCount = float.Parse(json["itemCount"].ToString());
            string chartName = json["chartName"].ToString();

            if (!Enum.TryParse<ChartType>(chartName, out var tempChartType)) {
                throw new Exception("지정되지 않은 Post ChartType 입니다.");
            }
            chartType = tempChartType;
            
            // 우편에 부탁된 아이템을 위한 차트를 타입에 맞게 변경
            try {
                switch (chartType) {
                    case ChartType.forPost:
                        itemID = int.Parse(json["item"]["ItemID"].ToString());
                        if (itemID == 1) {
                            itemName = "gold";
                            //Receive 함수 호출 시 해당 델리게이트 호출
                            _receiveFunc = () => { Managers.Game.UpdateUserData(itemCount, 0); };
                        } 
                        else if (itemID == 2) {
                            itemName = "jewel";
                            // Jewel은 아직 미구현
                        }
                        else if (itemID == 3) {
                            itemName = "exp";
                            //Receive 함수 호출 시 해당 델리게이트 호출
                            _receiveFunc = () => { Managers.Game.UpdateUserData(0, itemCount); };
                        }
                        break;
                    case ChartType.weaponChart:
                        itemID = int.Parse(json["item"]["WeaponID"].ToString());
                        itemName = StaticManager.Backend.Chart.Weapon.Dictionary[itemID].WeaponName;
                        //Receive 함수 호출 시 itemID의 무기를 얻도록 업데이트
                        _receiveFunc = () => { Managers.Game.UpdateWeaponInventory(itemID); };
                        break;
                    case ChartType.itemChart:
                        itemID = int.Parse(json["item"]["ItemID"].ToString());
                        itemName = StaticManager.Backend.Chart.Item.Dictionary[itemID].ItemName;
                        //Receive 함수 호출 시 itemID의 아이템을 얻도록 업데이트
                        _receiveFunc = () => { Managers.Game.UpdateItemInventory(itemID, (int)itemCount); };
                        break;
                }
            }
            catch (Exception e) {
                throw new Exception("PostChartItem : itemID를 파싱하지 못했습니다.\n" + e.ToString());
            }
        }

        public void Receive() {
            _receiveFunc.Invoke();
        }
    }
    
    //===============================================================
    //  Post.Manager 클래스의 GetPostList의 리턴값에 대한 파싱하는 클래스
    //===============================================================
    public class PostItem {
        public readonly string title;
        public readonly string content;
        public readonly DateTime expirationDate;
        public readonly string inDate;
        public readonly PostType PostType;
        
        // public readonly string author;
        // public readonly DateTime reservationDate;
        // public readonly DateTime sentDate;
        // public readonly string nickname;
        
        public readonly List<PostChartItem> items = new List<PostChartItem>();

        public PostItem(PostType postType, JsonData postListJson) {
            PostType = postType;

            content = postListJson["content"].ToString();
            expirationDate = DateTime.Parse(postListJson["expirationDate"].ToString());
            inDate = postListJson["inDate"].ToString();
            title = postListJson["title"].ToString();
            
            // sentDate = DateTime.Parse(postListJson["sentDate"].ToString());
            // reservationDate = DateTime.Parse(postListJson["reservationDate"].ToString());
            // nickname = postListJson["nickname"].ToString();
            // if (postListJson.ContainsKey("author")) {
            //     author = postListJson["author"].ToString();
            // }

            // 우편 보상 데이터
            if (postListJson["items"].Count > 0) {
                for (int itemNum = 0; itemNum < postListJson["items"].Count; itemNum++) {
                    PostChartItem item = new PostChartItem(postListJson["items"][itemNum]);
                    items.Add(item);
                }
            }
        }

        // 우편 보상을 받은 후 호출되는 델리게이트 함수
        public delegate void IsReceiveSuccessFunc(bool isSuccess);
        
        // [뒤끝] 우편 수령 함수
        public void ReceiveItem(IsReceiveSuccessFunc isReceiveSuccessFunc) {
            SendQueue.Enqueue(Backend.UPost.ReceivePostItem, PostType, inDate, callback => {
                bool isSuccess = false;
                try {
                    Debug.Log($"Backend.UPost.ReceivePostItem({PostType}, {inDate}) : {callback}");

                    // 수령할 경우
                    if (callback.IsSuccess()) {
                        isSuccess = true;
                        
                        string postItemString = String.Empty;
                        
                        // 해당 우편이 가지고 있는 item의 Receive함수를 호출하여 보상을 획득
                        foreach (var item in items) {
                            item.Receive();
                            postItemString += $"{item.itemName} x {item.itemCount}\n";
                        }

                        // 보상이 없는 경우에는 그냥 패스
                        if (string.IsNullOrEmpty(postItemString)) {
                            postItemString = "해당 우편은 아이템이 존재하지 않습니다.";
                            StaticManager.UI.AlertUI.OpenAlertUI("우편 수령 완료",postItemString);
                        }
                        else {
                            // 보상을 다 얻고 난 다음에는 저장
                            // 우편을 수령할 경우 우편이 제거되기 때문에 업데이트 주기에 게임을 종료하면 우편 수령에 대한 결과는 사라질 수 있다.
                            StaticManager.Backend.UpdateAllGameData(callback => {
                                if (callback.IsSuccess()) {
                                    StaticManager.UI.AlertUI.OpenAlertUI("우편 수령 완료","다음 아이템을 수령하였습니다\n" + postItemString);
                                }
                                else {
                                    StaticManager.UI.AlertUI.OpenErrorUI(GetType().Name,MethodBase.GetCurrentMethod()?.ToString(), callback.ToString());
                                }
                            });
                        }
                    }
                    else {
                        StaticManager.UI.AlertUI.OpenErrorUIWithText("우편 수령 실패 에러", "우편 수령에 실패했습니다.\n" + callback.ToString());
                    }
                }
                catch (Exception e) {
                    StaticManager.UI.AlertUI.OpenErrorUI(GetType().Name,MethodBase.GetCurrentMethod()?.ToString(), e.ToString());
                }
                finally {
                    if (isSuccess) {
                        //수령이 완료될 경우 우편 목록에서 제거
                        StaticManager.Backend.Post.RemovePost(inDate);
                    }
                    
                    isReceiveSuccessFunc(isSuccess);
                }
            });
        }
    }
}