using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public int level;

    // 0 미실행, 1,클리어
    public int clearState;
    
    public static LevelSelector SInstance { get; set; }

    public Animator _SelectorAnim;

    public PopUpManager _popUpManager;
    
    // Start is called before the first frame update
    void Awake()
    {
        SInstance = this;
        _SelectorAnim = GetComponent<Animator>();
        
        _SelectorAnim.SetInteger("ClearState", clearState);
    }

    void Update()
    {
        _SelectorAnim.SetInteger("ClearState", clearState);
    }

    public void OpenGameinfoUI()
    {
        _popUpManager.level = level;
        if (clearState > 0)
        {
            PlayerPrefs.SetInt("Selected Level", level);
            _popUpManager.Target_info(level);
        }
        else
            return;
    }
}
