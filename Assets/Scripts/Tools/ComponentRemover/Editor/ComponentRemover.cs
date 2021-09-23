using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ComponentRemover.Editor
{
    public class ComponentRemover : EditorWindow
    {
        public GameObject targetObject;
        public UnityEngine.Object componentObject;
        public SearchTypes searchType;
        public string componentName;

        private readonly string NAME = "[ComponentRemover] ";


        [MenuItem("CROC/Editor Tools/Component Remover")]
        public static void ShowWindow()
        {
            GetWindow<ComponentRemover>("Component Remover");
        }

        private void OnGUI()
        {
            GUILayout.Label("Select a search type");
            searchType = (SearchTypes) EditorGUILayout.EnumPopup("Search type ", searchType);
            targetObject = EditorGUILayout.ObjectField("Object", targetObject, typeof(GameObject), true) as GameObject;
            if (searchType == SearchTypes.ByObject)
            {
                GUILayout.Label("Drag and drop necessary object (component, but scripts mainly)");
                componentObject = EditorGUILayout.ObjectField("Component to Remove", componentObject, typeof(UnityEngine.Object),
                    false) as UnityEngine.Object;
            }

            if (searchType == SearchTypes.ByName)
            {
                GUILayout.Label("Enter name of the component to remove");
                componentName = EditorGUILayout.TextField("Component name", componentName);
            }

            if (GUILayout.Button("Start removing"))
            {
                switch (searchType)
                {
                    case SearchTypes.ByObject:
                        RemoveComponents(componentObject.name);
                        break;
                    case SearchTypes.ByName:
                        RemoveComponents(componentName);
                        break;
                }
            }

            GUILayout.Label("Don't forget to apply changes to prefab");
        }

        private void RemoveComponents(string targetComponentName)
        {
            if ((targetObject != null) && (!string.IsNullOrEmpty(targetComponentName)))
            {
                var components = new List<Component>(targetObject.GetComponentsInChildren<Component>().ToList());
                if (!components.Any())
                {
                    Debug.Log($"{NAME}there is no components at {targetObject.name} and his children");
                    return;
                }

                for (int i = 0; i < components.Count; i++)
                {
                    var childComponentName = components[i].GetType().Name; // split because it returns UnityEngine.{Type}
                    if (childComponentName.Equals(targetComponentName))
                    {
                        var childObjectName = components[i].name;
                        DestroyImmediate(components[i]);
                        Debug.Log(NAME + childComponentName + " at " + childObjectName + " was REMOVED");
                    }
                }
                
                // mark prefab as dirty
                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage != null)
                {
                    EditorSceneManager.MarkSceneDirty(prefabStage.scene);
                }
            }

            else
            {
                Debug.Log(NAME + "Target object or component is empty");
            }
        }
    }

    public enum SearchTypes
    {
        ByName,
        ByObject
    }
}