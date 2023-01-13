using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using UnityEngine.UI;
public abstract class BaseUI : MonoBehaviour {
    
    private Vector3 _showPosition = new Vector3(0, 0, 0);
    private Vector3 _waitForLoadingPosition = new Vector3(0, 3000, 0);
    
    protected string _errorTitle = "에러 발생";
    
    protected delegate void ShowUIFunc();
    
    protected abstract void InitUI(ShowUIFunc showUIFunc);

    public void CloseUI() {
        Destroy(gameObject);
    }
    
    public void OpenUI() {
        StaticManager.UI.SetLoadingIcon(true);
        gameObject.transform.localPosition = _waitForLoadingPosition;
        InitUI(ShowUI);
    }
    
    private void ShowUI() {
        transform.localPosition = _showPosition;
        StaticManager.UI.SetLoadingIcon(false);

    }

    // ======================================================
    // 공통 에러처리 & 에러처리용 UI 
    // ======================================================
    protected void ShowAlertUI(string callback) {
        Debug.LogWarning(callback);
        StaticManager.UI.AlertUI.OpenWarningUI(_errorTitle, callback);
    }

    protected bool IsBackendError(BackendReturnObject bro) {
        if (bro.IsSuccess()) {
            Debug.Log(bro);
            return false;
        }
        else {
            Debug.LogWarning(bro);
            return true;
        }
    }
    
    
    public enum UIColor {
        BaseColor,
        NonActiveColor,
        SelectButtonColor,
        NonSelectedButtonColor
    }

    public Color GetUIColor(UIColor uiColor) {
        switch (uiColor) {
            case UIColor.BaseColor:
                return new Color32(255,236,144,255);
            case UIColor.NonActiveColor:
                return new Color32(140,140,140,255);
            case UIColor.SelectButtonColor:
                return new Color32(181,255,167,255);
            case UIColor.NonSelectedButtonColor:
                return new Color32(255,255,255,255);
            default:
                throw new ArgumentOutOfRangeException(nameof(uiColor), uiColor, null);
        }
    }
}