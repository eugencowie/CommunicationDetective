using Firebase.Database;
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
    [SerializeField] public List<GameObject> Slots;
}

public class DatabaseController : MonoBehaviour
{
    [SerializeField] private Data[] Data = new Data[4];
    [SerializeField] private GameObject ButtonTemplate = null;

    private OnlineManager NetworkController;
    private string m_lobby;
    private int m_scene;

    private void Start()
    {
        NetworkController = new OnlineManager();

        NetworkController.GetPlayerLobby(lobby => {
            if (!string.IsNullOrEmpty(lobby)) {
                m_lobby = lobby;
                DownloadItems();
                NetworkController.RegisterCluesChanged(m_lobby, OnSlotChanged);
            }
            else SceneManager.LoadScene("Communication Detective/Scenes/Lobby");
        });

        NetworkController.GetPlayerScene(scene => {
            if (scene > 0) m_scene = scene;
            else SceneManager.LoadScene("Communication Detective/Scenes/Lobby");
        });

        for (int i = 0; i < Data.Length; i++)
        {
            Data data = Data[i];
            data.PlayerButton.GetComponent<Button>().onClick.AddListener(() => PlayerButtonPressed(data));
        }

        PlayerButtonPressed(Data[0]);
    }

    public void ReturnButtonPressed()
    {
        SceneManager.LoadScene(m_scene);
    }

    private void PlayerButtonPressed(Data data)
    {
        foreach (var button in Data.Select(d => d.PlayerButton))
        {
            ColorBlock colours = button.GetComponent<Button>().colors;
            colours.normalColor = colours.highlightedColor = Color.white;
            button.GetComponent<Button>().colors = colours;
        }
        ColorBlock colours2 = data.PlayerButton.GetComponent<Button>().colors;
        colours2.normalColor = colours2.highlightedColor = Color.green;
        data.PlayerButton.GetComponent<Button>().colors = colours2;

        foreach (var cluePanel in Data.Select(d => d.CluePanel))
        {
            cluePanel.SetActive(false);
        }
        data.CluePanel.SetActive(true);
    }

    public void UploadItem(int slot, ObjectHintData hint)
    {
        NetworkController.UploadDatabaseItem(slot, hint);
    }

    private void DownloadItems()
    {
        for (int i = 0; i < 4; i++)
        {
            int tmp = i;
            NetworkController.DownloadClues(m_lobby, tmp, player => {
                for (int j = 0; j < player.Clues.Clues.Length; j++) {
                    int tmp2 = j;
                    var clue = player.Clues.Clues[tmp2];
                    clue.PullEntries(_ => {
                        if (!string.IsNullOrEmpty(clue.Name.Value)) {
                            //Debug.Log(string.Format("Player {0}, slot {1}: {2}", tmp, tmp2, clue.Name.Value));
                            var slot = Data[tmp].Slots[tmp2];
                            foreach (Transform t in slot.transform) if (t.gameObject.name == clue.Name.Value) Destroy(t.gameObject);
                            var newObj = Instantiate(ButtonTemplate, ButtonTemplate.transform.parent);
                            newObj.SetActive(true);
                            newObj.name = clue.Name.Value;
                            newObj.transform.SetParent(slot.transform);
                            foreach (Transform t in newObj.transform) {
                                if (t.gameObject.GetComponent<Text>() != null) {
                                    t.gameObject.GetComponent<Text>().text = clue.Name.Value;
                                }
                            }
                            newObj.GetComponent<DragHandler>().enabled = false;
                            slot.GetComponent<Slot>().Text.GetComponent<Text>().text = clue.Hint.Value;
                        }
                    });
                }
            });
        }
    }
    
    private void OnSlotChanged(OnlineDatabaseEntry entry, ValueChangedEventArgs args)
    {
        if (args.Snapshot.Exists)
        {
            Debug.Log(entry.Key + ": " + args.Snapshot.Value);
        }
    }
}
