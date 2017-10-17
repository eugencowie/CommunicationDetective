using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using UnityEngine;

/// <summary>
/// Provides direct low-level access to the Firebase database.
/// </summary>
public class Database
{
    /// <summary>
    /// Reference to the Firebase database root node.
    /// </summary>
    private DatabaseReference m_root;

    public Database()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://communication-detective.firebaseio.com");
        m_root = FirebaseDatabase.DefaultInstance.RootReference;
    }
    
    /// <summary>
    /// Checks if data exists in the database. This is an asynchronous operation which will call
    /// the specified action on completion.
    /// </summary>
    public void Exists(string path, Action<bool> returnExists)
    {
        ValidateAction(ref returnExists);

        m_root.Child(path).GetValueAsync().ContinueWith(t => {
            returnExists(t.Result.Exists);
        });
    }
    
    /// <summary>
    /// Pulls data from the database. This is an asynchronous operation which will call the
    /// specified action on completion.
    /// </summary>
    public void Pull(string path, Action<string> returnResult)
    {
        ValidateAction(ref returnResult);

        m_root.Child(path).GetValueAsync().ContinueWith(t => {
            if (t.Result.Exists) returnResult(t.Result.Value.ToString());
            else returnResult(null);
        });
    }
    
    /// <summary>
    /// Pushes data to the database. This is an asynchronous operation which will call the
    /// specified action on completion.
    /// </summary>
    public void Push(string path, string data, Action<bool> returnSuccess=null)
    {
        ValidateAction(ref returnSuccess);

        m_root.Child(path).SetValueAsync(data).ContinueWith(t => {
            returnSuccess(t.IsCompleted);
        });
    }
    
    /// <summary>
    /// Deletes data from the database. This is an asynchronous operation which will call
    /// the specified action on completion.
    /// </summary>
    public void Delete(string path, Action<bool> returnSuccess=null)
    {
        Push(path, null, returnSuccess);
    }
    
    public void RegisterListener(string path, EventHandler<ValueChangedEventArgs> listener)
    {
        FirebaseDatabase.DefaultInstance.GetReference(path).ValueChanged += listener;
    }

    public void DeregisterListener(string path, EventHandler<ValueChangedEventArgs> listener)
    {
        FirebaseDatabase.DefaultInstance.GetReference(path).ValueChanged -= listener;
    }
    
    public static void ValidateAction<T>(ref Action<T> action)
    {
        action = action ?? (_ => {});
    }
    
    /// <summary>
    /// Tests to check if the database is working properly.
    /// </summary>
    public static void RunTests()
    {
        Database database = new Database();

        database.Exists("test/does/not/exist", exists => {
            Debug.Assert(exists == false);
        });

        database.Push("test/data/key", "value", success => {
            Debug.Assert(success == true);
            database.Exists("test/data", exists => {
                Debug.Assert(exists == true);
                database.Exists("test/data/key", keyExists => {
                    Debug.Assert(keyExists == true);
                    database.Pull("test/data/key", result => {
                        Debug.Assert(result == "value");
                    });
                });
            });
        });
    }
}
