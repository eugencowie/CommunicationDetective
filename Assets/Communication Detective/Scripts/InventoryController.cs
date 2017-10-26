using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem
{
    public string Name;
    public List<string> Hints;

    public InventoryItem(string name, List<string> hints)
    {
        Name = name;
        Hints = hints;
    }
}

public class InventoryController : MonoBehaviour
{
    [SerializeField] private List<GameObject> ItemButtons = new List<GameObject>();

    private List<InventoryItem> m_items = new List<InventoryItem>();

    public void AddItems(string name, List<string> hints)
    {
        if (!m_items.Any(i => i.Name == name))
        {
            m_items.Add(new InventoryItem(name, hints));

            foreach (string hint in hints)
            {
                Debug.Log(name + ": " + hint);
            }

            UpdateButtons();
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
