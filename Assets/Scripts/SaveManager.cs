using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager
{
    public static void SavePlayer(Saving manager)
    {   
        BinaryFormatter formatter = new BinaryFormatter();
        
        string path = Application.persistentDataPath + "/saves.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(manager);
        
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/saves.fun";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            
            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            
            return data;
        }
        else
        {
            Debug.LogError("Save file doesn't exist");
            return null;
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public double lastTime;
    public float[] cameraPosition;

    public PlayerData(Saving manager)
    {
        lastTime = manager.curTime;
    }
}
