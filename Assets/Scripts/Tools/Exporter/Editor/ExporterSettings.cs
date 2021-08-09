using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Exporter.Editor
{
    [CreateAssetMenu(fileName = "CustomExporterSettings", menuName = "Custom Tools/Custom Exporter Settings", order = 1010)]
    public class ExporterSettings : ScriptableObject
    {
        [Tooltip(@"File formats that are available to export")]
        public List<string> exportFileFormats = new List<string>() {"fbx", "FBX"};

        [Tooltip(@"Texture formats that are available to export")]
        public List<string> exportTextureFormats = new List<string>()
            {"png", "jpg", "jpeg", "tif", "tiff", "targa", "tga"};

        [Tooltip(@"Enables auto-adding prefix tex_ to exported textures")]
        public bool addTexturePrefix = true;


        private static readonly string NAME = "[ExporterSettings]: ";

        
        private static ExporterSettings _instance;
        public static ExporterSettings Instance => _instance ?? (_instance = LoadAsset());

        private static ExporterSettings LoadAsset([CallerFilePath] string callerFilepath = null)
        {
            var path = @"Assets\Data\Tools\CustomExporterSettings.asset";
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


            var paths = AssetDatabase.FindAssets("t:ExporterSettings");
            if (paths.Length > 0)
            {
                path = AssetDatabase.GUIDToAssetPath(paths[0]);
            }

            var asset = AssetDatabase.LoadAssetAtPath<ExporterSettings>(path);
            if (asset == null)
            {
                asset = CreateInstance<ExporterSettings>();
                AssetDatabase.CreateAsset(asset, path);
                Debug.Log($@"{NAME}CustomExporterSettings created at path {path}");
                AssetDatabase.SaveAssets();
            }

            return asset;
        }

        private static string GetAssetPath([CallerFilePath] string callerFilePath = null)
        {
            var folder = Path.GetDirectoryName(callerFilePath);
            folder = folder.Substring(folder.LastIndexOf("\\Assets\\", StringComparison.Ordinal) + 1);
            return Path.Combine(folder, "CustomExportSettings.asset");
        }
    }
}