using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BackEnd;

public class UserData
{
    public int HighStage = 0;
    public int Energy = 50;
    public int Gold = 0;
    public Dictionary<string, int> items = new Dictionary<string, int>();
    public List<string> collection = new List<string>();
}

public class BackendGameData
{
    private static BackendGameData _instance = null;

    public static BackendGameData Instance
    {
        get
        {
            if (_instance == null)
                _instance = new BackendGameData();

            return _instance;
        }
    }
    
    public static UserData userData;

    private string gameDataRowInDate = string.Empty;
    
    public void GameDataInsert()
    {
        // Step 1. 게임정보 삽입 구현하기
        if (userData == null) {
            userData = new UserData();
        }

        Debug.Log("데이터를 초기화합니다.");
        userData.HighStage = 1;
        userData.Energy = 5;
        userData.Gold = 0;

        userData.items.Add("Move3", 5);
        userData.items.Add("Pop", 3);
        userData.items.Add("Shake", 1);

        userData.collection.Add("하츄핑");

        Debug.Log("뒤끝 업데이트 목록에 해당 데이터들을 추가합니다.");
        Param param = new Param();
        param.Add("clearedStage", userData.HighStage);
        param.Add("Energy", userData.Energy);
        param.Add("Gold", userData.Gold);
        param.Add("items", userData.items);
        param.Add("Collection", userData.collection);


        Debug.Log("게임정보 데이터 삽입을 요청합니다.");
        var bro = Backend.GameData.Insert("USER_DATA", param);

        if (bro.IsSuccess()) {
            Debug.Log("게임정보 데이터 삽입에 성공했습니다. : " + bro);

            //삽입한 게임정보의 고유값입니다.
            gameDataRowInDate = bro.GetInDate();
        } else {
            Debug.LogError("게임정보 데이터 삽입에 실패했습니다. : " + bro);
        }
    }
    
    public void GameDataGet() {
        // Step 2. 게임정보 불러오기 구현하기
        Debug.Log("게임 정보 조회 함수를 호출합니다.");
        var bro = Backend.GameData.GetMyData("USER_DATA", new Where());
        if (bro.IsSuccess()) {
            Debug.Log("게임 정보 조회에 성공했습니다. : " + bro);


            LitJson.JsonData gameDataJson = bro.FlattenRows(); // Json으로 리턴된 데이터를 받아옵니다.

            // 받아온 데이터의 갯수가 0이라면 데이터가 존재하지 않는 것입니다.
            if (gameDataJson.Count <= 0) {
                Debug.LogWarning("데이터가 존재하지 않습니다.");
            } else {
                gameDataRowInDate = gameDataJson[0]["inDate"].ToString(); //불러온 게임정보의 고유값입니다.

                userData = new UserData();

                userData.HighStage = int.Parse(gameDataJson[0]["clearedStage"].ToString());
                userData.Energy        = int.Parse(gameDataJson[0]["heart"].ToString());
                userData.Gold         = int.Parse(gameDataJson[0]["coin"].ToString());

                foreach (string itemKey in gameDataJson[0]["items"].Keys) {
                    userData.items.Add(itemKey, int.Parse(gameDataJson[0]["items"][itemKey].ToString()));
                }

                foreach (LitJson.JsonData collection in gameDataJson[0]["Collection"]) {
                    userData.collection.Add(collection.ToString());
                }

                Debug.Log(userData.ToString());
            }
        } else {
            Debug.LogError("게임 정보 조회에 실패했습니다. : " + bro);
        }
    }
    
    public void LevelUp() {
        // Step 3. 게임정보 수정 구현하기
    }
    
    public void GameDataUpdate() {
        // Step 3. 게임정보 수정 구현하기
    }
    
}
