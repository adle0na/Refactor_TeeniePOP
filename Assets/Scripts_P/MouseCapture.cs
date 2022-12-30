using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseCapture : MonoBehaviour
{
    public GameObject startPrefab;

    private float spawnsTime;

    public float defaultTime = 0.05f;
    
    void Update()
    {
        if (Input.GetMouseButton(0) && spawnsTime >= defaultTime)
        {
            StarCreat();
            spawnsTime = 0;
        }
        spawnsTime += Time.deltaTime;
    }

    void StarCreat()
    {
        Vector3 mPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mPosition.z = 0;
        Instantiate(startPrefab, mPosition, quaternion.identity);
    }
    
    public void OpenLevelSelectScene()
    {
        SceneManager.LoadScene(1);
    }
}
