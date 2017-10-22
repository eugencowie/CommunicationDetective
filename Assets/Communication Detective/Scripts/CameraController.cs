using UnityEngine;

public class CameraController : MonoBehaviour
{
    public int LeftConstraint = 20;
    public int RightConstraint = 255;

    private const float TurnSpeed = 100.0f;

    private Vector3 m_mouseOrigin;
    private bool m_isRotating;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_mouseOrigin = Input.mousePosition;
            m_isRotating = true;
        }

        if (!Input.GetMouseButton(0))
        {
            m_isRotating = false;
        }

        if (m_isRotating)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - m_mouseOrigin);
            float movement = pos.normalized.x * TurnSpeed * Time.deltaTime * -1;
            if (transform.rotation.eulerAngles.y > LeftConstraint - movement && transform.rotation.eulerAngles.y < RightConstraint - movement)
            {
                transform.RotateAround(transform.position, Vector3.up, movement);
            }
        }
    }
}
