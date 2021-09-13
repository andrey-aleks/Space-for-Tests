using System;
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
        public static DateTime StartTime;
        public static float TimeForWaiting = 0.5f;

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
                    StartTime = DateTime.Now; // current seconds. It is for waiting
                    EditorApplication.update += WaitAndLoadScenes; // wait for some seconds and load saved scenes
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
            // clear old data
            source.UnloadedScenesPaths.Clear();
            source.ActiveScenePath = "";

            // save active scene path
            source.ActiveScenePath = EditorSceneManager.GetActiveScene().path;
            var sceneCount = EditorSceneManager.sceneCount;

            for (var i = sceneCount - 1; i >= 0; i--)
            {
                // save loaded scene path to List
                var currentScene = EditorSceneManager.GetSceneAt(i);
                source.UnloadedScenesPaths.Add(currentScene.path);
            }

            for (var i = sceneCount - 1; i >= 0; i--)
            {
                // get i-scene from hierarchy
                var scene = EditorSceneManager.GetSceneAt(i);
                // if loader scene, than set active
                if (scene.name.Equals(source.MainSceneName) &&
                    SceneManager.GetSceneByName(source.MainSceneName).isLoaded)
                {
                    EditorSceneManager.SetActiveScene(scene);
                }
                // else just unload this scene
                else
                {
                    EditorSceneManager.UnloadSceneAsync(scene);
                }
            }

            EditorUtility.SetDirty(source); // mask Scriptable Obj as dirty for saving
            AssetDatabase.SaveAssets(); // and save him

            // if there is no loader scene, than load it
            if (!SceneManager.GetSceneByName(source.MainSceneName).IsValid() ||
                !SceneManager.GetSceneByName(source.MainSceneName).isLoaded)
            {
                var mainSceneGuid = AssetDatabase.FindAssets(source.MainSceneName).First();
                var scenePath = AssetDatabase.GUIDToAssetPath(mainSceneGuid);
                EditorSceneManager.OpenScene(scenePath);
            }
        }

        private static void WaitAndLoadScenes()
        {
            if (DateTime.Now.Second - StartTime.Second > TimeForWaiting)
            {
                LoadScenes();
                EditorApplication.update -= WaitAndLoadScenes;
            }
        }

        private static void LoadScenes()
        {
            var source = PlayButtonHelper.Instance;
            // if there is no scenes in list, than return
            if (source.UnloadedScenesPaths.Count < 1)
            {
                return;
            }

            // open scene from list
            foreach (var scenePath in source.UnloadedScenesPaths)
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }

            // if there was some active scene before unload, than set as active 
            if (!source.ActiveScenePath.Equals(""))
            {
                EditorSceneManager.SetActiveScene(EditorSceneManager.GetSceneByPath(source.ActiveScenePath));
            }
        }
    }
}