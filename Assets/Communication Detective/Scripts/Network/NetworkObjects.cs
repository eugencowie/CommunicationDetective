public class Player : DatabaseNode
{
    public readonly string Id;
    public readonly DatabaseEntry Lobby;
    public readonly DatabaseEntry Scene;

    public Player(Database database, string id)
        : base(database, "players/" + id)
    {
        Id = id;
        Lobby = new DatabaseEntry(Database, Key + "/lobby");
        Scene = new DatabaseEntry(Database, Key + "/scene");
    }
}

public class Lobby : DatabaseNode
{
    public readonly string Id;
    public readonly DatabaseEntry CreatedTime;
    public readonly DatabaseEntry Players;
    public readonly DatabaseEntry State;

    public Lobby(Database database, string id)
        : base(database, "lobbies/" + id)
    {
        Id = id;
        CreatedTime = new DatabaseEntry(Database, Key + "/created-time");
        Players = new DatabaseEntry(Database, Key + "/players");
        State = new DatabaseEntry(Database, Key + "/state");
    }
}
