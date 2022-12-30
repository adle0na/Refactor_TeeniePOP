using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class BackendGameInfo : MonoBehaviour
{
    public void OnClickInsertData()
    {
        int charLevel = Random.Range(0, 99);
        int currentStage = Random.Range(0, 59);
        int heart = Random.Range(0, 5);
        long gold = Random.Range(0, 999999);
        long cupCake = Random.Range(0, 99999);
        
        // 서버와 통신할때 넘겨주는 파라미터 클래스
        Param param = new Param();
        param.Add("lv", charLevel);
        param.Add("Stage", currentStage);
        param.Add("Heart", heart);
        param.Add("Gold", gold);
        param.Add("CupCake", cupCake);

        BackendReturnObject BRO = Backend.GameData.Insert("custom", param);
    }
}
