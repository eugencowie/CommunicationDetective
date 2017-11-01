using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DatabaseController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject[] PlayerButtons;
    [SerializeField] private GameObject[] PlayerClues;

    private OnlineManager NetworkController;
    private int m_scene;

    //public static GameObject itemBeingDragged;
    //Vector3 startPosition;
    //Transform startParent;

    private void Start()
    {
        NetworkController = new OnlineManager();

        NetworkController.GetPlayerScene(scene => {
            if (scene > 0) m_scene = scene;
            else SceneManager.LoadScene("Communication Detective/Scenes/Lobby");
        });

        for (int i = 0; i < PlayerButtons.Length; i++)
        {
            int tmp = i;
            PlayerButtons[i].GetComponent<Button>().onClick.AddListener(() => PlayerButtonPressed(tmp));
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //itemBeingDragged = gameObject;
        //startPosition = transform.position;
        //startParent = transform.parent;
        //GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        //transform.position = eventData.position;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        //itemBeingDragged = null;
        //GetComponent<CanvasGroup>().blocksRaycasts = true;
        //if (transform.parent == startParent)
        //{
        //    transform.position = startPosition;
        //}
    }

    public void ReturnButtonPressed()
    {
        SceneManager.LoadScene(m_scene);
    }

    private void PlayerButtonPressed(int buttonIndex)
    {
        foreach (var button in PlayerButtons)
        {
            button.SetActive(true);
        }
        PlayerButtons[buttonIndex].SetActive(false);

        foreach (var cluePanel in PlayerClues)
        {
            cluePanel.SetActive(false);
        }
        PlayerClues[buttonIndex].SetActive(true);
    }
}
