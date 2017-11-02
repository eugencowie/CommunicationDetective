using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class Data
{
    public GameObject PlayerButton;
    public GameObject CluePanel;
    public GameObject[] ClueButtons;
    public GameObject[] ClueLabels;
}

public class DatabaseController : MonoBehaviour
{
    [SerializeField] private GameObject[] Data;

    private OnlineManager NetworkController;
    private int m_scene;

    private void Start()
    {
        NetworkController = new OnlineManager();

        NetworkController.GetPlayerScene(scene => {
            if (scene > 0) m_scene = scene;
            else SceneManager.LoadScene("Communication Detective/Scenes/Lobby");
        });

        /*for (int i = 0; i < PlayerButtons.Length; i++)
        {
            int tmp = i;
            PlayerButtons[i].GetComponent<Button>().onClick.AddListener(() => PlayerButtonPressed(tmp));
        }*/
    }

    public void ReturnButtonPressed()
    {
        SceneManager.LoadScene(m_scene);
    }

    private void PlayerButtonPressed(int buttonIndex)
    {
        /*foreach (var button in PlayerButtons)
        {
            button.SetActive(true);
        }
        PlayerButtons[buttonIndex].SetActive(false);

        foreach (var cluePanel in PlayerClues)
        {
            cluePanel.SetActive(false);
        }
        PlayerClues[buttonIndex].SetActive(true);*/
    }
}
