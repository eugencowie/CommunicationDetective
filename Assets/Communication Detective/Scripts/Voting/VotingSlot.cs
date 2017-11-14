using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class VotingSlot : MonoBehaviour, IDropHandler
{
    public GameObject item
    {
        get
        {
            if (transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;
            }
            return null;
        }
    }

    public bool CanDrop;

    //public GameObject Text;

    //public DatabaseController DatabaseController;

    //[Range(1, 6)]
    //public int SlotNumber;

    #region IDropHandler implementation
    public void OnDrop(PointerEventData eventData)
    {
        if (item == null && CanDrop)
        {
            var suspect = VotingDragHandler.itemBeingDragged.GetComponent<VotingDragHandler>().Suspect;

            suspect.No.gameObject.SetActive(true);

            /*string newName = DragHandler.itemBeingDragged.name;
            if (StaticInventory.Hints.Any(h => h.Name == newName))
            {
                ObjectHintData hint = StaticInventory.Hints.First(h => h.Name == newName);

                GameObject newObject = Instantiate(DragHandler.itemBeingDragged, DragHandler.itemBeingDragged.transform.parent);
                newObject.name = DragHandler.itemBeingDragged.name;
                newObject.transform.SetParent(transform);

                newObject.GetComponent<Button>().onClick.AddListener(() => {
                    Text.GetComponent<Text>().text = "";
                    DatabaseController.RemoveItem(SlotNumber);
                    Destroy(newObject);
                });

                newObject.GetComponent<DragHandler>().enabled = false;

                Text.GetComponent<Text>().text = hint.Hint;

                DatabaseController.UploadItem(SlotNumber, hint);
            }*/
        }
    }
    #endregion
}
