using System;

/// <summary>
/// Represents a node in the database which contains a collection of database entries.
/// </summary>
public abstract class DatabaseNode
{
    /// <summary>
    /// Reference to the database.
    /// </summary>
    protected readonly Database Database;
    
    /// <summary>
    /// The full path to the node in the database.
    /// </summary>
    public readonly string Key;

    public DatabaseNode(Database database, string key)
    {
        Database = database;
        Key = key;
    }

    #region Exists

    public void Exists(Action<bool> returnExists)
    {
        Database.Exists(Key, returnExists);
    }

    public void Exists(Action onExists, Action onNotExists=null)
    {
        Database.Exists(Key, onExists, onNotExists);
    }

    #endregion

    #region Delete

    public void Delete(Action<bool> returnSuccess=null)
    {
        Database.Delete(Key, returnSuccess);
    }

    public void Delete(Action onSuccess, Action onFailure=null)
    {
        Database.Delete(Key, onSuccess, onFailure);
    }

    #endregion
}
