﻿using UnityEngine;

public class DatabaseInspectController : MonoBehaviour
{
    [SerializeField] private GameObject MainScreen = null;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MainScreen.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
