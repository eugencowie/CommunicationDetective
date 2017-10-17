using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speedH = 0.10f;

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        { 
}
            transform.Rotate(Vector3.up, speedH * Input.GetAxis("Mouse X"));

    }
}
