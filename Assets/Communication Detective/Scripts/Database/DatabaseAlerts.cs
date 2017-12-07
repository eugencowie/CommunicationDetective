using Firebase.Database;
using UnityEngine;

public class DatabaseAlerts : MonoBehaviour
{
    private OnlineManager m_network;
    private string m_lobby;

    private void Start()
    {
        m_network = new OnlineManager();

        m_network.GetPlayerLobby(lobby => {
            if (!string.IsNullOrEmpty(lobby)) {
                m_lobby = lobby;
                m_network.RegisterCluesChanged(lobby, OnSlotChanged);
            }
        });
    }

    private void OnDestroy()
    {
        m_network.DeregisterCluesChanged();
    }

    private void OnSlotChanged(OnlineDatabaseEntry entry, ValueChangedEventArgs args)
    {
        Debug.Log(entry.Key + " | " + (args.Snapshot.Exists ? args.Snapshot.Value.ToString() : ""));
    }
}
