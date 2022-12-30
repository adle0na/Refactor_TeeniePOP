using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private Camera _cam;

    private Vector3 dragOrigin;

    private void Update()
    {
        PanCamera();
    }

    private void PanCamera()
    {
        if (Input.GetMouseButtonDown(0))
            dragOrigin = _cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - _cam.ScreenToWorldPoint(Input.mousePosition);
            
            //Debug
            print("origin " + dragOrigin + " newPosition " + _cam.ScreenToWorldPoint(Input.mousePosition));
            
            _cam.transform.position += difference;
        }
    }
}
