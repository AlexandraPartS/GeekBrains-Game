using System.Collections;
using UnityEditor;
using UnityEngine;

public class Window : EditorWindow
{
    public GameObject botPref;
    public int objectCounter;
    public float radius = 20;

    [MenuItem("Инструменты/ Создание префабов/ Генератор ботов")]
    public static void ShowWindow()
    {
        GetWindow(typeof(Window), false, "Генератор ботов");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Настройки", EditorStyles.boldLabel);
        botPref = EditorGUILayout.ObjectField("", botPref, typeof(GameObject), true) as GameObject;
        objectCounter = EditorGUILayout.IntSlider("", objectCounter, 3, 200);
        radius = EditorGUILayout.Slider("", radius, 10, 100);

        if(GUILayout.Button("Создать"))
        {
            if(botPref)
            { 
                GameObject Main = new GameObject("MainBot");
                for(int i=0; i<objectCounter; i++ )
                {
                    float angle = i* 2 * Mathf.PI / objectCounter;
                    Vector3 pos = (new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle))*radius);
                    GameObject temp = Instantiate(botPref, pos, Quaternion.identity);
                    temp.transform.parent = Main.transform;
                    temp.name = "("+i+")";
                }
            }
        }
    }




}
