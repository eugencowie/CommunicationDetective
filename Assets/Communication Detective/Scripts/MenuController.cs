using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Tooltip("GameObject with a Text component to display the generated room code")]
    public GameObject CodeLabelObject;
    private Text CodeLabel
    {
        get { return CodeLabelObject.GetComponent<Text>(); }
    }

    [Tooltip("GameObject with an InputField component to input the room code to join")]
    public GameObject CodeFieldObject;
    private InputField CodeField
    {
        get { return CodeFieldObject.GetComponent<InputField>(); }
    }

    [Tooltip("GameObject with a Text component to display the list of players in the room")]
    public GameObject PlayersLabelObject;
    private Text PlayersLabel
    {
        get { return PlayersLabelObject.GetComponent<Text>(); }
    }

    [Tooltip("GameObject with a Text component to display the status of the room")]
    public GameObject StatusLabelObject;
    private Text StatusLabel
    {
        get { return StatusLabelObject.GetComponent<Text>(); }
    }

    public GameObject StartPanel;
    public GameObject JoinPanel;
    public GameObject LobbyPanel;
    public GameObject WaitPanel;
    
    [Range(1,20)]
    public int RequiredPlayers = 4;

    private Network NetworkController;

    private void Start()
    {
        NetworkController = new Network();

        SwitchPanel(WaitPanel);

        NetworkController.GetPlayerLobby(room => {
            if (string.IsNullOrEmpty(room)) SwitchPanel(StartPanel);
            else {
                CodeLabel.text = room;
                RegisterOnPlayersChanged(room);
                RegisterOnRoomStateChanged(room);
                SwitchPanel(LobbyPanel);
            }
        });
    }

    /// <summary>
    /// Called when the join button in the start panel is pressed.
    /// </summary>
    public void JoinButtonPressed()
    {
        SwitchPanel(JoinPanel);
    }

    /// <summary>
    /// Called when the submit button in the join panel is pressed.
    /// </summary>
    public void SubmitButtonPressed()
    {
        if (!string.IsNullOrEmpty(CodeField.text))
        {
            SwitchPanel(WaitPanel);

            NetworkController.JoinLobby(CodeField.text.ToUpper(), success => {
                if (!success) {
                    CodeField.text = "";
                    SwitchPanel(JoinPanel);
                } else {
                    CodeLabel.text = CodeField.text.ToUpper();
                    RegisterOnPlayersChanged(CodeLabel.text);
                    RegisterOnRoomStateChanged(CodeLabel.text);
                    SwitchPanel(LobbyPanel);
                }
            });
        }
    }

    /// <summary>
    /// Called when the create button in the start panel is pressed.
    /// </summary>
    public void CreateButtonPressed()
    {
        SwitchPanel(WaitPanel);

        NetworkController.CreateLobbyCode(code => {
            if (string.IsNullOrEmpty(code)) SwitchPanel(StartPanel);
            else {
                NetworkController.CreateLobby(code, createSuccess => {
                    if (!createSuccess) SwitchPanel(StartPanel);
                    else {
                        NetworkController.JoinLobby(code, joinSuccess => {
                            if (!joinSuccess) SwitchPanel(StartPanel);
                            else {
                                CodeLabel.text = code;
                                RegisterOnPlayersChanged(code);
                                RegisterOnRoomStateChanged(code);
                                SwitchPanel(LobbyPanel);
                            }
                        });
                    }
                });
            }
        });
    }

    /// <summary>
    /// Called when the start button in the lobby panel is pressed.
    /// </summary>
    public void StartButtonPressed()
    {
        SwitchPanel(WaitPanel);

        NetworkController.CanStartGame(CodeLabel.text, RequiredPlayers, error => {
            if (error != LobbyError.None) {
                if (error == LobbyError.TooFewPlayers) StatusLabel.text = "too few players, requires " + RequiredPlayers;
                else if (error == LobbyError.TooManyPlayers) StatusLabel.text = "too many players, requires " + RequiredPlayers;
                else StatusLabel.text = "unknown error";
                SwitchPanel(LobbyPanel);
            }
            else NetworkController.SetLobbyState(CodeLabel.text, LobbyState.InGame);
        });
    }

    /// <summary>
    /// Called when the leave button in the lobby panel is pressed.
    /// </summary>
    public void LeaveButtonPressed()
    {
        SwitchPanel(WaitPanel);

        NetworkController.LeaveLobby(CodeLabel.text, success => {
            if (success) {
                DeregisterOnRoomStateChanged(CodeLabel.text);
                DeregisterOnPlayersChanged(CodeLabel.text);
                CodeLabel.text = "_____";
                SwitchPanel(StartPanel);
            }
            else SwitchPanel(LobbyPanel);
        });
    }

    public void CodeFieldChanged(string s)
    {
        CodeField.text = CodeField.text.ToUpper();
    }

    private void SwitchPanel(GameObject panel)
    {
        // Disable all panels
        foreach (var p in new GameObject[] { StartPanel, JoinPanel, LobbyPanel, WaitPanel })
        {
            p.SetActive(false);
        }

        // Enable specified panel
        panel.SetActive(true);
    }

    private void RegisterOnPlayersChanged(string room)
    {
        string roomPlayersKey = string.Format("lobbies/{0}/players", room);

        NetworkController.RegisterListener(roomPlayersKey, OnPlayersChanged);
    }

    private void DeregisterOnPlayersChanged(string room)
    {
        string roomPlayersKey = string.Format("lobbies/{0}/players", room);

        NetworkController.DeregisterListener(roomPlayersKey, OnPlayersChanged);
    }

    private void OnPlayersChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.Snapshot.Exists)
        {
            PlayersLabel.text = args.Snapshot.Value.ToString().Replace(',', '\n');
        }
    }

    private void RegisterOnRoomStateChanged(string room)
    {
        string roomStateKey = string.Format("lobbies/{0}/state", room);

        NetworkController.RegisterListener(roomStateKey, OnRoomStateChanged);
    }

    private void DeregisterOnRoomStateChanged(string room)
    {
        string roomStateKey = string.Format("lobbies/{0}/state", room);

        NetworkController.DeregisterListener(roomStateKey, OnRoomStateChanged);
    }

    private void OnRoomStateChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.Snapshot.Exists)
        {
            string statusStr = args.Snapshot.Value.ToString();
            int statusNr = -1;
            if (int.TryParse(statusStr, out statusNr))
            {
                LobbyState state = (LobbyState)statusNr;
                if (state == LobbyState.InGame)
                {
                    NetworkController.AssignPlayerScenes(CodeLabel.text, roomNr => {
                        if (roomNr >= 1 && roomNr <= 4) {
                            string room = "Room" + roomNr.ToString();
                            StatusLabel.text = "joined game, your room = " + room;
                            DeregisterOnRoomStateChanged(CodeLabel.text);
                            DeregisterOnPlayersChanged(CodeLabel.text);
                            SceneManager.LoadScene("Communication Detective/Scenes/" + room);
                        }
                        else StatusLabel.text = "invalid room number";
                    });
                }
            }
        }
    }
}
