using System;
using System.Collections.Generic;

/// <summary>
/// Represents a node in the database which contains a collection of database entries.
/// </summary>
public abstract class OnlineDatabaseNode
{
    /// <summary>
    /// Reference to the database.
    /// </summary>
    protected readonly OnlineDatabase Database;

    /// <summary>
    /// The full path to the node in the database.
    /// </summary>
    public readonly string Key;

    /// <summary>
    /// Initialises the database node.
    /// </summary>
    public OnlineDatabaseNode(OnlineDatabase database, string key)
    {
        Database = database;
        Key = key;
    }

    /// <summary>
    /// An enumerable collection of database entries.
    /// </summary>
    protected abstract IEnumerable<OnlineDatabaseEntry> Entries { get; }

    /// <summary>
    /// Checks if the key exists in the database. This is an asynchronous operation which will call
    /// the specified action on completion.
    /// </summary>
    public void Exists(Action<bool> returnExists)
    {
        Database.Exists(Key, returnExists);
    }

    /// <summary>
    /// Deletes the key from the database. This is an asynchronous operation which will call
    /// the specified action on completion.
    /// </summary>
    public void Delete(Action<bool> returnSuccess=null)
    {
        Database.Delete(Key, returnSuccess);
    }

    /// <summary>
    /// Pulls all entries from the database. This is an asynchronous operation which will call
    /// the specified action on completion.
    /// </summary>
    public void PullEntries()
    {
        foreach (var entry in Entries)
        {
            entry.Pull();
        }
    }

    /// <summary>
    /// Pushes all entries to the database. This is an asynchronous operation which will call
    /// the specified action on completion.
    /// </summary>
    public void PushEntries()
    {
        foreach (var entry in Entries)
        {
            entry.Push();
        }
    }

    /// <summary>
    /// Deletes all entries from the database. This is an asynchronous operation which will call
    /// the specified action on completion.
    /// </summary>
    public void DeleteEntries()
    {
        foreach (var entry in Entries)
        {
            entry.Delete();
        }
    }
}
