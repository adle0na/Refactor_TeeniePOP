// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections;
using TMPro;
using UnityEngine;

namespace InGameScene.UI {
    //===========================================================
    // 스테이지를 넘어갈때마다 현재 스테이지를 알려주는 UI
    //===========================================================
    public class InGameUI_Stage : MonoBehaviour {
        [SerializeField] private GameObject _titleStageText;
        private float _titleStateTextMoveSpeed = 400.0f;

        // 현재 스테이지 보여주는 코루틴 함수 시작하는 함수
        public void ShowTitleStage(string stageName) {
            _titleStageText.GetComponentInChildren<TMP_Text>().text = $"{stageName}";
            StartCoroutine(UpdownTitleInGameUI_Stage());
        }

        // 현재 스테이지 UI를 아래로 내려가면서 보여주고 올라오는 코루틴
        IEnumerator UpdownTitleInGameUI_Stage() {
            _titleStageText.gameObject.SetActive(true);

            Vector3 upPosition = new Vector3(0, 0, 0);
            Vector3 downPosition = new Vector3(0, -400, 0);

            _titleStageText.transform.localPosition = upPosition;
            var tempPosition = _titleStageText.transform.localPosition;

            while (_titleStageText.transform.localPosition.y > downPosition.y) {
                tempPosition.y -= _titleStateTextMoveSpeed * Time.deltaTime;
                _titleStageText.transform.localPosition = tempPosition;

                yield return null;
            }

            yield return new WaitForSeconds(2.0f);

            while (_titleStageText.transform.localPosition.y < upPosition.y) {
                tempPosition.y += _titleStateTextMoveSpeed * Time.deltaTime;
                _titleStageText.transform.localPosition = tempPosition;

                yield return null;
            }

            _titleStageText.gameObject.SetActive(false);
        }
    }
}