using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [Range(0.2f, 2.0f)]
    public float InspectDistance = 0.25f;

    private Vector2 m_touchStartPos;
    private Vector2 m_touchEndPos;

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
                    GameObject newObject = Instantiate(gameObject);
                    Quaternion oldRotation = newObject.transform.rotation;
                    newObject.transform.SetPositionAndRotation(Camera.main.transform.position, Camera.main.transform.rotation);
                    newObject.transform.Translate(new Vector3(0, 0, InspectDistance), Space.Self);
                    newObject.transform.rotation = oldRotation;
                    newObject.AddComponent<InspectController>().OnInspectEnded = () => {
                        foreach (var obj in FindObjectsOfType<ObjectController>()) {
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
