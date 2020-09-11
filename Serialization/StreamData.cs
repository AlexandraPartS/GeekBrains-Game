using System;
using System.IO;
using UnityEngine;

public class StreamData : ISaveData
{

    //string SavePath = Path.Combine(Application.dataPath, "XMLdata.json");
    string SavePath = Path.Combine(Application.dataPath, "StreamData.XYZ");

    public void Save(PlayerData player)
    {
        using (StreamWriter _writer = new StreamWriter(SavePath))
        {
            _writer.WriteLine(player.PLName);
            _writer.WriteLine(player.PLHealth);
            _writer.WriteLine(player.PLDead);
        }
    }


    public PlayerData Load()
    {
        var result = new PlayerData();

        if (!File.Exists(SavePath))
        {
            Debug.Log("File NOT exest!");
            return result;
        }

        using (StreamReader _reader = new StreamReader(SavePath))
        {
            result.PLName = _reader.ReadLine();
            result.PLHealth = Convert.ToInt32(_reader.ReadLine());
            result.PLDead = Convert.ToBoolean(_reader.ReadLine());
        }
        return result;
    }



}
