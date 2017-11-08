using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomController : MonoBehaviour
{
    [SerializeField] private GameObject DatabaseButton = null;

    private OnlineManager NetworkController;

    private string m_roomCode;

    private void Start()
    {
        NetworkController = new OnlineManager();

        NetworkController.GetPlayerLobby(room => {
            if (!string.IsNullOrEmpty(room)) m_roomCode = room;
            else SceneManager.LoadScene("Communication Detective/Scenes/Lobby");
        });
    }

    /// <summary>
    /// Called when the leave button in the lobby panel is pressed.
    /// </summary>
    public void LeaveButtonPressed()
    {
        //NetworkController.LeaveLobby(m_roomCode, success => {
        //    if (success) SceneManager.LoadScene("Communication Detective/Scenes/Lobby");
        //});
    }

    public void DatabaseButtonPressed()
    {
        if (DatabaseButton.activeSelf)
        {
            DatabaseButton.SetActive(false);
            SceneManager.LoadScene("Communication Detective/Scenes/Database");
        }
    }
}
