using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelSelector : MonoBehaviour, IPointerDownHandler
{
    public int level;
    // 0 미실행, 1,클리어
    public int clearState;
    
    private Animator _SelectorAnim;

    [SerializeField] private PopUpManager PopUp;

    // Start is called before the first frame update
    void Awake()
    {
        _SelectorAnim = GetComponent<Animator>();
        _SelectorAnim.SetInteger("ClearState", clearState);
    }

    void Update()
    {
        _SelectorAnim.SetInteger("ClearState", clearState);
    }

    public void OnPointerDown(PointerEventData data)
    {
        PlayerPrefs.SetInt("SelectedLevel", level);
        PopUp.Target_info();
    }
}
