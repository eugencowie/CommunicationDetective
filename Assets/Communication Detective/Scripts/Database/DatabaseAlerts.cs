using Firebase.Database;
using UnityEngine;

/*public struct ClueData
{
    bool SlotContains = True;
    bool NotBeenLookedAt = True;

    if(SlotContains && NotBeenLookedAt)
        {

        Show !
        }
        
}*/

public static class StaticClues
{
    
}

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
        string[] keys = entry.Key.Split('/');
        if (keys.Length >= 5)
        {
            string field = keys[4];

            if (args.Snapshot.Exists && field == "name")
            {
                string value = args.Snapshot.Value.ToString();

                int slot;
                if (int.TryParse(keys[3].Replace("slot-", ""), out slot))
                {
                    m_network.GetPlayerNumber(m_lobby, keys[1], player => {
                        Debug.Log("player-" + player + "/" + "slot-" + slot + "/" + field + " = " + value);
                    });
                }
            }
        }
    }
}
