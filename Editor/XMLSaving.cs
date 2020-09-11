using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public struct SVect3
{
    public float X;
    public float Y;
    public float Z;
    public SVect3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    public static implicit operator SVect3(Vector3 val)
    {
        return new SVect3(val.x, val.y, val.z);
    }
    public static implicit operator Vector3(SVect3 val)
    {
        return new Vector3(val.X, val.Y, val.Z);
    }
}

[Serializable]
public struct SQuater
{
    public float X;
    public float Y;
    public float Z;
    public float W;
    public SQuater(float x, float y, float z, float w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }
    public static implicit operator SQuater(Quaternion val)
    {
        return new SQuater(val.x, val.y, val.z, val.w);
    }
    public static implicit operator Quaternion(SQuater val)
    {
        return new Quaternion(val.X, val.Y, val.Z, val.W);
    }
}
[Serializable]
public struct SObject
{
    public string Name;
    public SVect3 Position;
    public SVect3 Scale;
    public SQuater Rotation;
}
//Class for saving GO
public class SaveObject
{
    private static XmlSerializer _formatter;
    public static string SavePath = Path.Combine(Application.dataPath, "EditorDataXML.xml");
    static SaveObject()
    {
        _formatter = new XmlSerializer(typeof(SObject[]));
    }

    [MenuItem("Инструменты/ Сохранение шаблонов/ Сохранить шаблон сцены", false, 1)]
    public static void SaveScene()
    {
        string SavePath = Path.Combine(Application.dataPath, "EditorDataXML.xml"); //путь для сохранения. Задаем ерез string. 2 части пути: путь до папки, где раб.файлы игры, и название файла. Решение: можно прописать через "+", а можно утилитой Path и методом combine

        //Scene AllObScene = SceneManager.GetActiveScene();
        GameObject AllObjScene = GameObject.FindObjectOfType<PathWaypoint>().gameObject;
        List<GameObject> rootObject = new List<GameObject>();
        //AllObScene.GetRootGameObjects(rootObject);
        foreach(Transform item in AllObjScene.transform)
        {
            rootObject.Add(item.gameObject);
        }
        List<SObject> LevelObject = new List<SObject>();
        foreach (var item in rootObject)
        {
            var temp = item.transform;

            LevelObject.Add(
                new SObject
                {
                    Name = temp.name,
                    Position = temp.position,
                    Rotation = temp.rotation,
                    Scale = temp.localScale
                });
        }
        //Сохранение. Проверка - все ли хорошо:
        if (LevelObject.Count <= 0 || String.IsNullOrEmpty(SavePath))
        {
            Debug.Log("Не задан путь или массив пуст");
        }
        using (FileStream fs = new FileStream(SavePath, FileMode.Create)) //Сохранение черз стандартную конструкцию FileStream
        {
            _formatter.Serialize(fs, LevelObject.ToArray());
        }
    }

    [MenuItem("Инструменты/ Сохранение шаблонов/ Загрузить шаблон сцены", false, 1)]
    public static void LoadScene()
    {
        SObject[] result;
        using (FileStream fs = new FileStream(SavePath, FileMode.Open))
        {
            result = (SObject[])_formatter.Deserialize(fs);
        }

        var _prefabMain = new GameObject("Waypoints");
        _prefabMain.AddComponent<PathWaypoint>();


        foreach (var item in result)
        {
            var _prefab = new GameObject(item.Name);
            _prefab.transform.position = item.Position;
            _prefab.transform.localScale = item.Scale;
            _prefab.transform.rotation = item.Rotation;
            _prefab.transform.parent = _prefabMain.transform;
        }

    }
}


    public class XMLSaving : MonoBehaviour
{

}
