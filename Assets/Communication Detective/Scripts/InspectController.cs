using System;
using UnityEngine;

public class InspectController : MonoBehaviour
{
    private Vector2 m_touchStartPos;
    private Vector2 m_touchEndPos;

    public const float turnSpeed = 120.0f;      // Speed of camera turning when mouse moves in along an axis

    private Vector3 mouseOrigin;    // Position of cursor when mouse dragging starts
    private bool isRotating;    // Is the camera being rotated?

    public Action OnInspectEnded;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseOrigin = Input.mousePosition;
            isRotating = true;
        }

        if (!Input.GetMouseButton(0)) isRotating = false;

        if (isRotating)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
            Vector3 movement = pos.normalized * turnSpeed * -1;
            transform.RotateAround(transform.position, Vector3.up, movement.x * Time.deltaTime);
            transform.RotateAround(transform.position, Vector3.forward, movement.y * Time.deltaTime);
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            m_touchStartPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_touchEndPos = Input.mousePosition;
            OnTouchEnded();
        }
    }

    private void OnTouchEnded()
    {
        Vector2 touchDistance = m_touchEndPos - m_touchStartPos;

        // If swipe has small distance it is probably a tap.
        if (touchDistance.magnitude < 5)
        {
            if (OnInspectEnded != null) OnInspectEnded();
            Camera.main.GetComponent<CameraController>().enabled = true;
            Destroy(gameObject);
        }
    }
}
