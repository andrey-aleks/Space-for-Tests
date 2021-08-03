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
        [Tooltip(@"Project folder with '\' at the end")]
        public string parentFolder = @"Assets\Models\";

        public string baseColorPostfix = "BC";
        public string baseColorMapName = "_BaseMap";
        
        public string normalPostfix = "N";
        public string normalMapName = "_BumpMap";
        
        public string metallicPostfix = "M";
        public string metallicMapName = "_MetallicGlossMap";
        
        public string occlusionPostfix = "AO";
        public string occlusionMapName = "_OcclusionMap";
        
        
        private static ImportSettings _instance;
        public static ImportSettings Instance => _instance ?? (_instance = LoadAsset());

        private static ImportSettings LoadAsset([CallerFilePath] string callerFilepath = null)
        {
            var path = GetAssetPath();
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
            folder = folder.Substring(folder.LastIndexOf("\\Assets\\", StringComparison.Ordinal) + 1);
            return Path.Combine(folder, "CustomImportSettings.asset");
        }
        


    }
}
