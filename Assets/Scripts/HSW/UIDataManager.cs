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
    // 상단 UI에 사용되는 유저 데이터 관리 클래스
    //===========================================================
    public class UIDataManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI Energy_count;
        [SerializeField] private TextMeshProUGUI timer_Text;
        [SerializeField] private TextMeshProUGUI Gold_count;

        private int maxEnergy = 50;
        public  int currentEnergy;
        private int restoreDuration = 600;

        public int  currentGold;
        
        private DateTime nextEnergyTime;
        private DateTime lastEnergyTime;
        private bool isRestoring = false;
    
        void Start()
        {
            UpdateEnergy();
            Load();
            StartCoroutine(RestoreEnergy());
        }

        #region 에너지 관련

        public void UseEnergy()
        {
            if (currentEnergy >= 1)
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
                Debug.Log("하트가 부족해요");
            }

        }
        
        private IEnumerator RestoreEnergy()
        {
            UpdateEnergyTimer();
            isRestoring = true;

            while (currentEnergy < maxEnergy)
            {
                DateTime currentDateTime = DateTime.Now;
                DateTime nextDateTime = nextEnergyTime;

                bool isEnergyCharing = false;

                while (currentDateTime > nextDateTime)
                {
                    if (currentEnergy < maxEnergy)
                    {
                        isEnergyCharing = true;
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

                if (isEnergyCharing == true)
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
        }

        private DateTime AddDuration(DateTime dateTime, int duration)
        {
            return dateTime.AddSeconds(duration);
        }
        
        private void UpdateEnergyTimer()
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

        private void UpdateEnergy()
        {
            Energy_count.text = String.Format(($"{currentEnergy}/{maxEnergy}"));
        }
        
        #endregion

        #region 골드 관련

        public void UseGold(int price)
        {
            if (currentGold >= price)
            {
                currentGold -= price;
                UpdateGold();
            }
            else
            {
                // 골드가 부족해요 팝업 띄우기
                Debug.Log("골드가 부족해요");
            }
        }

        public void GetGold(int amount)
        {
            currentGold += amount;
            UpdateGold();
            PlayerPrefs.SetFloat("CurrentScore", 0);
        }
        
        private void UpdateGold()
        {
            Gold_count.text = currentGold.ToString();
        }

        #endregion
        
        // 날짜값 서버에서 받아오는걸로 수정
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