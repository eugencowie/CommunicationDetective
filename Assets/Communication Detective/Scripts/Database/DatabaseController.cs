using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DatabaseController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private OnlineManager NetworkController;
    private int m_scene;

    public static GameObject itemBeingDragged;
    Vector3 startPosition;
    Transform startParent;

    private void Start()
    {
        NetworkController = new OnlineManager();

        NetworkController.GetPlayerScene(scene => {
            if (scene > 0) m_scene = scene;
            else SceneManager.LoadScene("Communication Detective/Scenes/Lobby");
        });
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        itemBeingDragged = gameObject;
        startPosition = transform.position;
        startParent = transform.parent;
        //GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        itemBeingDragged = null;
        //GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (transform.parent == startParent)
        {
            transform.position = startPosition;
        }
    }

    public void ReturnButtonPressed()
    {
        SceneManager.LoadScene(m_scene);
    }

    public void Player1ButtonPressed()
    {

    }

    public void Player2ButtonPressed()
    {

    }

    public void Player3ButtonPressed()
    {

    }

    public void Player4ButtonPressed()
    {

    }
}
