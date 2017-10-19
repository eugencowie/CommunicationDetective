using UnityEngine;

public class ObjectController : MonoBehaviour
{
    private Vector2 m_touchStartPos;
    private Vector2 m_touchEndPos;

    private void Update()
    {
        /*for (int i = 0; i < Input.touchCount; i++)
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
        }*/

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
            Ray ray = Camera.main.ScreenPointToRay(m_touchStartPos);

            Debug.Log("1");
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("2");
                if (hit.collider == GetComponent<Collider>())
                {
                    Debug.Log("3");
                    GameObject newObject = Instantiate(gameObject);
                    Quaternion oldRotation = newObject.transform.rotation;
                    newObject.transform.SetPositionAndRotation(Camera.main.transform.position, Camera.main.transform.rotation);
                    newObject.transform.Translate(new Vector3(0, 0, 1), Space.Self);
                    newObject.transform.rotation = oldRotation;
                    newObject.AddComponent<InspectController>().OnInspectEnded = () => {
                        foreach (var obj in FindObjectsOfType<ObjectController>())
                        {
                            obj.enabled = true;
                        }
                    };
                    newObject.GetComponent<ObjectController>().enabled = false;
                    Camera.main.GetComponent<CameraController>().enabled = false;
                    foreach (var obj in FindObjectsOfType<ObjectController>())
                    {
                        obj.enabled = false;
                    }
                }
            }
        }
    }
}
