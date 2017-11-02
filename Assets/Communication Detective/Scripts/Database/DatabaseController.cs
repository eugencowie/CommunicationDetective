using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class Data
{
    [SerializeField] public GameObject PlayerButton;
    [SerializeField] public GameObject CluePanel;
    [SerializeField] public GameObject[] ClueButtons;
    [SerializeField] public GameObject[] ClueLabels;
}

public class DatabaseController : MonoBehaviour
{
    [SerializeField] private Data[] Data;

    private OnlineManager NetworkController;
    private int m_scene;

    private void Start()
    {
        NetworkController = new OnlineManager();

        NetworkController.GetPlayerScene(scene => {
            if (scene > 0) m_scene = scene;
            else SceneManager.LoadScene("Communication Detective/Scenes/Lobby");
        });

        for (int i = 0; i < Data.Length; i++)
        {
            Data data = Data[i];
            data.PlayerButton.GetComponent<Button>().onClick.AddListener(() => PlayerButtonPressed(data));
        }
    }

    public void ReturnButtonPressed()
    {
        SceneManager.LoadScene(m_scene);
    }

    private void PlayerButtonPressed(Data data)
    {
        foreach (var button in Data.Select(d => d.PlayerButton))
        {
            button.SetActive(true);
        }
        data.PlayerButton.SetActive(false);

        foreach (var cluePanel in Data.Select(d => d.CluePanel))
        {
            cluePanel.SetActive(false);
        }
        data.CluePanel.SetActive(true);
    }
}
