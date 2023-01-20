using System;
using System.Collections;
using System.Collections.Generic;
using InGameScene.UI;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.SceneManagement;

public class PopUpManager : MonoBehaviour
{
    [SerializeField] private LevelData levelDB;
    [SerializeField] private UIDataManager _uiDataManager;
    
    [Header("User Valuse")]
    [SerializeField]
    private List<GameObject> topIcons;
    // 0: 하트, 1: 코인
    
    [SerializeField]
    private List<GameObject> PopUps;
    // 0: 클리어, 1: 실패, 2: 목표 3: 우편함, 4: 설정
    
    [SerializeField]
    private GameObject popupBackGround;

    [SerializeField]
    private List<TextMeshProUGUI> clear_text;
    // 0: 클리어 레벨, 1: 획득 코인, 2: 최종 점수

    public GameObject[] info_targets;
    
    public TextMeshProUGUI currentLevel_txt;

    // pref값으로 변경할것
    private int move3_count = 5;
    private int pop_count = 4;
    private int shake_count = 0;
    
    private LevelSelector _levelSelector;

    public int level;

    private void Awake()
    {
        // 획득한 골드량 데이터에 추가, 초기화 설정
        _uiDataManager.GetGold((int)(PlayerPrefs.GetFloat("CurrentScore") * 0.015f));
        clear_text[1].text = (PlayerPrefs.GetFloat("CurrentScore") * 0.015f).ToString("000");
        clear_text[2].text = PlayerPrefs.GetFloat("CurrentScore") + "점";
        if (PlayerPrefs.GetInt("ClearCheck") == 1)
        {
            clear_text[0].text = "레벨 " + (PlayerPrefs.GetInt("SelectedLevel") + 1) + " 성공!";
            popupBackGround.SetActive(true);
            PopUps[0].SetActive(true);
            PlayerPrefs.SetInt("ClearCheck", 0);
        }
        
        // 획득한 동전양 비례하여 이펙트 실행
    }

    #region 팝업 버튼

    public void Target_info()
    {
        level = PlayerPrefs.GetInt("SelectedLevel");
        popupBackGround.SetActive(true);
        PopUps[2].SetActive(true);
        string[] targets      = levelDB.Sheet1[level].targets.Split(", ");
        currentLevel_txt.text = "레벨" + (level + 1);

        for (int i = 0; i < targets.Length; i++)
        {
            int result;
            int.TryParse(targets[i], out result);

            info_targets[result].SetActive(true);
        }

    }

    public void PlayBtn()
    {
        PlayerPrefs.SetInt("InGameState", 1);
        SceneManager.LoadScene("Scenes/InGameScene_P");
        _uiDataManager.UseEnergy();
    }

    public void exit_Btn_SeletMap()
    {
        popupBackGround.SetActive(false);
        for (int i = 0; i < PopUps.Count; i++)
        {
            PopUps[i].SetActive(false);
        }

        for (int i = 0; i < info_targets.Length; i++)
        {
            info_targets[i].SetActive(false);
        }
    }

    public void NextLevel()
    {
        // 여기엔 캐릭터가 다음 스테이지로 이동하는 애니메이션 실행할것
    }
    

    #endregion

    
}
