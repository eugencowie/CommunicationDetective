using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class StaticInventory
{
    public static List<ObjectHintData> Hints = new List<ObjectHintData>();
}

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject Button = null;

    private List<GameObject> m_buttons = new List<GameObject>();
    
    private const int m_spacing = 280;

    private void Start()
    {
        foreach (var button in m_buttons)
        {
            Destroy(button);
        }

        m_buttons.Clear();
        
        foreach (var item in StaticInventory.Hints)
        {
            AddItem(item);
        }
    }

    public void AddItem(ObjectHintData item)
    {
        if (!m_buttons.Any(b => b.name == item.Name))
        {
            // Add item to static inventory
            if (!StaticInventory.Hints.Any(h => h.Name == item.Name))
            {
                StaticInventory.Hints.Add(new ObjectHintData(item.Name, item.Hint));
            }

            // Create new button
            GameObject newButton = Instantiate(Button);
            newButton.name = item.Name;

            // Set initial transform
            newButton.transform.SetParent(transform);
            newButton.transform.localPosition = Button.transform.localPosition;
            newButton.transform.localRotation = Button.transform.localRotation;
            newButton.transform.localScale = Button.transform.localScale;

            // Set button position
            newButton.transform.localPosition -= new Vector3(0, m_spacing * m_buttons.Count, 0);

            // Set button text
            foreach (Transform t in newButton.transform)
            {
                if (t.gameObject.GetComponents<Text>().Length > 0)
                {
                    t.gameObject.GetComponent<Text>().text = item.Name;
                }
            }

            // Activate button
            newButton.SetActive(true);
            m_buttons.Add(newButton);
        }
    }

    public void AddItem(ObjectHint hint)
    {
        AddItem(new ObjectHintData(hint.Name, hint.Hint));
    }

    public void AddItems(ObjectHint[] items)
    {
        foreach (var item in items)
        {
            AddItem(item);
        }
    }
}
