using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.SceneManagement;

public class PopUpManager : MonoBehaviour
{
    [Header("User Valuse")]
    [SerializeField]
    private List<GameObject> topIcons;
    // 0: 하트, 1: 코인, 2: 컵케이크
    
    [SerializeField]
    private List<GameObject> PopUps;
    // 0: 클리어, 1: 실패, 2: 목표, 3: 상점, 4: 우편함, 5: 설정
    
    [SerializeField]
    private GameObject popupBackGround;

    [SerializeField]
    private List<TextMeshProUGUI> clear_text;
    // 0: 클리어 레벨, 1: 획득 코인, 2: 최종 점수
    
    private Heart _heart;
    
    public GameObject[] target_panels;
    
    public TextMeshProUGUI currentLevel_txt;

    // pref값으로 변경할것
    private int move3_count = 5;
    private int pop_count = 4;
    private int shake_count = 0;
    
    private LevelSelector _levelSelector;

    public int level;

    private void Awake()
    {
        clear_text[1].text = (PlayerPrefs.GetFloat("CurrentScore") * 0.015f).ToString("000");
        clear_text[2].text = PlayerPrefs.GetFloat("CurrentScore").ToString() + "점";
        if (PlayerPrefs.GetInt("ClearCheck") == 1)
        {
            clear_text[0].text = "레벨 " + (PlayerPrefs.GetInt("Selected Level") + " 성공!");
            popupBackGround.SetActive(true);
            PopUps[0].SetActive(true);
            PlayerPrefs.SetInt("ClearCheck", 0);
        }
    }

    #region 팝업 버튼

    public void Target_info(int _level)
    {
        _level = level;
        
        popupBackGround.SetActive(true);
        PopUps[2].SetActive(true);
        currentLevel_txt.text = "레벨" + (level + 1);
        target_panels[_level].SetActive(true);
    }

    public void PlayBtn()
    {
        topIcons[0].GetComponent<Heart>().UseHeart();
        PlayerPrefs.SetInt("InGameState", 1);
        PlayerPrefs.SetInt("SelectedLevel",level);
        SceneManager.LoadScene("Scenes_P/InGameScene");
    }

    public void exitBtn(int num)
    {
        popupBackGround.SetActive(false);
        
        PopUps[num].SetActive(false);

        if (num == 2)
        {
            for (int i = 0; i < target_panels.Length; i++)
            {
                target_panels[i].SetActive(false);
            }
        }
    }

    #endregion

    
}
