using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using InGameScene.UI;
using LitJson;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class BackendGameInfo : MonoBehaviour
{
    [SerializeField] private UIDataManager _uiDataManager;

    private void Awake()
    {
        DataLoad();
    }

    // 데이터값 삽입
    public void InsertData()
    {
        int Energy    = 50;
        int Gold      = 0;
        int HighLevel = 0;

        Param param = new Param();
        param.Add("Energy", Energy);
        param.Add("HighLevel", HighLevel);
        param.Add("Gold", Gold);

        Dictionary<string, int> Items = new Dictionary<string, int>()
        {
            { "Touch3", 5 },
            { "Pop", 3 },
            { "Shake", 1 }
        };
        
        param.Add("Items", Items);

        Dictionary<string, bool> Collections = new Dictionary<string, bool>()
        {
            { "0", true}
        };
        
        param.Add("Collections", Collections);

        BackendReturnObject BRO = Backend.GameData.Insert("CustomData", param);

        if (BRO.IsSuccess())
        {
            Debug.Log("indate : " + BRO.GetInDate());
        }
        else
        {
            switch (BRO.GetStatusCode())
            {
                case "404":
                    Debug.Log("존재하지 않는 tableName인 경우");
                    break;
                case "412":
                    Debug.Log("비활성화 된 tableName의 경우");
                    break;
                case "413":
                    Debug.Log("하나의 row가 400KB를 넘는 경우");
                    break;
                
                default:
                    Debug.Log("서버 공통에러 발생 " + BRO.GetMessage());
                    break;
            }
        }
    }

    #region 비활성화
   public void GetTableList()
   {
       BackendReturnObject BRO = Backend.GameData.GetTableList();
   
       if (BRO.IsSuccess())
       {
           JsonData publics = BRO.GetReturnValuetoJSON()["publicTables"];
           
           Debug.Log("public Tables");
           foreach (JsonData row in publics)
           {
               Debug.Log(row.ToString());
           }
       }
       else
       {
           Debug.Log("서버 공통 에러 발생: " + BRO.GetMessage());
       }
   }
   
   public void PublicContents()
   {
       BackendReturnObject BRO = Backend.GameInfo.GetPublicContents("CustomData", 1);
   
       if (BRO.IsSuccess())
       {
           GetGameInfo(BRO.GetReturnValuetoJSON());
       }
       else
       {
           CheckError(BRO);
       }
   }
   
   private string firstKey = string.Empty;
   
   public void PublicContentsNext()
   {
       BackendReturnObject BRO;
   
       if (firstKey == null)
       {
           BRO = Backend.GameInfo.GetPublicContents("CustomData", 1);
       }
       else
       {
           BRO = Backend.GameInfo.GetPublicContents("CustomData", firstKey, 1);
       }
   
       if (BRO.IsSuccess())
       {
           firstKey = BRO.FirstKeystring();
           GetGameInfo(BRO.GetReturnValuetoJSON());
       }
       else
       {
           CheckError(BRO);
       }
   }
   
   public void GetPrivateContents()
   {
       BackendReturnObject BRO = Backend.GameInfo.GetPrivateContents("CustomData");
   
       if (BRO.IsSuccess())
       {
           Debug.Log(BRO.GetReturnValue());
           GetGameInfo(BRO.GetReturnValuetoJSON());
       }
       else
       {
           CheckError(BRO);
       }
   }
   
   void GetGameInfo(JsonData returnData)
   {
       if (returnData != null)
       {
           Debug.Log("데이터가 존재합니다");
   
           if (returnData.Keys.Contains("rows"))
           {
               JsonData rows = returnData["rows"];
               for (int i = 0; i < rows.Count; i++)
               {
                   GetData(rows[i]);
               }
           }
           else if (returnData.Keys.Contains("row"))
           {
               JsonData row = returnData["row"];
               GetData(row[0]);
           }
       }
       else
       {
           Debug.Log("데이터가 없습니다");
       }
   }
   
   void GetData(JsonData data)
   {
       var Gamer_id = data["gamer_id"][0];
       var HighLevel = data["HighLevel"][0];
       var Energy = data["Energy"][0];
       var Gold = data["Gold"][0];
       var Collections = data["Collections"][0];
   
       Debug.Log("Energy " + Energy);
       Debug.Log("Gold" + Gold);
   
       if (data.Keys.Contains("HighLevel"))
       {
           Debug.Log("HighLevel : " + data["HighLevel"][0]);
       }
       else
       {
           Debug.Log("레벨 존재하지 않는 키");
       }
       
       if (data.Keys.Contains("Gold"))
       {
           Debug.Log("Gold : " + data["Gold"][0]);
       }
       else
       {
           Debug.Log("골드 존재하지 않는 키");
       }
       
       if (data.Keys.Contains("Energy"))
       {
           Debug.Log("Energy : " + data["Energy"][0]);
       }
       else
       {
           Debug.Log("에너지 존재하지 않는 키");
       }
       if (data.Keys.Contains("Collections"))
       {
           JsonData collectionData = data["Collections"][0];
           
           if(collectionData.Keys.Contains("0"))
           {
               Debug.Log("Collections: " + collectionData["0"][0]);
           }
           else
           {
               Debug.Log("콜렉션 존재하지 않는 키 입니다");
           }
       }
   }
   
   void CheckError(BackendReturnObject BRO)
   {
       switch (BRO.GetStatusCode())
       {
           case "200":
               Debug.Log("해당 유저의 데이터가 테이블에 없습니다");
               break;
       }
   }
    #endregion

    // 데이터 값 로드
    public void DataLoad()
    {
        Backend.GameData.GetMyData("CustomData", new Where(), 10, bro =>
        {
            if (bro.IsSuccess() == false)
            {
                // 요청 실패 처리
                Debug.Log(bro);
                return;
            }
            if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
            {
                // 요청이 성공해도 where 조건에 부합하는 데이터가 없을 수 있기 때문에
                // 데이터가 존재하는지 확인
                // 위와 같은 new Where() 조건의 경우 테이블에 row가 하나도 없으면 Count가 0 이하 일 수 있다.
                Debug.Log(bro);
                return;
            }
            _uiDataManager.currentEnergy = int.Parse(bro.FlattenRows()[0]["Energy"].ToString());
            Debug.Log("에너지값 동기화" + _uiDataManager.currentEnergy);
            _uiDataManager.currentGold   = int.Parse(bro.FlattenRows()[0]["Gold"].ToString());
            Debug.Log("골드값 동기화" + _uiDataManager.currentGold);
        });
        
    }
    
    // 데이터값 수정
    public void Backend_EnergyUpdate()
    {
        Param param = new Param();
        param.Add("Energy", _uiDataManager.currentEnergy);
        
        //Backend.GameData.UpdateV2( "CustomData", inDate, param, Backend.UserInDate, ( callback ) => 

    }

    public void Backend_GoldUpdate()
    {

    }
}
