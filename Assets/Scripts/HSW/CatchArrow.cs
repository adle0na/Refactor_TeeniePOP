using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatchArrow : MonoBehaviour
{
    public Image PowerBarMask;
    public float barChangeSpeed = 1;

    [SerializeField] private GameObject PowerBarGameObject;
    // 엑셀 데이터값 파싱할것
    [SerializeField] private float setTime = 100;
    [SerializeField] private TextMeshProUGUI countdownText;
    
    
    private float maxPowerBarValue = 100;
    private float currentPowerBarValue;
    private bool powerIsIncreasing;
    private bool CatchTimeOn;

    void Start()
    {
        countdownText.text = setTime.ToString();
        
        currentPowerBarValue = maxPowerBarValue;
        powerIsIncreasing = false;
        CatchTimeOn = true;
        StartCoroutine(UpdatePowerBar());
    }
    
    IEnumerator UpdatePowerBar()
    {
        while (CatchTimeOn)
        {
            if (!powerIsIncreasing)
            {
                currentPowerBarValue -= barChangeSpeed;
                if (currentPowerBarValue <= 0)
                    powerIsIncreasing = true;
            }

            if (powerIsIncreasing)
            {
                currentPowerBarValue += barChangeSpeed;
                if (currentPowerBarValue >= maxPowerBarValue)
                    powerIsIncreasing = false;
            }
            
            float fill = currentPowerBarValue / maxPowerBarValue;
            PowerBarMask.fillAmount = fill;
            
        }

        yield return null;
    }

    public void ShootArrow()
    {
        Debug.Log("화살 발사 확률 : " + currentPowerBarValue + "%");
        // 화살 발사 애니메이션 실행
    }
        
    public void InputTouch()
    {
        CatchTimeOn = false;
        ShootArrow();
        PowerBarGameObject.SetActive(false);
    }

    void Update()
    {
        setTime -= Time.deltaTime;
        countdownText.text = Mathf.Round(setTime).ToString();
    }
}
