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

	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
		if(!item)
        {
            GameObject newObject = Instantiate(DragHandler.itemBeingDragged, DragHandler.itemBeingDragged.transform.parent);
            newObject.name = DragHandler.itemBeingDragged.name;
            newObject.transform.SetParent (transform);
			ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject,null,(x,y) => x.HasChanged ());

            newObject.GetComponent<DragHandler>().enabled = false;

            Text.GetComponent<Text>().text = StaticInventory.Hints.First(h => h.Name == newObject.name).Hint;
        }
	}
	#endregion
}