using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using BackEnd;
using GooglePlayGames.BasicApi;
using Mono.Cecil.Cil;
using TMPro;
using UnityEngine.Serialization;

public class Energy : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI energy_count;
    [SerializeField] private TextMeshProUGUI timer_Text;
    [SerializeField] private GameObject      timer;

    private int maxEnergy = 50;
    private int currentEnergy;
    private int restoreDuration = 1800;
    private DateTime nextEnergyTime;
    private DateTime lastEnergyTime;
    private bool isRestoring = false;

    void Start()
    {
        UpdateEnergy();
        Load();
        StartCoroutine(RestoreEnergy());
    }
    
    public void UseEnergy()
    {
        if (currentEnergy >= 5)
        {
            currentEnergy -= 5;
            UpdateEnergy();

            if (isRestoring == false)
            {
                if (currentEnergy + 1 == maxEnergy)
                    nextEnergyTime = AddDuration(DateTime.Now, restoreDuration);

                StartCoroutine(RestoreEnergy());
            }
        }
        else
        {
            // 하트가 부족해요 팝업 띄우기
            Debug.Log("에너지가 부족해요");
        }

    }
    
    private IEnumerator RestoreEnergy()
    {
        UpdateEnergyTimer();
        isRestoring = true;
        
        while (currentEnergy < maxEnergy)
        {
            DateTime currentDateTime = DateTime.Now;
            DateTime nextDateTime    = nextEnergyTime;
            
            bool isEnergyAdding = false;

            while (currentDateTime > nextDateTime)
            {
                if (currentEnergy < maxEnergy)
                {
                    timer.SetActive(true);
                    isEnergyAdding = true;
                    currentEnergy++;
                    UpdateEnergy();
                    DateTime timeToAdd = lastEnergyTime > nextDateTime ? lastEnergyTime : nextDateTime;
                    nextDateTime = AddDuration(timeToAdd, restoreDuration);
                }
                else
                {
                    break;
                }
            }
            if (isEnergyAdding == true)
            {
                lastEnergyTime = DateTime.Now;
                nextEnergyTime = nextDateTime;
            }
            UpdateEnergyTimer();
            UpdateEnergy();
            Save();
            yield return null;
        }

        isRestoring = false;
        timer.SetActive(false);
    }

    private DateTime AddDuration(DateTime dateTime, int duration)
    {
        return dateTime.AddSeconds(duration);
    }

    private void UpdateEnergyTimer()
    {
        TimeSpan time    = nextEnergyTime - DateTime.Now;
        string timeValue = String.Format("{0:D2}분{1:D1}초", time.Minutes, time.Seconds);
        timer_Text.text  = timeValue;
    }
    
    private void UpdateEnergy()
    {
        energy_count.text = String.Format("{0} / {1}", currentEnergy, maxEnergy);
        currentEnergy.ToString(); 
    }
    

    private DateTime StringToDate(string dateTime)
    {
        if(String.IsNullOrEmpty(dateTime)) return DateTime.Now;
        else                               return DateTime.Parse(dateTime);
    }
    
    private void Load()
    {
        nextEnergyTime = StringToDate(PlayerPrefs.GetString("nextHeartTime"));
        lastEnergyTime = StringToDate(PlayerPrefs.GetString("lastHeartTime"));
    }

    private void Save()
    {
        PlayerPrefs.SetString("nextHeartTime", nextEnergyTime.ToString());
        PlayerPrefs.SetString("lastHeartTime", lastEnergyTime.ToString());
    }
}
