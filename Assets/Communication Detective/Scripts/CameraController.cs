using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speedH = 1f;
    public Vector2 startPos;
    public Vector2 direction;
    public bool directionChosen;

    public int LeftContraint;
    public int RightContraint;


    public const float turnSpeed = 1.0f;      // Speed of camera turning when mouse moves in along an axis
    public float panSpeed = 4.0f;       // Speed of the camera when being panned
    public float zoomSpeed = 4.0f;      // Speed of the camera going back and forth

    private Vector3 mouseOrigin;    // Position of cursor when mouse dragging starts
    private bool isRotating;    // Is the camera being rotated?

    //private void Move()
    //{
    //    transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    //}

    void Update()
    {

        if(Input.GetMouseButtonDown(0))
        {
            mouseOrigin = Input.mousePosition;
            isRotating = true;
        }

        if (!Input.GetMouseButton(0)) isRotating = false;

        if (isRotating)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
            float movement = pos.normalized.x * turnSpeed * -1;
            if (transform.rotation.eulerAngles.y > LeftContraint - movement && transform.rotation.eulerAngles.y < RightContraint - movement)
            {
                Debug.Log("Movement = " + movement);
                transform.RotateAround(transform.position, Vector3.up, movement);
            }
        }

        // Track a single touch as a direction control.
        if (Input.touchCount > 0 )
        {
            Touch touch = Input.GetTouch(0);

            // Handle finger movements based on touch phase.
            switch (touch.phase)
            {
                // Record initial touch position.
                case TouchPhase.Began:
                    Debug.Log("Touch Began");
                    startPos = touch.position;
                    directionChosen = false;
                    break;

                // Determine direction by comparing the current touch position with the initial one.
                case TouchPhase.Moved:
                    direction = touch.position - startPos;
                    break;

                // Report that a direction has been chosen when the finger is lifted.
                case TouchPhase.Ended:
                    Debug.Log("Touch Ended");
                    directionChosen = true;
                    break;
            }
        }
        if (directionChosen)
        {
            Debug.Log("Direction Chosed");
        }
    }
}
