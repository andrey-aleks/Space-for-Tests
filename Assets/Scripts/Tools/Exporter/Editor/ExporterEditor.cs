using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


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
            
            if (!string.IsNullOrEmpty(targetPath))
            {
                ExporterUtility.Export(GetFullSourcePath(exportObject), targetPath);
            }
            else
            {
                Debug.Log($"{NAME}wrong path");
            }
        }

        private static string GetFullSourcePath(Object asset)
        {
            var sourceFullPath = Environment.CurrentDirectory + @"\" + AssetDatabase.GetAssetPath(asset);
            return sourceFullPath;
        }
    }
}