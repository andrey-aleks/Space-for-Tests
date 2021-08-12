using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Importer.Editor
{
    [CreateAssetMenu(fileName = "CustomImporterSettings", menuName = "Custom Tools/Custom Importer Settings", order = 1010)]
    public class ImporterSettings : ScriptableObject
    {
        //[Tooltip(@"Should tool process drag&dropped files or not?")]
        //public bool processDragAndDrop = false;

        [Tooltip(@"Existing project folder with '\' at the end. Example: Assets\Models\")]
        public string parentFolder = @"Assets\Models\";

        public string baseColorPostfix = "BC";

        [Tooltip(@"Property name for the current texture in the Shader")]
        public string baseColorMapName = "_BaseMap";

        public string normalPostfix = "N";

        [Tooltip(@"Property name for the current texture in the Shader")]
        public string normalMapName = "_BumpMap";

        public string metallicPostfix = "MS";

        [Tooltip(@"Property name for the current texture in the Shader")]
        public string metallicMapName = "_MetallicGlossMap";

        public string occlusionPostfix = "AO";

        [Tooltip(@"Property name for the current texture in the Shader")]
        public string occlusionMapName = "_OcclusionMap";
        
        public string tileMaterialPostfix = "T";

        [Tooltip(@"Texture format with '.' at start. Example: .png")]
        public List<string> textureFormats = new List<string>() {".png"};

        [Tooltip(@"Should importer overwrite existing files?")]
        public bool enableOverwrite = false;


        private static readonly string NAME = "[ImporterSettings]: ";
        private static ImporterSettings _instance;
        public static ImporterSettings Instance => _instance ?? (_instance = LoadAsset());

        private static ImporterSettings LoadAsset([CallerFilePath] string callerFilepath = null)
        {
            var path = @"Assets\Data\Tools\CustomImporterSettings.asset";
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


            var paths = AssetDatabase.FindAssets("t:ImporterSettings");
            if (paths.Length > 0)
            {
                path = AssetDatabase.GUIDToAssetPath(paths[0]);
            }

            var asset = AssetDatabase.LoadAssetAtPath<ImporterSettings>(path);
            if (asset == null)
            {
                asset = CreateInstance<ImporterSettings>();
                AssetDatabase.CreateAsset(asset, path);
                Debug.Log($@"{NAME}CustomImporterSettings created at path {path}");
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