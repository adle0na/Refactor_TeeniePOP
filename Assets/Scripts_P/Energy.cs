using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using BackEnd;
using GooglePlayGames.BasicApi;
using Mono.Cecil.Cil;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;

namespace InGameScene.UI
{
    //===========================================================
    // UI에 사용되는 아이템 클래스
    //===========================================================
    public class Energy : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI Energy_count;
        [SerializeField] private TextMeshProUGUI timer_Text;
        
        private int maxEnergy = 50;
        private int currentEnergy;
        private int restoreDuration = 600;
        private DateTime nextEnergyTime;
        private DateTime lastEnergyTime;
        private bool isRestoring = false;

        void Start()
        {
            currentEnergy = StaticManager.Backend.GameData.UserData.Energy;
            Debug.Log("currentEnergy : " + currentEnergy);
            UpdateHeart();
            Load();
            StartCoroutine(RestoreHeart());
        }

        public void UseEnergy()
        {
            if (currentEnergy >= 1)
            {
                currentEnergy -= 5;
                UpdateHeart();

                if (isRestoring == false)
                {
                    if (currentEnergy + 1 == maxEnergy)
                        nextEnergyTime = AddDuration(DateTime.Now, restoreDuration);

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

            while (currentEnergy < maxEnergy)
            {
                DateTime currentDateTime = DateTime.Now;
                DateTime nextDateTime = nextEnergyTime;

                bool isHeartAdding = false;

                while (currentDateTime > nextDateTime)
                {
                    if (currentEnergy < maxEnergy)
                    {
                        isHeartAdding = true;
                        currentEnergy++;
                        UpdateHeart();
                        DateTime timeToAdd = lastEnergyTime > nextDateTime ? lastEnergyTime : nextDateTime;
                        nextDateTime = AddDuration(timeToAdd, restoreDuration);
                    }
                    else
                    {
                        break;
                    }
                }

                if (isHeartAdding == true)
                {
                    lastEnergyTime = DateTime.Now;
                    nextEnergyTime = nextDateTime;
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
            if (currentEnergy >= maxEnergy)
            {
                timer_Text.gameObject.SetActive(false);
                timer_Text.text = "MAX";
                return;
            }

            TimeSpan time = nextEnergyTime - DateTime.Now;
            string timeValue = String.Format("{0:D2}분{1:D1}초", time.Minutes, time.Seconds);
            timer_Text.text = timeValue;
        }

        private void UpdateHeart()
        {
            Energy_count.text = String.Format(($"{currentEnergy}/{maxEnergy}"));
        }

        private DateTime StringToDate(string dateTime)
        {
            if (String.IsNullOrEmpty(dateTime)) return DateTime.Now;
            else return DateTime.Parse(dateTime);
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
}