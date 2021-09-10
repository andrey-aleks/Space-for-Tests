using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;

namespace Tools.PlayButtonExtender
{
    static class ToolbarStyles
    {
        public static readonly GUIStyle commandButtonStyle;

        static ToolbarStyles()
        {
            commandButtonStyle = new GUIStyle("Command")
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold
            };
        }
    }

    [InitializeOnLoad]
    public class PlayButtonEditor : MonoBehaviour
    {
        static PlayButtonEditor()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        }

        static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("P", "Play with only LoaderScene"), ToolbarStyles.commandButtonStyle))
            {
                if (EditorApplication.isPlaying)
                {
                    EditorApplication.ExitPlaymode();
                    LoadScenes();
                }
                else
                {
                    UnloadScenes();
                    EditorApplication.EnterPlaymode();
                }
            }
        }

        private static void UnloadScenes()
        {
            var source = PlayButtonHelper.Instance;
            source.LoadedScenesPaths.Clear();
            source.ActiveScenePath = "";

            source.ActiveScenePath = EditorSceneManager.GetActiveScene().path;
            var sceneCount = EditorSceneManager.loadedSceneCount;

            for (var i = 0; i < sceneCount; i++)
            {
                var currentScene = EditorSceneManager.GetSceneAt(i);
                try
                {
                    EditorBuildSettings.scenes.GetValue(currentScene.buildIndex);
                }
                catch (Exception e)
                {
                    AddSceneToBuildSettings(currentScene);
                }

                source.LoadedScenesPaths.Add(currentScene.path);
            }

            for (var i = sceneCount - 1; i >= 0; i--)
            {
                var scene = EditorSceneManager.GetSceneAt(i);
                if (scene.name.Equals(source.MainSceneName))
                {
                    EditorSceneManager.SetActiveScene(scene);
                }
                else
                {
                    EditorSceneManager.UnloadSceneAsync(scene);
                }
            }

            if (!SceneManager.GetSceneByName(source.MainSceneName).IsValid())
            {
                var mainSceneGuid = AssetDatabase.FindAssets(source.MainSceneName).First();
                var scenePath = AssetDatabase.GUIDToAssetPath(mainSceneGuid);
                EditorSceneManager.OpenScene(scenePath);
            }
            foreach (var scene in source.LoadedScenesPaths)
            {
                Debug.Log(scene + " path 1");
            }
        }

        private static void AddSceneToBuildSettings(Scene scene)
        {
            var editorBuildSettingsScenes = EditorBuildSettings.scenes.ToList();
            editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scene.path, true));

            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
        }


        [MenuItem("CROC/Load Saved Scenes#_g")]
        private static void LoadScenes()
        {
           
            var source = PlayButtonHelper.Instance;

            Debug.Log(source.LoadedScenesPaths.Count + " scenes");
            foreach (var scene in source.LoadedScenesPaths)
            {
                Debug.Log(scene + " path 2");
            }

            if (source.LoadedScenesPaths.Count < 1)
            {
                return;
            }


            var scenes = new List<SceneAsset>();
            foreach (var buildSettingsScene in EditorBuildSettings.scenes)
            {
                scenes.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>(buildSettingsScene.path));
                //scenes.Add(EditorSceneManager.GetSceneByPath()); // не работает, потому что он берет из Hierarchy
            }

            foreach (var scene in scenes)
            {
                Debug.Log("name " + scene.name);
            }

            foreach (var scenePath in source.LoadedScenesPaths)
            {
                var name = Path.GetFileNameWithoutExtension(scenePath);
                Debug.Log(name + " is name");

                //EditorSceneManager.LoadSceneAsync(name);
                
                //SceneManager.LoadScene(name, LoadSceneMode.Additive);
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

            }

            if (!source.ActiveScenePath.Equals(""))
            {
                EditorSceneManager.SetActiveScene(EditorSceneManager.GetSceneByPath(source.ActiveScenePath));
            }
        }
    }

    static class SceneHelper
    {
        public static List<Scene> Scenes = new List<Scene>();
        public static int ActiveSceneIndex;
    }
}