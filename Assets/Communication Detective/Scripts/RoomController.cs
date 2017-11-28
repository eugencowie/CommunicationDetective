using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomController : MonoBehaviour
{
    [SerializeField] private GameObject MainScreen = null;
    [SerializeField] private GameObject ConfirmLeaveScreen = null;
    [SerializeField] private GameObject ConfirmReadyScreen = null;
    [SerializeField] private GameObject ReadyButton = null;
    [SerializeField] private GameObject DatabaseButton = null;

    private OnlineManager NetworkController;

    private string m_roomCode;

    private Dictionary<string, bool> m_readyPlayers = new Dictionary<string, bool>();

    private void Start()
    {
        NetworkController = new OnlineManager();

        ReadyButton.SetActive(false);
        DatabaseButton.SetActive(false);

        NetworkController.GetPlayerLobby(room => {
            if (!string.IsNullOrEmpty(room)) {
                NetworkController.GetPlayers(room, players => {
                    m_roomCode = room;
                    foreach (var player in players) m_readyPlayers[player] = false;
                    NetworkController.RegisterReadyChanged(room, OnReadyChanged);
                    ReadyButton.SetActive(true);
                    DatabaseButton.SetActive(true);
                });
            }
            else SceneManager.LoadScene("Communication Detective/Scenes/Lobby");
        });
    }

    /// <summary>
    /// Called when the leave button in the lobby panel is pressed.
    /// </summary>
    public void LeaveButtonPressed()
    {
        ConfirmLeaveScreen.SetActive(true);
        MainScreen.SetActive(false);
    }
    
    public void DatabaseButtonPressed()
    {
        if (DatabaseButton.activeSelf)
        {
            DatabaseButton.SetActive(false);
            SceneManager.LoadScene("Communication Detective/Scenes/Database");
        }
    }

    public void ReadyButtonPressed()
    {
        MainScreen.SetActive(false);
        ConfirmReadyScreen.SetActive(true);
    }

    private void OnReadyChanged(OnlineDatabaseEntry entry, ValueChangedEventArgs args)
    {
        if (ReadyButton == null)
            return;

        if (args.Snapshot.Exists)
        {
            string value = args.Snapshot.Value.ToString();

            if (value == "true")
            {
                string[] key = entry.Key.Split('/');
                string player = key[1];
                m_readyPlayers[player] = true;

                if (player == OnlineManager.GetPlayerId())
                {
                    ReadyButtonPressed();
                }

                if (!m_readyPlayers.Any(p => p.Value == false))
                {
                    NetworkController.DeregisterReadyChanged(m_roomCode);
                    SceneManager.LoadScene("Communication Detective/Scenes/Voting");
                }
            }
        }
    }

    public void ConfirmLeave_ContinueButtonPressed()
    {
        NetworkController.LeaveLobby(m_roomCode, _ => {
            SceneManager.LoadScene("Communication Detective/Scenes/Lobby");
        });

        //NetworkController.LeaveLobby(m_roomCode, success => {
        //    if (success) SceneManager.LoadScene("Communication Detective/Scenes/Lobby");
        //});
    }

    public void ConfirmLeave_CancelButtonPressed()
    {
        ConfirmLeaveScreen.SetActive(false);
        MainScreen.SetActive(true);
    }

    public void ConfirmReady_ContinueButtonPressed()
    {
        if (ReadyButton.activeSelf)
        {
            ReadyButton.SetActive(false);
            NetworkController.ReadyUp(success => {
                ReadyButton.SetActive(true);
                if (success)
                {
                    ReadyButton.GetComponent<Image>().color = Color.yellow;
                    foreach (Transform t in ReadyButton.gameObject.transform)
                    {
                        var text = t.GetComponent<Text>();
                        if (text != null) text.text = "Waiting...";
                    }
                }
            });
        }
    }

    public void ConfirmReady_CancelButtonPressed()
    {
        ConfirmReadyScreen.SetActive(false);
        MainScreen.SetActive(true);
    }
}
