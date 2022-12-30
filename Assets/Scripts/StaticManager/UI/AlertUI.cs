//Copyright 2013-2022 AFI,INC. All right reserved.

using System;
using System.Collections;
using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlertUI : MonoBehaviour {
    [Header("기본 에러 UI")]
    [SerializeField] private Image _alertTitleImage;
    [SerializeField] private TMP_Text _alertTitleText;
    [SerializeField] private TMP_Text _alertInfoText;
    [SerializeField] private Button _customButton;

    [Header("에러 세부 정보 관련 UI")] 
    [SerializeField] private GameObject _errorDetailGroup;
    [SerializeField] private TMP_Text _errorDetailText;
    [SerializeField] private Button _openDetailInfoButton;
    [SerializeField] private Button _closeDetailInfoButton;
    public delegate void ClickConfirmButton();

    private string _detailError;

    // 각 버튼에 기능을 할당하는 초기화 함수
    public void Init() {
        _openDetailInfoButton.onClick.AddListener(() => StartCoroutine(OpenDetailGroup()));
        _closeDetailInfoButton.onClick.AddListener(CloseDetailGroup);
        _errorDetailGroup.SetActive(false);

    }

    // 확인 시 알람 화면만 비활성화되는 '확인' 전용 UI
    public void OpenAlertUI(string title,string infoText) {
        _openDetailInfoButton.gameObject.SetActive(false);

        _alertTitleImage.color = new Color32(61,138,0,255);
        OpenUI(title, infoText, null);
    }
    
    // 안내 경고창 전용 UI
    public void OpenWarningUI(string titleText, string infoText) {
        OpenWarningUI(titleText, infoText, null);
    }
    public void OpenWarningUI(string titleText, string infoText, ClickConfirmButton clickConfirmButton) {
        _openDetailInfoButton.gameObject.SetActive(false);
        _alertTitleImage.color = new Color32(203,88,0,255);
        OpenUI(titleText, infoText, clickConfirmButton);
    }

    // '타이틀로' 버튼만 있는 경고 안내 UI 띄우는 함수
    public void OpenErrorUI(string className, string functionName, Exception e) {
        OpenErrorUI(className,functionName, e.ToString());
    }

    public void OpenErrorUI(string className, string functionName, string errorDetail) {
        StaticManager.Backend.StopUpdate();
        string error = $"{className} : {functionName}\n{errorDetail}";
        Debug.LogError(error);
        
        _alertTitleImage.color = new Color32(135,0,0,255);

        _openDetailInfoButton.gameObject.SetActive(true);
        _detailError = error;
        StaticManager.Backend.SendBugReport(className, functionName, errorDetail);
        
        OpenUI("에러 발생!", "오류가 발생하였습니다.\n타이틀로 돌아갑니다.", GoTitle);
    }
    
    public void OpenErrorUIWithText(string title, string content) {
        StaticManager.Backend.StopUpdate();
        _alertTitleImage.color = new Color32(135,0,0,255);
        
        OpenUI(title, content, GoTitle);
    }
    


    // 모든 안내 UI 띄우는 메인 함수
    private void OpenUI(string titleText, string infoText,
        ClickConfirmButton clickConfirmButton) {
        gameObject.SetActive(true);

        _alertTitleText.text = titleText;
        _alertInfoText.text = infoText;
        
        _customButton.onClick.RemoveAllListeners();
        _customButton.onClick.AddListener(() => {
            CloseUI();
            if (clickConfirmButton != null) {
                clickConfirmButton.Invoke();
            }
        });
    }

    public void SetYetLoginErrorText() {
        _alertTitleText.text += "(로그인 X)";
    }


    // 버튼 클릭 시, 에러 UI가 닫힌다.
    private void CloseUI() {
        _openDetailInfoButton.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    // 버튼 클릭 시, 페이드 기능과 함꼐 로그인화면으로 이동한다.
    private void GoTitle() {
        StaticManager.UI.SetLoadingIcon(false);
        StaticManager.Instance.ChangeScene("LoginScene");
    }

    // 에러 UI에 있는 (i) 버튼 클릭시, 에러 디테일 UI가 보여진다.
    IEnumerator OpenDetailGroup() {
        _errorDetailGroup.SetActive(true);
        _errorDetailText.text = _detailError;

        //text에서 길이를 측정하고 적용하기까지 한번의 Update가 호출되어야한다.
        yield return new WaitForFixedUpdate();

        float textHeight = _errorDetailText.rectTransform.rect.height;
        float textParentHeight = _errorDetailText.transform.parent.GetComponent<RectTransform>().rect.height;

        // 내용이 적을 경우, 가운데 정렬 / 내용이 많을 경우 위에서부터 4~5줄만 보이게 정렬
        if (textHeight > textParentHeight) {
            _errorDetailText.rectTransform.pivot = new Vector2(0.5f, 1);
            _errorDetailText.rectTransform.anchorMax = new Vector2(0.5f, 0.9f);
        }
        else {
            _errorDetailText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            _errorDetailText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        }

        //정렬한 후 수정되는 포지션 원위치
        _errorDetailText.rectTransform.anchoredPosition = new Vector2(0, 0);
    }


    // 에러 디테일 UI에 있는 확인 버튼 클릭 시, 에러 디테일 UI가 사라진다.
    private void CloseDetailGroup() {
        _errorDetailText.text = string.Empty;
        _errorDetailGroup.SetActive(false);
    }
}