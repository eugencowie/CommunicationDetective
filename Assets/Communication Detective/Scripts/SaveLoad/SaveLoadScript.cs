using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoadScript{

    //public static List<ObjectHintData> savedGames = new List<ObjectHintData>();
	
    public static void Save()
    {
        //savedGames.Add(ObjectHintData.current);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd");
        bf.Serialize(file, StaticInventory.Hints);
        file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
            StaticInventory.Hints = (List<ObjectHintData>)bf.Deserialize(file);
            file.Close();
        }
    }
}
