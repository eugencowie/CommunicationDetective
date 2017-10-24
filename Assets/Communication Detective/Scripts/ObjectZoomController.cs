using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ObjectZoomController : MonoBehaviour
{
    public GameObject TargetCameraObject;
    
    private Camera TargetCamera
    {
        get { return TargetCameraObject.GetComponent<Camera>(); }
    }

    private Vector2 m_touchStartPos;
    private Vector2 m_touchEndPos;

    private enum LerpState { Initial, LerpingToward, AtObject, LerpingAway }
    private LerpState m_lerpState = LerpState.Initial;

    private Vector3 m_oldPosition;
    private Quaternion m_oldRotation;
    private Vector3 m_newPosition;
    private Quaternion m_newRotation;

    [SerializeField]
    private float translateSpeed = 1.5f;

    [SerializeField]
    private float rotationSpeed = 0.05f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_touchStartPos = Input.mousePosition;
        }

        if (!Input.GetMouseButton(0))
        {
            m_touchEndPos = Input.mousePosition;
            OnTouchEnded();
        }

        if (m_lerpState == LerpState.LerpingToward || m_lerpState == LerpState.LerpingAway)
        {
            Vector3 currentPosition = Camera.main.transform.position;
            Quaternion currentRotation = Camera.main.transform.rotation;

            Vector3 distance = m_newPosition - currentPosition;
            if (distance.magnitude > 0.1f)
            {
                Camera.main.transform.position += (distance.normalized * Time.deltaTime * translateSpeed);
            }

            Vector3 rotationDistanceAxis;
            float rotationDistanceAngle;

            Quaternion rotationDistance = Quaternion.Inverse(currentRotation) * m_newRotation;
            rotationDistance.ToAngleAxis(out rotationDistanceAngle, out rotationDistanceAxis);
            if (rotationDistanceAngle > 1)
            {
                Camera.main.transform.Rotate(rotationDistanceAxis, rotationDistanceAngle * rotationSpeed);
            }
            
            // if finished moving and rotating
            if (!(distance.magnitude > 0.1f) && !(rotationDistanceAngle > 1))
            {
                if (m_lerpState == LerpState.LerpingToward)
                {
                    m_lerpState = LerpState.AtObject;
                }
                else if (m_lerpState == LerpState.LerpingAway)
                {
                    Camera.main.GetComponent<CameraController>().enabled = true;
                    m_lerpState = LerpState.Initial;
                }
            }
        }
    }

    private void OnTouchEnded()
    {
        Vector2 touchDistance = m_touchEndPos - m_touchStartPos;

        // If swipe has small distance it is probably a tap.
        if (touchDistance.magnitude < 5)
        {
            RaycastHit hit = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay(m_touchStartPos);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider == GetComponent<Collider>())
                {
                    Camera.main.GetComponent<CameraController>().enabled = false;

                    if (m_lerpState == LerpState.Initial)
                    {
                        m_lerpState = LerpState.LerpingToward;
                        m_newPosition = TargetCamera.transform.position;
                        m_newRotation = TargetCamera.transform.rotation;
                        m_oldPosition = Camera.main.transform.position;
                        m_oldRotation = Camera.main.transform.rotation;
                    }
                    else if (m_lerpState == LerpState.AtObject)
                    {
                        m_lerpState = LerpState.LerpingAway;
                        m_newPosition = m_oldPosition;
                        m_newRotation = m_oldRotation;
                        m_oldPosition = Camera.main.transform.position;
                        m_oldRotation = Camera.main.transform.rotation;
                    }
                }
            }
        }
    }

}
