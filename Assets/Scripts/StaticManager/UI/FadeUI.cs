//Copyright 2013-2022 AFI,INC. All right reserved.

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{

    private RectTransform _rectTransform;
    private Image         _fadeImage;
    public delegate void AfterFade(); // 페이드 기능 이용후에 호출되는 대리자 함수

    public void Init()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();
        _fadeImage = gameObject.GetComponent<Image>();

        _rectTransform.sizeDelta = new Vector2(1080, 2280);
    }

    public enum FadeType {
        ChangeToTransparent, // 투명으로 변경
        ChangeToBlack // 검정으로 편경
    }

    // 페이드 기능 시작
    public void FadeStart(FadeType fadeType, AfterFade afterFade,float second = 1) {
        this.gameObject.SetActive(true);
        StartCoroutine(UpdateAlphaColor(fadeType, afterFade,second));
    }

    // 즉시 페이드 색 변경
    public void SetFadeImmediately(FadeType fadeType) {
        this.gameObject.SetActive(true);

        Color color = _fadeImage.color;
        
        switch (fadeType) {
            case FadeType.ChangeToTransparent:
                color.a = 0;
                break;
            case FadeType.ChangeToBlack:
                color.a = 1;
                break;
        }

        _fadeImage.color = color;
    }

    // 페이드 기능으로 일정주기마다 불투명도 증감
    IEnumerator UpdateAlphaColor(FadeType fadeType, AfterFade afterFade, float second) {
        
        var routine = new WaitForSeconds(0.01f);

        float fadeSpeed = (10 / second) * 0.001f;
        Color color = _fadeImage.color;

        switch (fadeType) {
            case FadeType.ChangeToTransparent:
                fadeSpeed = -1 * fadeSpeed;
                color.a = 1;
                break;
            case FadeType.ChangeToBlack:
                color.a = 0;
                break;
        }
        
        do {
            color.a += fadeSpeed;
            _fadeImage.color = color;
            yield return routine;
        } while (0 < _fadeImage.color.a && _fadeImage.color.a < 1);
        this.gameObject.SetActive(false);

        if (afterFade != null) {
            afterFade.Invoke();
        }
        
    }
}
