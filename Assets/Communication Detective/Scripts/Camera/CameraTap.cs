using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera)), RequireComponent(typeof(CameraSwipe))]
public class CameraTap : MonoBehaviour
{
    [SerializeField] private GameObject BlurPlane = null;
    [SerializeField] private GameObject StartCamera = null;
    [SerializeField] private GameObject InventoryController = null;

    private InventoryController Inventory
    {
        get { return InventoryController.GetComponent<InventoryController>(); }
    }

    private Vector2 m_touchStartPos;
    private Vector2 m_touchEndPos;

    private Camera m_camera;
    private CameraSwipe m_cameraRotation;

    private void Start()
    {
        m_camera = GetComponent<Camera>();
        m_cameraRotation = GetComponent<CameraSwipe>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            m_touchStartPos = Input.mousePosition;
        }

        if (!Input.GetMouseButton(0))
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
            // If hit object has hints, we can add them to the inventory
            ObjectHint[] hints = hit.collider.gameObject.GetComponents<ObjectHint>();
            if (hints.Length > 0)
            {
                Inventory.AddItems(hints);
            }

            // If hit object has the inspectable component, we can inspect it
            ObjectInspectable inspectable = hit.collider.gameObject.GetComponent<ObjectInspectable>();
            if (inspectable != null)
            {
                GameObject newObject = Instantiate(inspectable.gameObject);

                newObject.transform.parent = m_camera.transform;
                newObject.transform.localPosition = new Vector3(0, 0, inspectable.InspectDistance);
                newObject.transform.localScale *= inspectable.InspectScale;

                newObject.AddComponent<ObjectInspecting>().OnInspectEnded = () => {
                    BlurPlane.SetActive(false);
                    enabled = m_cameraRotation.enabled = true;
                    Destroy(newObject);
                };

                BlurPlane.SetActive(true);
                enabled = m_cameraRotation.enabled = false;
            }

            // If hit object has the zoomable component, we can zoom in on it
            ObjectZoomable zoomable = hit.collider.gameObject.GetComponent<ObjectZoomable>();
            if (zoomable != null)
            {
                // Reset start camera direction
                StartCamera.transform.rotation = transform.rotation;

                CameraMovement movement = m_camera.gameObject.AddComponent<CameraMovement>();

                movement.Target = zoomable.TargetCamera;
                movement.MovementSpeed = zoomable.CameraMoveSpeed;
                movement.RotationSpeed = zoomable.CameraRotationSpeed;

                movement.OnMoveEnded = () => {
                    ObjectZooming zooming = zoomable.gameObject.AddComponent<ObjectZooming>();
                    zooming.OnZoomEnded = () => {
                        movement.Target = StartCamera;
                        movement.OnMoveEnded = () => {
                            enabled = m_cameraRotation.enabled = true;
                            Destroy(movement);
                        };
                        Destroy(zooming);
                    };
                };

                enabled = m_cameraRotation.enabled = false;
            }
        }
    }
}
