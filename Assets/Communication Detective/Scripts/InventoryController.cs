using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private List<GameObject> ItemButtons = new List<GameObject>();

    private List<ObjectHint> m_items = new List<ObjectHint>();

    public void AddItems(params ObjectHint[] items)
    {
        foreach (var item in items)
        {
            if (!m_items.Any(i => i.Name == item.Name))
            {
                Debug.Log(item.Name + ": " + item.Hint);
                m_items.Add(item);
                UpdateButtons();
            }
        }
    }

    private void UpdateButtons()
    {
        int numItems = Mathf.Min(m_items.Count, ItemButtons.Count);

        for (int i = 0; i < numItems; i++)
        {
            ItemButtons[i].GetComponent<Text>().text = m_items[i].Name;
        }
    }
}
