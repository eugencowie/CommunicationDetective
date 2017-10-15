using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum RoomState { Lobby, InRoom, Voting, Finished }

public enum RoomError { None, Unknown, TooFewPlayers, TooManyPlayers }

public class NetworkController
{
    private Database m_database;
    private string m_playerId;

    public NetworkController()
    {
        m_database = new Database();
        m_playerId = GetPlayerId();
    }

    #region Listeners

    private void SubscribeToRoomPlayers(string room, Action<string> valueChanged)
    {
        string roomPlayersKey = string.Format("rooms/{0}/players", room);

        m_database.RegisterListener(roomPlayersKey, (sender, args) => {
            if (args.DatabaseError == null) {
                valueChanged(args.Snapshot.Value.ToString());
            }
        });
    }

    public void RegisterListener(string path, EventHandler<ValueChangedEventArgs> listener)
    {
        m_database.RegisterListener(path, listener);
    }

    public void DeregisterListener(string path, EventHandler<ValueChangedEventArgs> listener)
    {
        m_database.DeregisterListener(path, listener);
    }

    #endregion

    #region Async methods

    public void GetPlayerRoomAsync(Action<string> returnRoom)
    {
        string playerKey = "players/" + m_playerId;

        // If player key exists and player's room exists, return the room code. Otherwise, if the
        // player key does not exists (or it exists, but the player's room does not exists) then
        // return null.
        m_database.ExistsAsync(playerKey, playerExists => {
            if (!playerExists) returnRoom(null);
            else m_database.PullAsync(playerKey, playerValue => {
                string roomKey = "rooms/" + playerValue;
                m_database.ExistsAsync(roomKey, roomExists => {
                    if (roomExists) returnRoom(playerValue);
                    else m_database.DeleteAsync(playerKey, playerSucess => {
                        returnRoom(null);
                    });
                });
            });
        });
    }

    public void JoinRoomAsync(string code, Action<bool> returnSuccess)
    {
        string roomKey = "rooms/" + code;
        string playerKey = "players/" + m_playerId;
        string roomPlayersKey = roomKey + "/players";

        // If room exists, push the room code to the player key, pull the room's list of players,
        // add the player to the room's list of players and push the room's new list of players.
        m_database.ExistsAsync(playerKey, playerExists => {
            if (playerExists) returnSuccess(false);
            else m_database.ExistsAsync(roomKey, roomExists => {
                if (!roomExists) returnSuccess(false);
                else m_database.PushAsync(playerKey, code, playerSuccess => {
                    m_database.PullAsync(roomPlayersKey, roomPlayersValue => {
                        List<string> roomPlayers = roomPlayersValue.Split(',').ToList();
                        if (!roomPlayers.Contains(m_playerId)) {
                            roomPlayers.Add(m_playerId);
                            roomPlayers.RemoveAll(s => string.IsNullOrEmpty(s));
                            roomPlayersValue = string.Join(",", roomPlayers.ToArray());
                            m_database.PushAsync(roomPlayersKey, roomPlayersValue, roomPlayersSuccess => {
                                returnSuccess(playerSuccess && roomPlayersSuccess);
                            });
                        }
                    });
                });
            });
        });
    }

    public void CreateRoomAsync(string code, Action<bool> returnSuccess)
    {
        string createdKey = string.Format("rooms/{0}/created-time", code);
        string stateKey = string.Format("rooms/{0}/state", code);
        string playersKey = string.Format("rooms/{0}/players", code);

        string createdValue = DateTimeOffset.UtcNow.ToString("o");
        string stateValue = ((int)RoomState.Lobby).ToString();
        string playersValue = "";

        // Push the three room values to the database.
        m_database.PushAsync(createdKey, createdValue, createdSuccess => {
            m_database.PushAsync(stateKey, stateValue, stateSuccess => {
                m_database.PushAsync(playersKey, playersValue, playersSuccess => {
                    returnSuccess(createdSuccess && stateSuccess && playersSuccess);
                });
            });
        });
    }

    public void CreateCodeAsync(Action<string> returnCode)
    {
        // Three attempts to find a unique room code.
        string[] codes = { GenerateRandomCode(), GenerateRandomCode(), GenerateRandomCode() };
        List<string> keys = codes.Select(c => "rooms/" + c).ToList();

        // If the generated room code already exists, try again (up to three tries).
        m_database.ExistsAsync(keys[0], exists0 => {
            if (!exists0) returnCode(codes[0]);
            else m_database.ExistsAsync(keys[1], exists1 => {
                if (!exists1) returnCode(codes[1]);
                else m_database.ExistsAsync(keys[2], exists2 => {
                    if (!exists2) returnCode(codes[2]);
                    else returnCode(null);
                });
            });
        });
    }

    public void LeaveRoomAsync(string code, Action<bool> returnSuccess)
    {
        string playerKey = "players/" + m_playerId;

        string roomKey = "rooms/" + code;
        string roomPlayersKey = roomKey + "/players";

        // Delete the player key, if room exists then pull the room's list of players, remove the
        // player from the room's list of players and push the room's new list of players (unless
        // there are player left, in which case delete the room).
        m_database.DeleteAsync(playerKey, playerSuccess => {
            m_database.ExistsAsync(roomKey, roomExists => {
                if (!roomExists) returnSuccess(true);
                else m_database.PullAsync(roomPlayersKey, roomPlayersValue => {
                    List<string> roomPlayers = roomPlayersValue.Split(',').ToList();
                    roomPlayers.Remove(m_playerId);
                    roomPlayers.RemoveAll(s => string.IsNullOrEmpty(s));
                    if (roomPlayers.Count > 0) {
                        roomPlayersValue = string.Join(",", roomPlayers.ToArray());
                        m_database.PushAsync(roomPlayersKey, roomPlayersValue, roomPlayersSuccess => {
                            returnSuccess(playerSuccess && roomPlayersSuccess);
                        });
                    } else {
                        m_database.DeleteAsync(roomKey, roomSuccess => {
                            returnSuccess(playerSuccess && roomSuccess);
                        });
                    }
                });
            });
        });
    }

    public void CanStartGame(string room, Action<RoomError> returnError)
    {
        int requiredPlayers = 2;
        string roomKey = "rooms/" + room;
        string roomPlayersKey = roomKey + "/players";

        m_database.PullAsync(roomPlayersKey, roomPlayersValue => {
            List<string> roomPlayers = roomPlayersValue.Split(',').ToList();
            roomPlayers.RemoveAll(s => string.IsNullOrEmpty(s));
            if (roomPlayers.Count < requiredPlayers) returnError(RoomError.TooFewPlayers);
            else if (roomPlayers.Count > requiredPlayers) returnError(RoomError.TooManyPlayers);
            else returnError(RoomError.None);
        });
    }

    public void SetRoomState(string room, RoomState state)
    {
        string roomKey = "rooms/" + room;
        string roomStateKey = roomKey + "/state";
        string roomStateValue = ((int)state).ToString();

        m_database.PushAsync(roomStateKey, roomStateValue);
    }

    public void GetPlayerRoomNrAsync(string room, Action<int> returnRoomNr)
    {
        string roomKey = "rooms/" + room;
        string roomPlayersKey = roomKey + "/players";

        m_database.PullAsync(roomPlayersKey, roomPlayersValue => {
            List<string> roomPlayers = roomPlayersValue.Split(',').ToList();
            roomPlayers.RemoveAll(s => string.IsNullOrEmpty(s));
            int roomNr = -1;
            for (int i = 0; i < roomPlayers.Count; i++) {
                if (roomPlayers[i] == m_playerId) {
                    roomNr = i+1;
                }
            }
            returnRoomNr(roomNr);
        });
    }

    #endregion

    #region Utility methods

    /// <summary>
    /// Returns a unique player id.
    /// </summary>
    private static string GetPlayerId()
    {
        const string key = "UniquePlayerId";

        if (PlayerPrefs.HasKey(key))
        {
            string value = PlayerPrefs.GetString(key);

            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }
        }

        string id = SystemInfo.deviceUniqueIdentifier;
        PlayerPrefs.SetString(key, id);
        return id;
    }

    /// <summary>
    /// Generates a random five-character room code.
    /// </summary>
    private static string GenerateRandomCode()
    {
        const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        string roomCode = "";

        for (int i = 0; i < 5; i++)
        {
            // Gets a random valid character and adds it to the room code string.
            int randomIndex = UnityEngine.Random.Range(0, validChars.Length - 1);
            char randomChar = validChars[randomIndex];
            roomCode += randomChar;
        }

        return roomCode;
    }

    #endregion
}
