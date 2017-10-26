using System;
using UnityEngine;

public class ObjectZooming : MonoBehaviour
{
    public Action OnZoomEnded;

    private Vector2 m_touchStartPos;
    private Vector2 m_touchEndPos;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_touchStartPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            m_touchEndPos = Input.mousePosition;
            TouchEnded();
        }
    }

    private void TouchEnded()
    {
        Vector2 touchDistance = m_touchEndPos - m_touchStartPos;

        // If swipe has small distance it is probably a tap.
        if (touchDistance.magnitude < 20)
        {
            if (OnZoomEnded != null) OnZoomEnded();
        }
    }
}
