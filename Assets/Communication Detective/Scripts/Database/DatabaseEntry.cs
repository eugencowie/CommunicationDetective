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

    /// <summary>
    /// Initialises the database entry.
    /// </summary>
    public DatabaseEntry(Database database, string key)
    {
        m_database = database;
        Key = key;
        Value = "";
    }

    /// <summary>
    /// Checks if the key exists in the database. This is an asynchronous operation which will call
    /// the specified action on completion.
    /// </summary>
    public void Exists(Action<bool> returnExists)
    {
        m_database.Exists(Key, returnExists);
    }

    /// <summary>
    /// Pulls the value from the database. This is an asynchronous operation which will call the
    /// specified action on completion.
    /// </summary>
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

    /// <summary>
    /// Pushes the value to the database. This is an asynchronous operation which will call the
    /// specified action on completion.
    /// </summary>
    public void Push(Action<bool> returnSuccess=null)
    {
        m_database.Push(Key, Value, returnSuccess);
    }

    /// <summary>
    /// Deletes the key from the database. This is an asynchronous operation which will call
    /// the specified action on completion.
    /// </summary>
    public void Delete(Action<bool> returnSuccess=null)
    {
        m_database.Delete(Key, returnSuccess);
    }
}
