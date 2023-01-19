using System;
using System.Collections;
using System.Collections.Generic;
using InGameScene;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUIController : MonoBehaviour
{
    [SerializeField] private GameObject levelSelectMap;
    [SerializeField] private GameObject InGame;

    public static StaticManager Instance { get; private set; }
    public static UIManager UI { get; private set; }
    
    void Awake()
    {
        if (FindObjectOfType(typeof(StaticManager)) == null)
        {
            var obj = Resources.Load<GameObject>("Prefabs/StaticManager");
            Instantiate(obj);
        }

        if (PlayerPrefs.GetInt("InGameState") == 0)
        {
            InGame.SetActive(false);
            levelSelectMap.SetActive(true);
        }
        else
        {
            InGame.SetActive(true);
            levelSelectMap.SetActive(false);
        }
        

    }

    public void goToSelect()
    {
        Debug.Log("맵선택 이동");
        PlayerPrefs.SetInt("InGameState", 0);
        StaticManager.Instance.ChangeScene("Scenes_P/InGameScene");
    }

    public void playGame()
    {
        Debug.Log("게임 시작");
        PlayerPrefs.SetInt("InGameState", 1);
        StaticManager.Instance.ChangeScene("Scenes_P/InGameScene");
    }
}
