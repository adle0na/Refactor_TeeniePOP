using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tping : MonoBehaviour
{
    public string ID;

    public GameObject SelectSprite;
    public bool IsSelect { get; private set; }

    public GameObject clearEffect;

    public Animator clearAnim;

    private bool effectCheck = false;
    
    private void Awake()
    {
        GetComponent<SpriteRenderer>();
        GetComponent<Animator>();
    }
    
    private void OnMouseDown()
    {
        LevelManager.Instance.PingDown(this);
    }

    private void OnMouseEnter()
    {
        LevelManager.Instance.PingEnter(this);
    }

    private void OnMouseUp()
    {
        LevelManager.Instance.PingUp();
    }

    public void SetIsSelect(bool isSelect)
    {
        IsSelect = isSelect;
        SelectSprite.SetActive(isSelect);
    }

}
