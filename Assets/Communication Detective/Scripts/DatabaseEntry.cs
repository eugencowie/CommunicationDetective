using System;

public class DatabaseEntry
{
    private readonly Database m_database;

    public readonly string Key;
    public string Value;

    public DatabaseEntry(Database database, string key)
    {
        m_database = database;
        Key = key;
        Value = "";
    }

    public void ExistsAsync(Action<bool> returnExists)
    {
        m_database.ExistsAsync(Key, returnExists);
    }

    public void PullAsync(Action<bool> returnSuccess=null)
    {
        if (returnSuccess == null) returnSuccess = (_ => { });

        ExistsAsync(exists => {
            if (!exists) returnSuccess(false);
            else m_database.PullAsync(Key, result => {
                Value = result;
                returnSuccess(true);
            });
        });
    }

    public void PushAsync(Action<bool> returnSuccess=null)
    {
        m_database.PushAsync(Key, Value, returnSuccess);
    }

    public void DeleteAsync(Action<bool> returnSuccess=null)
    {
        m_database.PushAsync(Key, null, returnSuccess);
    }
}
