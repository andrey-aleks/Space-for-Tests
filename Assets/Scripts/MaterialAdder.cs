using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Object = System.Object;

public class MaterialAdder : EditorWindow
{
    public MonoScript script;
    public Material material;
    public string postfix;

    [MenuItem("Tools/MaterialAdder")]
    public static void ShowWindow()
    {
        GetWindow<MaterialAdder>("MaterialAdder", true);
    }

    public void OnGUI()
    {
        material = (Material) EditorGUILayout.ObjectField("Material to add", material, typeof(Material));
        script = (MonoScript) EditorGUILayout.ObjectField("Script to add", script, typeof(MonoScript));
        postfix = EditorGUILayout.TextField("Postfix without '_'", postfix);
        if (GUILayout.Button("Add"))
        {
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
                    if (_object.name.Split('_').Last().Contains(postfix))
                    {
                        AddComponents(_object);
                    }
                }
            }
        }
        else
        {
            Debug.Log("[MaterialAdder] There is no objects at scene");
        }
    }

    private void AddComponents(GameObject currentGameObject)
    {
        Component addedScript = currentGameObject.GetComponent(script.GetClass());

        if (addedScript != null && currentGameObject.GetComponent<Renderer>().sharedMaterial == material)
        {
            return;
        }

        if (addedScript != null)
        {
            currentGameObject.GetComponent<Renderer>().sharedMaterial = material;
            Debug.Log("[MaterialAdder] " + currentGameObject.name + " mat added");
            return;
        }

        currentGameObject.AddComponent(script.GetClass());
        currentGameObject.GetComponent<MeshRenderer>().sharedMaterial = material;
        Debug.Log("[MaterialAdder] " + currentGameObject.name + " mat & script added");
    }
}