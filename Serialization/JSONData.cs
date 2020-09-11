using System.IO;
using UnityEngine;

public class JSONData : ISaveData
{

    string SavePath = Path.Combine(Application.dataPath, "XMLdata.json");

    public void Save(PlayerData player)
    {
        string FileJson = JsonUtility.ToJson(player);
        File.WriteAllText(SavePath, FileJson);
    }


    public PlayerData Load()
    {
        var result = new PlayerData();

        if (!File.Exists(SavePath))
        {
            Debug.Log("File NOT exest!");
            return result;
        }

        string temp = File.ReadAllText(SavePath);

        return JsonUtility.FromJson<PlayerData>(temp);
    }



}
