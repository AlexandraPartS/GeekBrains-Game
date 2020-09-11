using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathWaypoint))]
public class XMLSavingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PathWaypoint Wpath = (PathWaypoint)target;
        SaveObject _SaveObject = new SaveObject();      //Класс взяли, чтобы им воспользоваться

        if(GUILayout.Button("Сохранить"))
        {
            SaveObject.SaveScene();
        }

        if (GUILayout.Button("Загрузить"))
        {
            SaveObject.LoadScene();
        }
    }





}
