using System.Collections.Generic;

/// <summary>
/// Represents a node in the database which contains a collection of database entries.
/// </summary>
public class Player : OnlineDatabaseNode
{
    public readonly string Id;

    public readonly OnlineDatabaseEntry Lobby;
    public readonly OnlineDatabaseEntry Scene;

    /// <summary>
    /// Initialises the database node.
    /// </summary>
    public Player(OnlineDatabase database, string id)
        : base(database, "players/" + id)
    {
        Id = id;
        Lobby = new OnlineDatabaseEntry(Database, Key + "/lobby");
        Scene = new OnlineDatabaseEntry(Database, Key + "/scene");
    }

    /// <summary>
    /// An enumerable collection of database entries.
    /// </summary>
    protected override IEnumerable<OnlineDatabaseEntry> Entries
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
    protected override IEnumerable<OnlineDatabaseEntry> Entries
    {
        get { return new OnlineDatabaseEntry[] { CreatedTime, Players, State }; }
    }
}
