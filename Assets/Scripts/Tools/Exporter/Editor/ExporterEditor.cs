using System;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Exporter.Editor
{
    public class ExporterEditor : EditorWindow
    {
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
            if (!string.IsNullOrEmpty(targetPath) &&
                AssetDatabase.GetAssetPath(exportObject).Split('\\').Last().Contains(".fbx"))
            {
                var sourcePaths = Selection.assetGUIDs;
                var sourcePath = Environment.CurrentDirectory + @"\" + AssetDatabase.GUIDToAssetPath(sourcePaths[0]);
                var dependencies = AssetDatabase.GetDependencies(AssetDatabase.GUIDToAssetPath(sourcePaths[0]));

                ExporterUtility.Export(sourcePath, targetPath, exportObject, dependencies);
            }
            else
            {
                Debug.Log($"{NAME}wrong path or wrong object (must be FBX)");
            }
        }
    }
}