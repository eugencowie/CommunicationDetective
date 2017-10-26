using System.Collections.Generic;
using UnityEngine;

public class ObjectZoomable : MonoBehaviour
{
    public GameObject TargetCamera;
    public float CameraMoveSpeed = 3f;
    public float CameraRotationSpeed = 0.0555f;
    public List<string> Hints = new List<string>();
}
