using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using UnityEngine;

public class Database
{
    private DatabaseReference m_root;

    public Database()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://communication-detective.firebaseio.com");
        m_root = FirebaseDatabase.DefaultInstance.RootReference;
    }

    /// <summary>
    /// Pushes data to the database. This is an asynchronous operation.
    /// </summary>
    public void PushAsync(string path, string data)
    {
        m_root.Child(path).SetValueAsync(data);
    }

    /// <summary>
    /// Pushes data to the database. This is an asynchronous operation which will call the
    /// specified action on completion.
    /// </summary>
    public void PushAsync(string path, string data, Action<bool> returnSuccess)
    {
        m_root.Child(path).SetValueAsync(data).ContinueWith(t => {
            returnSuccess(t.IsCompleted);
        });
    }

    /// <summary>
    /// Deletes data from the database. This is an asynchronous operation.
    /// </summary>
    public void DeleteAsync(string path)
    {
        PushAsync(path, null);
    }

    /// <summary>
    /// Deletes data from the database. This is an asynchronous operation which will call
    /// the specified action on completion.
    /// </summary>
    public void DeleteAsync(string path, Action<bool> returnSuccess)
    {
        PushAsync(path, null, returnSuccess);
    }

    /// <summary>
    /// Pulls data from the database. This is an asynchronous operation which will call the
    /// specified action on completion.
    /// </summary>
    public void PullAsync(string path, Action<string> returnResult)
    {
        m_root.Child(path).GetValueAsync().ContinueWith(t => {
            returnResult(t.Result.Value.ToString());
        });
    }

    /// <summary>
    /// Checks if data exists in the database. This is an asynchronous operation which will call
    /// the specified action on completion.
    /// </summary>
    public void ExistsAsync(string path, Action<bool> returnExists)
    {
        m_root.Child(path).GetValueAsync().ContinueWith(t => {
            returnExists(t.Result.Exists);
        });
    }

    public void RegisterListener(string path, EventHandler<ValueChangedEventArgs> listener)
    {
        FirebaseDatabase.DefaultInstance.GetReference(path).ValueChanged += listener;
    }

    public void DeregisterListener(string path, EventHandler<ValueChangedEventArgs> listener)
    {
        FirebaseDatabase.DefaultInstance.GetReference(path).ValueChanged -= listener;
    }

    /// <summary>
    /// Tests to check if the database is working properly.
    /// </summary>
    public static void RunTests()
    {
        Database database = new Database();

        database.ExistsAsync("test/does/not/exist", exists => {
            Debug.Assert(exists == false);
        });

        database.PushAsync("test/data/key", "value", success => {
            Debug.Assert(success == true);
            database.ExistsAsync("test/data", exists => {
                Debug.Assert(exists == true);
                database.ExistsAsync("test/data/key", keyExists => {
                    Debug.Assert(keyExists == true);
                    database.PullAsync("test/data/key", result => {
                        Debug.Assert(result == "value");
                    });
                });
            });
        });
    }
}
