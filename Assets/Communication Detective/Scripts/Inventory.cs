using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject Button = null;

    private List<GameObject> m_buttons = new List<GameObject>();
    
    private const int m_spacing = 280;

    public void AddItem(ObjectHint item)
    {
        if (!m_buttons.Any(b => b.name == item.Name))
        {
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

    public void AddItems(params ObjectHint[] items)
    {
        foreach (var item in items)
        {
            AddItem(item);
        }
    }
}
