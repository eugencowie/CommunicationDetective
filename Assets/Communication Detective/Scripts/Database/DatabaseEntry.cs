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

    public void Exists(Action<bool> returnExists)
    {
        m_database.Exists(Key, returnExists);
    }

    public void Pull(Action<bool> returnSuccess=null)
    {
        Database.ValidateAction(ref returnSuccess);

        m_database.Pull(Key, result => {
            if (result != null) {
                Value = result;
                returnSuccess(true);
            }
            else returnSuccess(false);
        });
    }

    public void Push(Action<bool> returnSuccess=null)
    {
        m_database.Push(Key, Value, returnSuccess);
    }

    public void Delete(Action<bool> returnSuccess=null)
    {
        m_database.Delete(Key, returnSuccess);
    }
}
