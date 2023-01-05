using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LevelSort : MonoBehaviour
{
    private int stageNum;
    
    // 스테이지 씬에서 레벨 정렬
    public GameObject[] Levels;

    private LevelManager _levelManager;
    private void Awake()
    {
        Levels[0].GetComponent<LevelSelector>().clearState = 1;
        
        for (int i = 0; i < Levels.Length; i++)
        {
            Levels[i].GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();

            Levels[i].GetComponent<LevelSelector>().level = i + 1;
        }
        
        // 유저 정보 가져와서 수정할 것
        for (int i = 0; i <= PlayerPrefs.GetInt("CurrentLevel"); i++)
            Levels[i].GetComponent<LevelSelector>().clearState = 1;
    }
    
}
