using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Tooltip("GameObject with a NetworkController component to handle network queries")]
    public GameObject NetworkControllerObject;
    private NetworkController NetworkController
    {
        get { return NetworkControllerObject.GetComponent<NetworkController>(); }
    }

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

    public GameObject StartPanel;
    public GameObject JoinPanel;
    public GameObject LobbyPanel;
    public GameObject WaitPanel;

    private void Start()
    {
        SwitchPanel(WaitPanel);

        NetworkController.GetPlayerRoomAsync(room => {
            if (string.IsNullOrEmpty(room)) SwitchPanel(StartPanel);
            else {
                CodeLabel.text = room;
                RegisterOnPlayersChanged(room);
                SwitchPanel(LobbyPanel);
            }
        });
    }

    public void JoinButtonPressed()
    {
        SwitchPanel(JoinPanel);
    }

    public void SubmitButtonPressed()
    {
        SwitchPanel(WaitPanel);

        NetworkController.JoinRoomAsync(CodeField.text.ToUpper(), success => {
            if (!success) {
                CodeField.text = "";
                SwitchPanel(JoinPanel);
            } else {
                CodeLabel.text = CodeField.text.ToUpper();
                RegisterOnPlayersChanged(CodeLabel.text);
                SwitchPanel(LobbyPanel);
            }
        });
    }

    public void CreateButtonPressed()
    {
        SwitchPanel(WaitPanel);

        NetworkController.CreateCodeAsync(code => {
            if (string.IsNullOrEmpty(code)) SwitchPanel(StartPanel);
            else {
                NetworkController.CreateRoomAsync(code, createSuccess => {
                    if (!createSuccess) SwitchPanel(StartPanel);
                    else {
                        NetworkController.JoinRoomAsync(code, joinSuccess => {
                            if (!joinSuccess) SwitchPanel(StartPanel);
                            else {
                                CodeLabel.text = code;
                                RegisterOnPlayersChanged(code);
                                SwitchPanel(LobbyPanel);
                            }
                        });
                    }
                });
            }
        });
    }

    public void LeaveButtonPressed()
    {
        SwitchPanel(WaitPanel);

        NetworkController.LeaveRoomAsync(CodeLabel.text, success => {
            if (success) {
                DeregisterOnPlayersChanged(CodeLabel.text);
                CodeLabel.text = "_____";
                SwitchPanel(StartPanel);
            }
            else SwitchPanel(LobbyPanel);
        });
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
        string roomPlayersKey = string.Format("rooms/{0}/players", room);

        NetworkController.RegisterListener(roomPlayersKey, OnPlayersChanged);
    }

    private void DeregisterOnPlayersChanged(string room)
    {
        string roomPlayersKey = string.Format("rooms/{0}/players", room);

        NetworkController.DeregisterListener(roomPlayersKey, OnPlayersChanged);
    }

    private void OnPlayersChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.Snapshot.Exists)
        {
            PlayersLabel.text = args.Snapshot.Value.ToString().Replace(',', '\n');
        }
    }
}
