﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject ButtonContainer = null;
    [SerializeField] private GameObject Button = null;

    private List<GameObject> m_buttons = new List<GameObject>();
    private const int m_spacing = 225;
    private const float m_scrollSpeed = 2000;
    private float m_scrollAmount = 0;

    private Vector3 m_touchOrigin;
    private bool m_isSwiping;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject())
        {
            m_touchOrigin = Input.mousePosition;
            m_isSwiping = true;
        }

        if (!Input.GetMouseButton(0))
        {
            m_isSwiping = false;
        }

        if (m_isSwiping)
        {
            Vector3 screenPos = Camera.main.ScreenToViewportPoint(Input.mousePosition - m_touchOrigin);

            float movement = screenPos.normalized.y * m_scrollSpeed * Time.deltaTime;
            
            if (Mathf.Abs(movement) > 20)
            {
                Scroll(movement);
            }
        }
    }

    public void AddItem(UnityAction itemAction, ObjectHint item)
    {
        if (!m_buttons.Any(b => b.name == item.Name))
        {
            // Create new button
            GameObject newButton = Instantiate(Button);
            newButton.name = item.Name;

            // Set click method
            newButton.GetComponent<Button>().onClick.AddListener(itemAction);

            // Set initial transform
            newButton.transform.SetParent(ButtonContainer.transform);
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

            // Scroll inventory to show the new item
            float maxScrollAmount = Mathf.Max(0, (m_buttons.Count * m_spacing) - (m_spacing * 5));
            ScrollTo(maxScrollAmount);
        }
    }

    public void AddItems(UnityAction itemAction, ObjectHint[] items)
    {
        foreach (var item in items)
        {
            AddItem(itemAction, item);
        }
    }

    public void ScrollUpButtonPressed()
    {
        Scroll(-m_spacing);
    }

    public void ScrollDownButtonPressed()
    {
        Scroll(m_spacing);
    }

    private void Scroll(float movement)
    {
        ScrollTo(m_scrollAmount + movement);
    }

    private void ScrollTo(float position)
    {
        m_scrollAmount = position;

        float maxScrollAMount = Mathf.Max(0, (m_buttons.Count * m_spacing) - (m_spacing * 5));
        m_scrollAmount = Mathf.Clamp(m_scrollAmount, 0, maxScrollAMount);

        ButtonContainer.transform.localPosition = new Vector3(0, m_scrollAmount, 0);
    }
}
