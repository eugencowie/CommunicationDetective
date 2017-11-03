using System.Collections.Generic;

/// <summary>
/// Represents a node in the database which contains a collection of database entries.
/// </summary>
public class Player : OnlineDatabaseNode
{
    public readonly string Id;

    public readonly OnlineDatabaseEntry Lobby;
    public readonly OnlineDatabaseEntry Scene;
    public readonly PlayerClues Clues;

    /// <summary>
    /// Initialises the database node.
    /// </summary>
    public Player(OnlineDatabase database, string id)
        : base(database, "players/" + id)
    {
        Id = id;
        Lobby = new OnlineDatabaseEntry(Database, Key + "/lobby");
        Scene = new OnlineDatabaseEntry(Database, Key + "/scene");
        Clues = new PlayerClues(Database, Key + "/clues");
    }

    /// <summary>
    /// An enumerable collection of database entries.
    /// </summary>
    protected override OnlineDatabaseEntry[] Entries
    {
        get { return new OnlineDatabaseEntry[] { Lobby, Scene }; }
    }
}

/// <summary>
/// Represents a node in the database which contains a collection of database entries.
/// </summary>
public class Lobby : OnlineDatabaseNode
{
    public readonly string Id;

    public readonly OnlineDatabaseEntry CreatedTime;
    public readonly OnlineDatabaseEntry Players;
    public readonly OnlineDatabaseEntry State;

    /// <summary>
    /// Initialises the database node.
    /// </summary>
    public Lobby(OnlineDatabase database, string id)
        : base(database, "lobbies/" + id)
    {
        Id = id;
        CreatedTime = new OnlineDatabaseEntry(Database, Key + "/created-time");
        Players = new OnlineDatabaseEntry(Database, Key + "/players");
        State = new OnlineDatabaseEntry(Database, Key + "/state");
    }

    /// <summary>
    /// An enumerable collection of database entries.
    /// </summary>
    protected override OnlineDatabaseEntry[] Entries
    {
        get { return new OnlineDatabaseEntry[] { CreatedTime, Players, State }; }
    }
}

/// <summary>
/// Represents a node in the database which contains a collection of database entries.
/// </summary>
public class PlayerClues : OnlineDatabaseNode
{
    public readonly PlayerClue[] Clues;

    /// <summary>
    /// Initialises the database node.
    /// </summary>
    public PlayerClues(OnlineDatabase database, string key)
        : base(database, key)
    {
        Clues = new PlayerClue[6] {
            new PlayerClue(Database, Key + "/slot-1"),
            new PlayerClue(Database, Key + "/slot-2"),
            new PlayerClue(Database, Key + "/slot-3"),
            new PlayerClue(Database, Key + "/slot-4"),
            new PlayerClue(Database, Key + "/slot-5"),
            new PlayerClue(Database, Key + "/slot-6")
        };
    }

    /// <summary>
    /// An enumerable collection of database entries.
    /// </summary>
    protected override OnlineDatabaseEntry[] Entries
    {
        get { return new OnlineDatabaseEntry[] { }; }
    }
}

/// <summary>
/// Represents a node in the database which contains a collection of database entries.
/// </summary>
public class PlayerClue : OnlineDatabaseNode
{
    public readonly OnlineDatabaseEntry Name;
    public readonly OnlineDatabaseEntry Hint;

    public PlayerClue(OnlineDatabase database, string key)
        : base(database, key)
    {
        Name = new OnlineDatabaseEntry(Database, Key + "/name");
        Hint = new OnlineDatabaseEntry(Database, Key + "/hint");
    }

    /// <summary>
    /// An enumerable collection of database entries.
    /// </summary>
    protected override OnlineDatabaseEntry[] Entries
    {
        get { return new OnlineDatabaseEntry[] { Name, Hint }; }
    }
}
