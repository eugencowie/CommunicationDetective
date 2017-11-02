using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Camera)), RequireComponent(typeof(CameraSwipe))]
public class CameraTap : MonoBehaviour
{
    [SerializeField] private GameObject BlurPlane = null;
    [SerializeField] private GameObject InventoryController = null;
    [SerializeField] private GameObject HintPanel = null;
    [SerializeField] private GameObject HintText = null;
    [SerializeField] private GameObject Spotlight = null;

    private Inventory Inventory
    {
        get { return InventoryController.GetComponent<Inventory>(); }
    }

    private Vector2 m_touchStartPos;
    private Vector2 m_touchEndPos;
    private bool m_isTapping;

    private Camera m_camera;
    private CameraSwipe m_cameraSwipe;

    private void Start()
    {
        m_camera = GetComponent<Camera>();
        m_cameraSwipe = GetComponent<CameraSwipe>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            m_touchStartPos = Input.mousePosition;
            m_isTapping = true;
        }

        if (!Input.GetMouseButton(0) && m_isTapping)
        {
            m_isTapping = false;
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
            // Get the average of the touch start and end position.
            Vector2 tapPosition = m_touchStartPos + (touchDistance / 2);

            HandleTap(tapPosition);
        }
    }

    private void HandleTap(Vector2 tapPosition)
    {
        Ray ray = m_camera.ScreenPointToRay(tapPosition);

        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            TapObject(hit.collider.gameObject);
        }
    }

    private void TapObject(GameObject tappedObject)
    {
        if (isActiveAndEnabled)
        {
            // If hit object has hints, we can add them to the inventory
            ObjectHint[] hints = tappedObject.GetComponents<ObjectHint>();
            if (hints.Length > 0)
            {
                AddHints(tappedObject, hints);
            }

            Text text = HintText.GetComponent<Text>();
            text.text = "";
            foreach (var hint in hints)
            {
                text.text += string.Format("{0}: {1}\n", hint.Name, hint.Hint);
            }

            // If hit object has the inspectable component, we can inspect it
            ObjectInspectable inspectable = tappedObject.GetComponent<ObjectInspectable>();
            if (inspectable != null)
            {
                InspectObject(inspectable, hints);
            }

            // If hit object has the zoomable component, we can zoom in on it
            ObjectZoomable zoomable = tappedObject.GetComponent<ObjectZoomable>();
            if (zoomable != null)
            {
                ZoomToObject(zoomable, hints);
            }
        }
    }

    private void AddHints(GameObject tappedObject, ObjectHint[] hints)
    {
        Inventory.AddItems(() => TapObject(tappedObject), hints);
    }

    private void InspectObject(ObjectInspectable inspectable, ObjectHint[] hints)
    {
        if (inspectable != null)
        {
            GameObject newObject = Instantiate(inspectable.gameObject);

            newObject.transform.parent = m_camera.transform;
            newObject.transform.localPosition = new Vector3(0, 0, inspectable.InspectDistance);
            newObject.transform.localScale *= inspectable.InspectScale;

            newObject.AddComponent<ObjectInspecting>().OnInspectEnded = () => {
                HintPanel.SetActive(false);
                BlurPlane.SetActive(false);
                if (Spotlight != null) Spotlight.SetActive(false);
                enabled = m_cameraSwipe.enabled = true;
                Destroy(newObject);
            };

            if (hints.Length > 0) HintPanel.SetActive(true);
            if (Spotlight != null) Spotlight.SetActive(true);
            BlurPlane.SetActive(true);
            enabled = m_cameraSwipe.enabled = false;
        }
    }

    private void ZoomToObject(ObjectZoomable zoomable, ObjectHint[] hints)
    {
        if (zoomable != null)
        {
            // Create an inactive clone of this camera in its current location
            GameObject StartCamera = Instantiate(gameObject);
            StartCamera.SetActive(false);

            // Add a camera movement component
            CameraMovement movement = gameObject.AddComponent<CameraMovement>();

            // Set camera movement parameters
            movement.Target = zoomable.TargetCamera;
            movement.Duration = zoomable.Duration;

            // Set what happens when the camera movement ends
            movement.OnMoveEnded = () => CameraMovementEnded(zoomable, hints, StartCamera, movement);

            // Disable this component and disable the camera swipe component
            enabled = m_cameraSwipe.enabled = false;
        }
    }

    private void CameraMovementEnded(ObjectZoomable zoomable, ObjectHint[] hints, GameObject StartCamera, CameraMovement movement)
    {
        // Show hint panel if there are hints
        if (hints.Length > 0) HintPanel.SetActive(true);

        // Add a object zooming component to the target game object
        ObjectZooming zooming = zoomable.gameObject.AddComponent<ObjectZooming>();

        // Set what happens when the zoom is ended by the player
        zooming.OnZoomEnded = () => ObjectZoomEnded(StartCamera, movement, zooming);

        // Disable the movement component
        movement.enabled = false;
    }

    private void ObjectZoomEnded(GameObject StartCamera, CameraMovement movement, ObjectZooming zooming)
    {
        // Hide the hint panel
        HintPanel.SetActive(false);

        // Set the camera movement target to the original position
        movement.Target = StartCamera;

        // Set what happens when the camera movement ends
        movement.OnMoveEnded = () => {
            // Re-enable this component and the camera swipe component
            enabled = m_cameraSwipe.enabled = true;
            // Destroy the clone and the camera movement component
            Destroy(StartCamera);
            Destroy(movement);
        };

        // Re-enable and restart the camera movement controller to take us home
        movement.enabled = true;
        movement.Reset();

        // Our job here is done, delete the zooming component
        Destroy(zooming);
    }
}
