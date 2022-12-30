// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections;
using UnityEngine;

using static InGameScene.Buff;

namespace InGameScene {
    //===============================================================
    // 버프를 관리하는 클래스
    //===============================================================
    public class BuffManager : MonoBehaviour{
        
        // 버프 배열
        private Buff[] _buffArray;
        private WaitForSeconds _buffCountSeconds = new WaitForSeconds(1);

        public void Init() {
            // 버프의 종류만큼 버프 배열 할당
            int buffTypeCount = Enum.GetValues(typeof(BuffStatType)).Length;
            _buffArray = new Buff[buffTypeCount];

            for (int i = 0; i < buffTypeCount; i++) {
                _buffArray[i] = new Buff();
            }
        }

        public bool StartBuff(BuffStatType buffStatType, float stat, float time,
            BuffAdditionType buffAdditionType) {
            
            // 버프 시작
            int buffCase = (int)buffStatType;

            float remainTime = _buffArray[buffCase].Time;

            if (remainTime > 0) {
                StaticManager.UI.AlertUI.OpenWarningUI("사용불가 안내", $"아직 사용중입니다.\n남은시간 : {remainTime}");
                return false;
            }

            _buffArray[buffCase].UpdateBuff(stat, time, buffAdditionType);
            StartCoroutine(StartBuffCoroutine(_buffArray[buffCase]));
            return true;
        }

        // 1초마다 반복하면서 시간을 계산하는 코루틴함수.
        IEnumerator StartBuffCoroutine(Buff buff) {
            while (buff.Time > 0) {
                yield return _buffCountSeconds;
                buff.Time -= 1;
            }

            buff.TurnOffBuff();
        }

        // 기본  스텟에서 버프된 스텟만큼 계산하여 리턴하는 함수.
        public float GetBuffedStat(BuffStatType buffStateType, float originalStat) {
            try {
                if (_buffArray[(int)buffStateType].IsBuffing == false) {
                    return originalStat;
                }
                
                // 버프 형태가 증감일 경우 그냥 더하기, 뺴기
                if (_buffArray[(int)buffStateType].BuffAddition == BuffAdditionType.Plus) {
                    return originalStat + _buffArray[(int)buffStateType].Stat;
                }
                else {
                    // 곱셈일 경우, 곱하기
                    return originalStat * _buffArray[(int)buffStateType].Stat;
                }
            }
            catch (Exception e) {
                throw new Exception($"GetBuffedStat {buffStateType} {originalStat} 에러\n" + e.ToString());
            }
        }
    }
}