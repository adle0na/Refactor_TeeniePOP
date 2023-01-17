using System;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageController : MonoBehaviour
{

    [SerializeField] private List<GameObject> inGameCanvas;

    [SerializeField] private List<GameObject> levelMap;

    void Awake()
    {
        // 0: SelectMap 1: InGame
        for (int i = 0; i < inGameCanvas.Count; i++)
        {
            switch (PlayerPrefs.GetInt("InGameState"))
            {
                case 0:
                    inGameCanvas[0].SetActive(true);

                    levelMap[0].SetActive(false);
                    inGameCanvas[1].SetActive(false);
                    break;
                case 1:
                    inGameCanvas[0].SetActive(false);

                    levelMap[0].SetActive(true);
                    inGameCanvas[1].SetActive(true);
                    break;
            }
        }
    }
}   
