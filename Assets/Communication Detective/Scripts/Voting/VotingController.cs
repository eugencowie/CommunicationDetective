using UnityEngine;
using UnityEngine.SceneManagement;

public class VotingController : MonoBehaviour
{
    [SerializeField] private GameObject ResetButton = null;
    [SerializeField] private GameObject ReturnButton = null;
    [SerializeField] private GameObject[] Backgrounds = new GameObject[4];
    [SerializeField] private VotingSuspect[] Suspects = new VotingSuspect[8];

    private OnlineManager NetworkController;
    private string m_lobby;
    private int m_scene;

    private void Start()
    {
        NetworkController = new OnlineManager();

        ResetButton.SetActive(false);
        ReturnButton.SetActive(false);

        NetworkController.GetPlayerScene(scene => {
            if (scene > 0)
            {
                m_scene = scene;
                SetBackground();
                NetworkController.GetPlayerLobby(lobby => {
                    if (!string.IsNullOrEmpty(lobby)) {
                        m_lobby = lobby;
                        //NetworkController.RegisterCluesChanged(m_lobby, OnSlotChanged);
                        ResetButton.SetActive(true);
                        ReturnButton.SetActive(true);
                    }
                    else SceneManager.LoadScene("Communication Detective/Scenes/Lobby");
                });
            }
            else SceneManager.LoadScene("Communication Detective/Scenes/Lobby");
        });
    }

    private void SetBackground()
    {
        if (m_scene <= Backgrounds.Length)
        {
            foreach (var bg in Backgrounds)
                bg.SetActive(false);

            Backgrounds[m_scene - 1].SetActive(true);
        }
    }

    public void ResetButtonPressed()
    {
        if (ResetButton.activeSelf)
        {
            ResetButton.SetActive(false);

            foreach (var suspect in Suspects)
            {
                suspect.No.gameObject.SetActive(false);
            }

            ResetButton.SetActive(true);
        }
    }

    public void ReturnButtonPressed()
    {
        if (ReturnButton.activeSelf)
        {
            ReturnButton.SetActive(false);
            SceneManager.LoadScene(m_scene);
        }
    }
}
