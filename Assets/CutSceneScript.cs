using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneScript : MonoBehaviour {

    public GameObject ResetBtn;

    private OnlineManager NetworkController;
    private string m_roomCode;


    // Use this for initialization
    void Start () {
        NetworkController = new OnlineManager();
        ResetBtn.SetActive(false);
        NetworkController.GetPlayerLobby(room => {
            if (!string.IsNullOrEmpty(room)) {
               m_roomCode = room;
                ResetBtn.SetActive(true);
            }
            else SceneManager.LoadScene("Communication Detective/Scenes/Lobby");
        });
    }
	
    public void ResetButton()
    {
        NetworkController.LeaveLobby(m_roomCode, _ => {
            SceneManager.LoadScene("Communication Detective/Scenes/Lobby");
        });
    }
}
