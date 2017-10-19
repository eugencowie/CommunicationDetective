using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speedH = 0.10f;
    public Vector2 startPos;
    public Vector2 direction;
    public bool directionChosen;

    private Camera m_Camera;
    public float m_DampTime = 0.2f; 
    private Vector3 m_MoveVelocity;
    private Vector3 m_DesiredPosition;


    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }

    void Update()
    {

        if(Input.GetMouseButtonDown(0))
        {
            //startPos =  
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
