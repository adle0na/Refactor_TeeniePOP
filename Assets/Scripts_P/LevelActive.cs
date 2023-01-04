using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelActive : MonoBehaviour
{
    public List<GameObject> _allLevels = new List<GameObject>();
    
    // 현재 레벨
    public int currentLevel;

    public GameObject popupBg;
    public GameObject targetRemind;

    private bool firstcheck = false;

    private LevelManager _levelManager;

    private void Awake()
    {
        targetRemind.SetActive(true);
        popupBg.SetActive(true);
        StartCoroutine(TargetAnim());
        
        _levelManager = _allLevels[currentLevel].GetComponent<LevelManager>();
    }

    #region 버튼관리

    public void RetryBtn()
    {
        SceneManager.LoadScene("Scenes_P/InGameScene");
    }

    public void NextStageBtn()
    {
        PlayerPrefs.SetInt("Selected Level", currentLevel + 1);
        PlayerPrefs.SetInt("CurrentLevel", currentLevel + 1);
        PlayerPrefs.SetInt("ClearCheck", 1);
        SceneManager.LoadScene("Scenes_P/LevelSelectMap");
    }

    public void Move3Item()
    {

    }

    public void PopItem()
    {

    }

    public void ShakeItem()
    {
        
    }
    
    #endregion

    IEnumerator TargetAnim()
    {
        currentLevel = PlayerPrefs.GetInt("Selected Level");
        yield return new WaitForSeconds(2f);
        
        Debug.Log("터치댐");
        targetRemind.SetActive(false);
        popupBg.SetActive(false);
        firstcheck = true;
        _allLevels[currentLevel].SetActive(true);
        
        yield break;
    }
}
