using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomController : MonoBehaviour
{
    private Network NetworkController;

    private string m_roomCode;

    private void Start()
    {
        NetworkController = new Network();

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
        NetworkController.LeaveRoomAsync(m_roomCode, success => {
            if (success) SceneManager.LoadScene("Communication Detective/Scenes/Lobby");
        });
    }
}
