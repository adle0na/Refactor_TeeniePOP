using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class CatchModeManager : MonoBehaviour
{
    public Image CatchBarMask;
    public float barChangeSpeed = 1;

    [SerializeField] private GameObject PowerBarGameObject;
    // 엑셀 데이터값 파싱할것
    [SerializeField] private float setTime = 30;
    [SerializeField] private TextMeshProUGUI countdownText;
    
    
    private float CatchAbleValue = 60;
    private float currentCatchValue;
    private bool powerIsIncreasing;
    private bool CatchTimeOn;

    void Start()
    {
        countdownText.text = setTime.ToString();
        
        currentCatchValue = 0;
        powerIsIncreasing = false;
        CatchTimeOn = true;

        StartCoroutine(TimeCheck());
    }
    
    public void ShootArrow()
    {
        Debug.Log("화살 발사 확률 : " + ((currentCatchValue / CatchAbleValue) * 100) + "%");
        // 화살 발사 애니메이션 실행
    }
        
    public void InputTouch()
    {
        if (CatchTimeOn)
            currentCatchValue++;
    }

    void Update()
    {
        float fill = currentCatchValue / CatchAbleValue;
        CatchBarMask.fillAmount = fill;

        if (CatchTimeOn)
        {
            setTime -= Time.deltaTime;
            countdownText.text = Mathf.Round(setTime).ToString();
        }

    }

    IEnumerator TimeCheck()
    {
        if(setTime <= 0)
        {
            ShootArrow();
            PowerBarGameObject.SetActive(false);
            CatchTimeOn = false;
        }
        yield break;
    }
}
