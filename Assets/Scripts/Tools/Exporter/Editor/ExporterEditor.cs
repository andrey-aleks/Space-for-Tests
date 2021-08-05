using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Exporter.Editor
{
    public class ExporterEditor : EditorWindow
    {
        private static readonly ExporterSettings Settings = ExporterSettings.Instance;

        private static string NAME = "[ExporterEditor]: ";

        [MenuItem("Assets/Custom Tools/Export")]
        public static void ShowWindow()
        {
            Init();
        }

        private static void Init()
        {
            var exportObject = Selection.activeObject;
            var targetPath = EditorUtility.SaveFilePanel("Export as", "", exportObject.name + ".fbx", "fbx");
            var fileFormat = AssetDatabase.GetAssetPath(exportObject).Split('\\').Last().Split('.').Last();
            if (!string.IsNullOrEmpty(targetPath) && Settings.exportFileFormats.Contains(fileFormat))
//                AssetDatabase.GetAssetPath(exportObject).Split('\\').Last().Contains(".fbx"))
            {
                var sourcePaths = Selection.assetGUIDs;
                var sourcePath = Environment.CurrentDirectory + @"\" + AssetDatabase.GUIDToAssetPath(sourcePaths[0]);
                var dependencies = AssetDatabase.GetDependencies(AssetDatabase.GUIDToAssetPath(sourcePaths[0]));

                ExporterUtility.Export(sourcePath, targetPath, exportObject, dependencies);
            }
            else
            {
                Debug.Log($"{NAME}wrong path or wrong object (must be fbx or FBX)");
            }
        }
    }
}