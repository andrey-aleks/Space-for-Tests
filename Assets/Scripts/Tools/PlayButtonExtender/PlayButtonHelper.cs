using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using Scene = UnityEngine.SceneManagement.Scene;

namespace Tools.PlayButtonExtender
{
    [CreateAssetMenu(fileName = "PlayButtonHelper", menuName = "CROC/Artist Tools/PlayButtonHelper", order = 1020)]
    public class PlayButtonHelper : ScriptableObject
    {
        public List<string> LoadedScenesPaths = new List<string>();
        public string ActiveScenePath;
        public string MainSceneName = "LoaderScene";
        
        private static readonly string NAME = "[PlayButtonHelper]: ";
        private static PlayButtonHelper _instance;
        public static PlayButtonHelper Instance => _instance ?? (_instance = LoadAsset());

        private static PlayButtonHelper LoadAsset([CallerFilePath] string callerFilepath = null)
        {
            var path = @"Assets\Data\Tools\PlayButtonHelper.asset";
            if (!AssetDatabase.IsValidFolder(path.Remove(path.LastIndexOf('\\'))))
            {
                var tempPath = path.Remove(path.LastIndexOf('\\'));
                if (!AssetDatabase.IsValidFolder(tempPath.Remove(tempPath.LastIndexOf('\\'))))
                {
                    AssetDatabase.CreateFolder(@"Assets", "Data");
                    AssetDatabase.CreateFolder(@"Assets\Data", "Tools");
                }
                else
                {
                    AssetDatabase.CreateFolder(@"Assets\Data", "Tools");
                }
            }


            var paths = AssetDatabase.FindAssets("t:PlayButtonHelper");
            if (paths.Length > 0)
            {
                path = AssetDatabase.GUIDToAssetPath(paths[0]);
            }

            var asset = AssetDatabase.LoadAssetAtPath<PlayButtonHelper>(path);
            if (asset == null)
            {
                asset = CreateInstance<PlayButtonHelper>();
                AssetDatabase.CreateAsset(asset, path);
                Debug.Log($@"{NAME}PlayButtonHelper created at path {path}");
                AssetDatabase.SaveAssets();
            }

            return asset;
        }
    }
}
