using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabasePageController : MonoBehaviour
{
    public GameObject PanelAbove;
    public GameObject PanelBelow;

    public void Up()
    {
        if (PanelAbove != null)
        {
            PanelAbove.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void Down()
    {
        if (PanelBelow != null)
        {
            PanelBelow.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
