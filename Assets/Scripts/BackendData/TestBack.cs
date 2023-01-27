using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using BackendData.Chart.LevelData;

public class TestBack : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI testtext;

    private Item _levelData;

    private void Start()
    {
        testtext.text = _levelData.level_Id.ToString();
        Debug.Log(_levelData.Play_drag);
        Debug.Log(_levelData.spawnPing_1);
    }
}
