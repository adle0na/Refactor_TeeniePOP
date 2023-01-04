using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using BackEnd;
using GooglePlayGames.BasicApi;
using Mono.Cecil.Cil;
using TMPro;

public class Heart : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI heart_count;
    [SerializeField] private TextMeshProUGUI timer_Text;

    private int maxHeart = 5;
    private int currentHeart;
    private int restoreDuration = 600;
    private DateTime nextHeartTime;
    private DateTime lastHeartTime;
    private bool isRestoring = false;

    void Start()
    {
        PlayerPrefs.SetInt("currentHeart", 5);
        UpdateHeart();
        Load();
        StartCoroutine(RestoreHeart());
    }
    
    public void UseHeart()
    {
        if (currentHeart >= 1)
        {
            currentHeart--;
            UpdateHeart();

            if (isRestoring == false)
            {
                if (currentHeart + 1 == maxHeart)
                    nextHeartTime = AddDuration(DateTime.Now, restoreDuration);

                StartCoroutine(RestoreHeart());
            }
        }
        else
        {
            // 하트가 부족해요 팝업 띄우기
            Debug.Log("하트가 부족해요");
        }

    }
    
    private IEnumerator RestoreHeart()
    {
        UpdateHeartTimer();
        isRestoring = true;
        
        while (currentHeart < maxHeart)
        {
            DateTime currentDateTime = DateTime.Now;
            DateTime nextDateTime    = nextHeartTime;
            
            bool isHeartAdding = false;

            while (currentDateTime > nextDateTime)
            {
                if (currentHeart < maxHeart)
                {
                    isHeartAdding = true;
                    currentHeart++;
                    UpdateHeart();
                    DateTime timeToAdd = lastHeartTime > nextDateTime ? lastHeartTime : nextDateTime;
                    nextDateTime = AddDuration(timeToAdd, restoreDuration);
                }
                else
                {
                    break;
                }
            }
            if (isHeartAdding == true)
            {
                lastHeartTime = DateTime.Now;
                nextHeartTime = nextDateTime;
            }
            UpdateHeartTimer();
            UpdateHeart();
            Save();
            yield return null;
        }

        isRestoring = false;
    }

    private DateTime AddDuration(DateTime dateTime, int duration)
    {
        return dateTime.AddSeconds(duration);
    }

    private void UpdateHeartTimer()
    {
        if (currentHeart >= maxHeart)
        {
            timer_Text.text = "MAX";
            return;
        }

        TimeSpan time    = nextHeartTime - DateTime.Now;
        string timeValue = String.Format("{0:D2}분{1:D1}초", time.Minutes, time.Seconds);
        timer_Text.text  = timeValue;
    }
    
    private void UpdateHeart()
    {
        heart_count.text = currentHeart.ToString();
    }
    private DateTime StringToDate(string dateTime)
    {
        if(String.IsNullOrEmpty(dateTime)) return DateTime.Now;
        else                               return DateTime.Parse(dateTime);
    }
    
    private void Load()
    {
        currentHeart  = BackendGameData.userData.heart;
        nextHeartTime = StringToDate(PlayerPrefs.GetString("nextHeartTime"));
        lastHeartTime = StringToDate(PlayerPrefs.GetString("lastHeartTime"));
    }

    private void Save()
    {
        PlayerPrefs.SetString("nextHeartTime", nextHeartTime.ToString());
        PlayerPrefs.SetString("lastHeartTime", lastHeartTime.ToString());
    }
}
