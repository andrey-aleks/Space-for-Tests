using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Utilities.Importer
{
    [CreateAssetMenu(fileName = "CustomImportSettings", menuName = "Custom Import Settings", order = 1010)]
    public class ImportSettings : ScriptableObject
    {
        public string regex;
        [Tooltip(@"Project folder with '\' at the end")]
        public string parentFolder;
        
        private static ImportSettings _instance;
        public static ImportSettings Instance => _instance ?? (_instance = LoadAsset());

        private static ImportSettings LoadAsset([CallerFilePath] string callerFilepath = null)
        {
            var path = GetAssetPath();
            Debug.Log("path: " + path);
            var asset = AssetDatabase.LoadAssetAtPath<ImportSettings>(path);

            if (asset == null)
            {
                asset = CreateInstance<ImportSettings>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
            }

            return asset;
        }
        
        private static string GetAssetPath([CallerFilePath] string callerFilePath = null)
        {
            var folder = Path.GetDirectoryName(callerFilePath);
            Debug.Log("path wo " + folder);
            folder = folder.Substring(folder.LastIndexOf("\\Assets\\", StringComparison.Ordinal) + 1);
            return Path.Combine(folder, "CustomImportSettings.asset");
        }
        


    }
}
