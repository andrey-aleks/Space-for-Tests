using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Exporter.Editor
{
    [CreateAssetMenu(fileName = "CustomExportSettings", menuName = "Custom Export Settings", order = 1010)]
    public class ExporterSettings : ScriptableObject
    {
        [Tooltip(@"File formats that are available to export")]
        public List<string> exportFileFormats = new List<string>() {"fbx", "FBX"};

        [Tooltip(@"Texture formats that are available to export")]
        public List<string> exportTextureFormats = new List<string>()
            {"png", "jpg", "jpeg", "tif", "tiff", "targa", "tga"};

        [Tooltip(@"Enables auto-adding prefix tex_ to exported textures")]
        public bool addTexturePrefix = true;


        private static ExporterSettings _instance;
        public static ExporterSettings Instance => _instance ?? (_instance = LoadAsset());

        private static ExporterSettings LoadAsset([CallerFilePath] string callerFilepath = null)
        {
            var path = GetAssetPath();
            var asset = AssetDatabase.LoadAssetAtPath<ExporterSettings>(path);

            if (asset == null)
            {
                asset = CreateInstance<ExporterSettings>();
                AssetDatabase.CreateAsset(asset, path);
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