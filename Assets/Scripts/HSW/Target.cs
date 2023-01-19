using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Editable Value")]
    public int Target_ID;
    public static Target TInstance { get; set; }
    
    [HideInInspector]
    public  Animator     _TargetAnim;
    
    private void Awake()
    {
        TInstance   = this;
        
        _TargetAnim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _TargetAnim.SetInteger("ID", Target_ID);
    }
}
