using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    public GameObject Target;

    public float MovementSpeed = 3f;
    public float RotationSpeed = 0.05f;

    public Action OnMoveEnded;

    private Camera m_camera;

    private void Start()
    {
        m_camera = GetComponent<Camera>();
    }

    private void Update()
    {
        Vector3 currentPosition = m_camera.transform.position;
        Quaternion currentRotation = m_camera.transform.rotation;

        Vector3 distance = Target.transform.position - currentPosition;
        if (distance.magnitude > 0.1f)
        {
            m_camera.transform.position += (distance.normalized * Time.deltaTime * MovementSpeed);
        }

        Vector3 rotationDistanceAxis;
        float rotationDistanceAngle;

        Quaternion rotationDistance = Quaternion.Inverse(currentRotation) * Target.transform.rotation;
        rotationDistance.ToAngleAxis(out rotationDistanceAngle, out rotationDistanceAxis);
        if (rotationDistanceAngle > 1)
        {
            m_camera.transform.Rotate(rotationDistanceAxis, rotationDistanceAngle * RotationSpeed);
        }

        // if finished moving and rotating
        if (!(distance.magnitude > 0.1f) && !(rotationDistanceAngle > 1))
        {
            if (OnMoveEnded != null) OnMoveEnded();
        }
    }
}
