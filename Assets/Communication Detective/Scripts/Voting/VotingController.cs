using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VotingController : MonoBehaviour
{
    [SerializeField] private GameObject ResetButton = null;
    [SerializeField] private GameObject ReturnButton = null;
    [SerializeField] private GameObject VoteButton = null;
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
        VoteButton.SetActive(false);

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
                        VoteButton.SetActive(true);
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

    public void ReturnButtonPressed()
    {
        if (ReturnButton.activeSelf)
        {
            ReturnButton.SetActive(false);
            SceneManager.LoadScene("Communication Detective/Scenes/Database");
        }
    }

    public void ResetButtonPressed()
    {
        if (ResetButton.activeSelf)
        {
            ResetButton.SetActive(false);

            for (int i = 0; i < Suspects.Length; i++)
            {
                int prevIdx = i - 1;
                int nextIdx = i + 1;

                if (prevIdx < 0)
                    prevIdx = Suspects.Length - 1;

                if (nextIdx >= Suspects.Length)
                    nextIdx = 0;

                var current = Suspects[i];

                var prev = Suspects[prevIdx];
                var next = Suspects[nextIdx];

                var page = current.gameObject.GetComponent<VotingPageController>();

                page.PanelLeft = prev.gameObject;
                page.PanelRight = next.gameObject;

                // reset opacity
                if (current.Slot != null)
                {
                    var color = current.Slot.GetComponent<Image>().color;
                    color.a = 1.0f;
                    current.Slot.GetComponent<Image>().color = color;
                }
            }

            ResetButton.SetActive(true);
        }
    }

    public void VoteButtonPressed()
    {
        var current = Suspects.First(s => s.gameObject.activeSelf);
        if (current != null)
        {
            Debug.Log(current.Name.text);
        }
    }
}
