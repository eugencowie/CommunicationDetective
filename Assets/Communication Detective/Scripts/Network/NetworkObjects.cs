using System.Collections.Generic;

/// <summary>
/// Represents a node in the database which contains a collection of database entries.
/// </summary>
public class Player : DatabaseNode
{
    public readonly string Id;

    public readonly DatabaseEntry Lobby;
    public readonly DatabaseEntry Scene;

    /// <summary>
    /// Initialises the database node.
    /// </summary>
    public Player(Database database, string id)
        : base(database, "players/" + id)
    {
        Id = id;
        Lobby = new DatabaseEntry(Database, Key + "/lobby");
        Scene = new DatabaseEntry(Database, Key + "/scene");
    }

    /// <summary>
    /// An enumerable collection of database entries.
    /// </summary>
    protected override IEnumerable<DatabaseEntry> Entries
    {
        get { return new DatabaseEntry[] { Lobby, Scene }; }
    }
}

/// <summary>
/// Represents a node in the database which contains a collection of database entries.
/// </summary>
public class Lobby : DatabaseNode
{
    public readonly string Id;

    public readonly DatabaseEntry CreatedTime;
    public readonly DatabaseEntry Players;
    public readonly DatabaseEntry State;

    /// <summary>
    /// Initialises the database node.
    /// </summary>
    public Lobby(Database database, string id)
        : base(database, "lobbies/" + id)
    {
        Id = id;
        CreatedTime = new DatabaseEntry(Database, Key + "/created-time");
        Players = new DatabaseEntry(Database, Key + "/players");
        State = new DatabaseEntry(Database, Key + "/state");
    }

    /// <summary>
    /// An enumerable collection of database entries.
    /// </summary>
    protected override IEnumerable<DatabaseEntry> Entries
    {
        get { return new DatabaseEntry[] { CreatedTime, Players, State }; }
    }
}
