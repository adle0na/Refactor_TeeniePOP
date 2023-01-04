using System;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageController : MonoBehaviour
{

    [SerializeField] private List<GameObject> inGameCanvas;

    void Awake()
    {
        for (int i = 0; i < inGameCanvas.Count; i++)
        {
            if(i != PlayerPrefs.GetInt("InGameState"))
                inGameCanvas[i].SetActive(false);
            else if(i == PlayerPrefs.GetInt("InGameState"))
                inGameCanvas[i].SetActive(true);
        }
    }
}   
