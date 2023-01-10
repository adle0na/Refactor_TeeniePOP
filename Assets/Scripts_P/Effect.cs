using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    [SerializeField]
    private GameObject _effect;
    
    System.Action _endCallBack = null;

    public void Play(string trigger, System.Action endCallBack = null)
    {
        GetComponent<Animator>().SetTrigger(trigger);
        _endCallBack = endCallBack;
    }

    public void OnEndEvent()
    {
        Destroy(this._effect);
    }
}
