using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = System.Object;

public class MaterialAdder : EditorWindow
{
    public MonoScript script;
    public Material newMaterial;

    private List<object> _teleportObjects;

    [MenuItem("Tools/MaterialAdder")]
    public static void ShowWindow()
    {
        GetWindow<MaterialAdder>("MaterialAdder", true);
    }

    public void OnGUI()
    {
        newMaterial = (Material) EditorGUILayout.ObjectField("Material", newMaterial, typeof(Material));
        script = (MonoScript) EditorGUILayout.ObjectField("Script", script, typeof(MonoScript));
        if (GUILayout.Button("Add"))
        {
           // FindObjects();
           FindAllActiveObjects();
        }
    }

    private void FindAllActiveObjects()
    {
        GameObject[] objects = FindObjectsOfType<GameObject>();
        foreach (var _object in objects)
        {
            if (_object.activeInHierarchy)
            {
                Debug.Log(_object.name);
            }
        }
    }

    /*    private void FindObjects()
    {
        var info = new DirectoryInfo("D:/ProjectsUnity/TestingURP/Assets/Models");
        var fileInfo = info.GetFiles();
        for (int i = 0; i < fileInfo.Length; i++)
        {
            _teleportObjects.Add(fileInfo.GetValue(i));
        }
        foreach (var file in fileInfo)
        {
            var fileName = file.Name;
            if (!fileName.Split('_').Last().Contains("Tel") || (fileName.Contains(".meta")))
            {
                _teleportObjects.Remove(file);
            }
        }
        AddComponents();
    }

    private void AddComponents()
    {
        foreach (var _object in _teleportObjects)
        {
            
        }
        Debug.Log(fileName);
        fileName = fileName.Substring(0, fileName.Length - 4);
        Debug.Log(fileName);
        GameObject gameObject = GameObject.Find(fileName);
        if (gameObject != null)
        {
            if (gameObject.GetComponent(script.GetClass()) != null ||
                gameObject.GetComponent<Renderer>().material == newMaterial)
            {
                return;
            }

            gameObject.GetComponent<MeshRenderer>().material = newMaterial;
            gameObject.AddComponent(script.GetClass());
        }
        
    }
    */
    
}