using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public GameObject Camera;

    private Vector2 m_touchStartPos;
    private Vector2 m_touchEndPos;

    private void Update()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    m_touchStartPos = touch.position;
                    break;

                case TouchPhase.Ended:
                    m_touchEndPos = touch.position;
                    OnTouchEnded();
                    break;
            }
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
            RaycastHit hit = new RaycastHit();
            Ray ray = Camera.GetComponent<Camera>().ScreenPointToRay(m_touchStartPos);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider == GetComponent<Collider>())
                {
                    // TODO: do something
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
