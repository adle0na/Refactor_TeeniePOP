using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.SceneManagement;

public class PopUpManager : MonoBehaviour
{
    public Heart _heart;
    public GameObject popupBackGround;
    public GameObject Clear;
    public TextMeshProUGUI clearLevel_txt;
    public TextMeshProUGUI clearCoin_txt;
    public TextMeshProUGUI clearScore_txt;
    
    public GameObject target_info;
    public GameObject[] target_panels;
    
    public TextMeshProUGUI currentLevel_txt;

    // pref값으로 변경할것
    public int move3_count = 10;
    public int pop_count = 4;
    public int shake_count = 0;
    
    private LevelSelector _levelSelector;

    public int level;

    private void Awake()
    {
        _heart.GetComponent<Heart>();
        
        clearCoin_txt.text = (PlayerPrefs.GetFloat("CurrentScore") * 0.015f).ToString("000");
        clearScore_txt.text = PlayerPrefs.GetFloat("CurrentScore").ToString() + "점";
        if (PlayerPrefs.GetInt("ClearCheck") == 1)
        {
            clearLevel_txt.text = "레벨 " + (PlayerPrefs.GetInt("Selected Level") + " 성공!");
            popupBackGround.SetActive(true);
            Clear.SetActive(true);
            PlayerPrefs.SetInt("ClearCheck", 0);
        }
    }

    public void Target_info(int _level)
    {
        _level = level;
        
        popupBackGround.SetActive(true);
        target_info.SetActive(true);
        currentLevel_txt.text = "레벨" + (level + 1);
        target_panels[_level].SetActive(true);
    }

    public void PlayBtn()
    {
        _heart.UseHeart();
        SceneManager.LoadScene("Scenes_P/InGameScene");
    }

    public void exitBtn(int _level)
    {
        _level = level;
        popupBackGround.SetActive(false);
        target_info.SetActive(false);
        target_panels[_level].SetActive(false);
        
        popupBackGround.SetActive(false);
        Clear.SetActive(false);
    }
    
}
