using System;

/// <summary>
/// Represents a key-value pair in the database.
/// </summary>
public class DatabaseEntry
{
    /// <summary>
    /// Reference to the database.
    /// </summary>
    private readonly Database m_database;

    /// <summary>
    /// The full path to the key in the database.
    /// </summary>
    public readonly string Key;

    /// <summary>
    /// The value of the entry.
    /// </summary>
    public string Value;

    public DatabaseEntry(Database database, string key)
    {
        m_database = database;
        Key = key;
        Value = "";
    }

    #region Exists

    public void Exists(Action<bool> returnExists)
    {
        m_database.Exists(Key, returnExists);
    }

    public void Exists(Action onExists, Action onNotExists=null)
    {
        m_database.Exists(Key, onExists, onNotExists);
    }

    #endregion

    #region Pull

    public void Pull(Action<bool> returnSuccess=null)
    {
        Database.ValidateAction(ref returnSuccess);

        Exists(exists => {
            if (!exists) returnSuccess(false);
            else m_database.Pull(Key, result => {
                Value = result;
                returnSuccess(true);
            });
        });
    }

    public void Pull(Action onSuccess, Action onFailure = null)
    {
        Database.ValidateAction(ref onSuccess);
        Database.ValidateAction(ref onFailure);

        Pull(success => {
            if (success) onSuccess();
            else onFailure();
        });
    }

    #endregion

    #region Push

    public void Push(Action<bool> returnSuccess=null)
    {
        m_database.Push(Key, Value, returnSuccess);
    }

    public void Push(Action onSuccess, Action onFailure=null)
    {
        m_database.Push(Key, Value, onSuccess, onFailure);
    }

    #endregion

    #region Delete

    public void Delete(Action<bool> returnSuccess=null)
    {
        m_database.Push(Key, null, returnSuccess);
    }

    public void Delete(Action onSuccess, Action onFailure=null)
    {
        m_database.Push(Key, null, onSuccess, onFailure);
    }

    #endregion
}
