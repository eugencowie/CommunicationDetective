using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

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

    public GameObject Text;

    public DatabaseController DatabaseController;

    [Range(1,6)]
    public int SlotNumber;

	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
		if (item == null)
        {
            string newName = DragHandler.itemBeingDragged.name;
            if (StaticInventory.Hints.Any(h => h.Name == newName))
            {
                ObjectHintData hint = StaticInventory.Hints.First(h => h.Name == newName);

                GameObject newObject = Instantiate(DragHandler.itemBeingDragged, DragHandler.itemBeingDragged.transform.parent);
                newObject.name = DragHandler.itemBeingDragged.name;
                newObject.transform.SetParent(transform);
                ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject, null, (x, y) => x.HasChanged());

                newObject.GetComponent<DragHandler>().enabled = false;

                Text.GetComponent<Text>().text = hint.Hint;

                DatabaseController.UploadItem(SlotNumber, hint);
            }
        }
	}
	#endregion
}