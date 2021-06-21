#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Utilities
{
    public class ComponentRemover : EditorWindow
    {
        public GameObject targetObject;
        public UnityEngine.Object component;
        public SearchTypes searchType;
        public string componentName;
        
        private const string NameForDebug = "[ComponentRemover] ";
        private List<Component> _components;
    
    
        [MenuItem("Tools/Custom Tools/ComponentRemover")]
        public static void ShowWindow()
        {
            GetWindow<ComponentRemover>();
        }

        private void OnGUI()
        {
            GUILayout.Label("Select a search type");
            searchType = (SearchTypes)EditorGUILayout.EnumPopup("Search type ", searchType);
            targetObject = EditorGUILayout.ObjectField("Object", targetObject, typeof(GameObject), true) as GameObject;
            if (searchType == SearchTypes.ByObject)
            {
                GUILayout.Label("Drag and drop necessary object (component)");
                component = EditorGUILayout.ObjectField("Component to Remove", component, typeof(UnityEngine.Object),
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
                        RemoveComponentsByObject();
                        break;
                    case SearchTypes.ByName:
                        RemoveComponentsByName();
                        break;
                }
            }
            GUILayout.Label("Don't forget to apply changes to prefab");
        }

        private void RemoveComponentsByName()
        {
            if ((targetObject != null) && (componentName != ""))
            {
                _components = targetObject.GetComponentsInChildren<Component>().ToList();

                for (int i = 0; i < _components.Count; i++)
                {
                    var _componentName = _components[i].GetType().ToString().Split('.').Last();
                    Debug.Log(_componentName);
                    if (_componentName.Equals(componentName))
                    {
                       // if (!_components[i].IsDestroyed())
                       // {
                            Debug.Log(NameForDebug + _componentName + " at " + _components[i].name + " was REMOVED");
                            DestroyImmediate(_components[i]);
                       // }
                    }
                }
            }
            else
            {
                Debug.Log(NameForDebug + "Target object or component name is null");
            }
            
        }

        private void RemoveComponentsByObject()
        {
            if ((targetObject != null) && (component != null))
            {
                _components = targetObject.GetComponentsInChildren<Component>().ToList();

                for (int i = 0; i < _components.Count; i++)
                {
                    var _componentName = _components[i].GetType().ToString().Split('.').Last();
                    Debug.Log(_componentName);
                    if (_componentName.Equals(component.name))
                    {
                      //  if (!_components[i].IsDestroyed())
                      //  {
                            Debug.Log(NameForDebug + _componentName + " at " + _components[i].name + " was REMOVED");
                            DestroyImmediate(_components[i]);
                      //  }
                    }
                }
            }
            else
            {
                Debug.Log(NameForDebug + "Target object or component is null");
            }
        }
    }

    public enum SearchTypes
    {
        ByName,
        ByObject
    }
}
#endif