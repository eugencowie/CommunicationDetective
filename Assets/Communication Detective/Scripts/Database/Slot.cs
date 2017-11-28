using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;


public static class StaticSlot
{
    // ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ ↙ ↙ ↙ 
    public static int MaxRemovals = 5; // ← ← 
    // ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑ ↖ ↖ ↖
    public static int TimesRemoved;
}

public class Slot : MonoBehaviour, IDropHandler {
	public GameObject item
    {
		get
        {
			if(transform.childCount>0){
				return transform.GetChild (0).gameObject;
			}
			return null;
		}
	}

    public bool CanDrop = false;

    private AudioSource m_audioSource;
    public AudioClip emailAudioClip;

    public GameObject Text;

    public DatabaseController DatabaseController;

    [Range(1,6)]
    public int SlotNumber;

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    #region IDropHandler implementation
    public void OnDrop (PointerEventData eventData)
	{
        if (item == null && CanDrop)
        {
            string newName = DragHandler.itemBeingDragged.name;
            if (StaticInventory.Hints.Any(h => h.Name == newName))
            {
                m_audioSource.PlayOneShot(emailAudioClip, 1f);
                ObjectHintData hint = StaticInventory.Hints.First(h => h.Name == newName);

                GameObject newObject = Instantiate(DragHandler.itemBeingDragged, DragHandler.itemBeingDragged.transform.parent);
                newObject.name = DragHandler.itemBeingDragged.name;
                newObject.transform.SetParent(transform);
                
                newObject.GetComponent<Image>().raycastTarget = true;

                newObject.GetComponent<Button>().onClick.AddListener(() => {
                    if (CanDrop && StaticSlot.TimesRemoved < StaticSlot.MaxRemovals) {
                        Text.GetComponent<Text>().text = "";
                        DatabaseController.RemoveItem(SlotNumber);
                        Destroy(newObject);
                        StaticSlot.TimesRemoved++;
                    }
                    else Debug.Log("YOU CANT GO THERE (EG. you have removed your maximum amount of times)");
                });

                newObject.GetComponent<DragHandler>().enabled = false;

                Text.GetComponent<Text>().text = hint.Hint;

                DatabaseController.UploadItem(SlotNumber, hint);
            }
        }
	}
	#endregion
}
