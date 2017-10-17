﻿using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum LobbyState { Lobby, InGame, Voting, Finished }

public enum LobbyError { None, Unknown, TooFewPlayers, TooManyPlayers }

public class Network
{
    private Database m_database;
    private Player m_player;
    private Lobby m_lobby;

    public Network()
    {
        m_database = new Database();
        m_player = new Player(m_database, GetPlayerId());
        m_lobby = null;
    }

    #region Async methods

    /// <summary>
    /// If the player is listed as being in a lobby and that lobby does exist, returns the lobby
    /// code. Otherwise, returns null.
    /// </summary>
    public void GetPlayerLobby(Action<string> returnLobby)
    {
        Database.ValidateAction(ref returnLobby);
        
        // If 'players/{0}/lobby' exists and 'lobbies/{1}' exists, return lobby code.
        m_player.Lobby.Pull(success => {
            if (success) {
                m_lobby = new Lobby(m_database, m_player.Lobby.Value);
                m_lobby.Exists(exists => {
                    if (exists) returnLobby(m_lobby.Id);
                    else m_player.Delete(_ => returnLobby(null));
                });
            }
            else returnLobby(null);
        });
    }

    /// <summary>
    /// If lobby exists, updates player entry to new lobby and adds player to lobby.
    /// </summary>
    public void JoinLobby(string code, Action<bool> returnSuccess=null)
    {
        Database.ValidateAction(ref returnSuccess);

        // If 'lobbies/{0}' exists, push 'players/{1}/lobby', get 'lobbies/{0}/players', add
        // player to list and push 'lobbies/{0}/players' back up.
        m_lobby = new Lobby(m_database, code);
        m_lobby.Exists(exists => {
            if (exists) {
                m_player.Lobby.Value = code;
                m_player.Lobby.Push(playerSuccess => {
                    if (playerSuccess) {
                        m_lobby.Players.Pull(lobbySuccess => {
                            if (lobbySuccess) {
                                List<string> players = m_lobby.Players.Value.Split(',').ToList();
                                if (!players.Contains(m_player.Id)) {
                                    players.Add(m_player.Id);
                                    players.RemoveAll(s => string.IsNullOrEmpty(s));
                                    m_lobby.Players.Value = string.Join(",", players.ToArray());
                                    m_lobby.Players.Push(roomPlayersSuccess => {
                                        if (roomPlayersSuccess) returnSuccess(true);
                                        else returnSuccess(false);
                                    });
                                }
                            }
                            else returnSuccess(false);
                        });
                    }
                    else returnSuccess(false);
                });
            }
            else returnSuccess(false);
        });
    }

    /// <summary>
    /// Creates a lobby on the server.
    /// </summary>
    public void CreateLobby(string code, Action<bool> returnSuccess=null)
    {
        Database.ValidateAction(ref returnSuccess);

        m_lobby = new Lobby(m_database, code);

        m_lobby.CreatedTime.Value = DateTimeOffset.UtcNow.ToString("o");
        m_lobby.State.Value = ((int)LobbyState.Lobby).ToString();

        m_lobby.CreatedTime.Push(success1 => {
            m_lobby.Players.Push(success2 => {
                m_lobby.State.Push(success3 => {
                    returnSuccess(success1 && success2 && success3);
                });
            });
        });
    }

    /// <summary>
    /// Attempts to generate a lobby code which is not in use. If all codes generated are in
    /// use, returns null.
    /// </summary>
    public void CreateLobbyCode(Action<string> returnCode)
    {
        Database.ValidateAction(ref returnCode);

        // Three attempts to find a unique room code.
        string[] codes = { GenerateRandomCode(), GenerateRandomCode(), GenerateRandomCode() };
        List<string> keys = codes.Select(c => new Lobby(m_database, c).Key).ToList();

        // If the generated room code already exists, try again (up to three tries).
        m_database.Exists(keys[0], exists0 => {
            if (!exists0) returnCode(codes[0]);
            else m_database.Exists(keys[1], exists1 => {
                if (!exists1) returnCode(codes[1]);
                else m_database.Exists(keys[2], exists2 => {
                    if (!exists2) returnCode(codes[2]);
                    else returnCode(null);
                });
            });
        });
    }

    /// <summary>
    /// Deletes the player entry and removes the player from the lobby. If no players are left in the
    /// lobby, deletes the lobby.
    /// </summary>
    public void LeaveLobby(string code, Action<bool> returnSuccess=null)
    {
        Database.ValidateAction(ref returnSuccess);

        // Delete 'players/{0}', pull 'lobbies/{1}/players', remove the player from list and push
        // 'lobbies/{1}/players' back up (unless there are no players left, then delete the lobby).
        m_player.Delete(success1 => {
            if (success1) {
                m_lobby = new Lobby(m_database, code); // TODO
                m_lobby.Players.Pull(success2 => {
                    if (success2) {
                        List<string> layers = m_lobby.Players.Value.Split(',').ToList();
                        layers.Remove(m_player.Id);
                        layers.RemoveAll(s => string.IsNullOrEmpty(s));
                        if (layers.Count > 0) {
                            m_lobby.Players.Value = string.Join(",", layers.ToArray());
                            m_lobby.Players.Push(returnSuccess);
                        } else {
                            m_lobby.Delete(returnSuccess);
                        }
                    }
                    else returnSuccess(false);
                });
            }
            else returnSuccess(false);
        });
    }

    /// <summary>
    /// Checks if the lobby has the required number of players.
    /// </summary>
    public void CanStartGame(string code, int requiredPlayers, Action<LobbyError> returnError)
    {
        Database.ValidateAction(ref returnError);

        m_lobby = new Lobby(m_database, code); // TODO
        m_lobby.Players.Pull(success => {
            if (success) {
                List<string> players = m_lobby.Players.Value.Split(',').ToList();
                players.RemoveAll(s => string.IsNullOrEmpty(s));
                if (players.Count < requiredPlayers) returnError(LobbyError.TooFewPlayers);
                else if (players.Count > requiredPlayers) returnError(LobbyError.TooManyPlayers);
                else returnError(LobbyError.None);
            }
            else returnError(LobbyError.Unknown);
        });
    }

    /// <summary>
    /// Pushes a new lobby state to the server.
    /// </summary>
    public void SetLobbyState(string code, LobbyState state, Action<bool> returnSuccess=null)
    {
        Database.ValidateAction(ref returnSuccess);

        m_lobby = new Lobby(m_database, code); // TODO
        m_lobby.State.Value = ((int)state).ToString();
        m_lobby.State.Push(returnSuccess);
    }

    /// <summary>
    /// 
    /// </summary>
    public void AssignPlayerScenes(string code, Action<int> returnScene)
    {
        Database.ValidateAction(ref returnScene);

        m_lobby = new Lobby(m_database, code); // TODO
        m_lobby.Players.Pull(success => {
            if (success) {
                List<string> players = m_lobby.Players.Value.Split(',').ToList();
                players.RemoveAll(s => string.IsNullOrEmpty(s));
                players = players.OrderBy(_ => UnityEngine.Random.value).ToList();
                int ourScene = -1;
                for (int i = 0; i < players.Count; i++) {
                    Player player = new Player(m_database, players[i]);
                    if (player.Id == m_player.Id) {
                        ourScene = (i+1);
                    }
                    player.Scene.Value = (i+1).ToString();
                    player.Scene.Push();
                }
                returnScene(ourScene);
            }
            else returnScene(-1);
        });
    }

    /*
    /// <summary>
    /// 
    /// </summary>
    public void GetPlayerScene(string code, Action<string> returnScene)
    {
        Database.ValidateAction(ref returnScene);

        m_lobby = new Lobby(m_database, code); // TODO

        string roomKey = "lobbies/" + code;
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
    */

    #endregion

    #region Listeners

    private void SubscribeToRoomPlayers(string room, Action<string> valueChanged)
    {
        string roomPlayersKey = string.Format("lobbies/{0}/players", room);

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
