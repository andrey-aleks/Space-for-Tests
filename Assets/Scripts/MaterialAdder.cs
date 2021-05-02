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
        if (objects.Length > 0)
        {
            foreach (var _object in objects)
            {
                if (_object.activeInHierarchy)
                {
                    if (_object.name.Split('_').Last().Contains("Tel"))
                    {
                        Debug.Log(_object.name);
                        AddComponents(_object);
                        
                    }
                }
            }
        }
        else
        {
            Debug.Log("[MaterialAdder] There is no active objects at active scene");
        }
    }

    private void AddComponents(GameObject currentGameObject)
    {   
            if (currentGameObject.GetComponent(script.GetClass()) != null)
            {
                currentGameObject.GetComponent<MeshRenderer>().material = newMaterial;
                Debug.Log("[MaterialAdder] "+currentGameObject.name + " mat added");
                return;
            }
            
            currentGameObject.AddComponent(script.GetClass());
            currentGameObject.GetComponent<MeshRenderer>().material = newMaterial;
            Debug.Log("[MaterialAdder] "+currentGameObject.name + " mat & script added");
    }
}