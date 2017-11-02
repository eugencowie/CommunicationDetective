using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static GameObject itemBeingDragged;
    Vector3 startPosition;
    Transform startParent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        itemBeingDragged = Instantiate(gameObject, gameObject.transform.parent);
        startPosition = itemBeingDragged.transform.position;
        startParent = itemBeingDragged.transform.parent;
        itemBeingDragged.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        itemBeingDragged.transform.position = eventData.position;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(itemBeingDragged);
        itemBeingDragged.GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (itemBeingDragged.transform.parent == startParent)
        {
            itemBeingDragged.transform.position = startPosition;
        }
        itemBeingDragged = null;
    }
}
