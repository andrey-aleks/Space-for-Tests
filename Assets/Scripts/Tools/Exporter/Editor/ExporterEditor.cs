using System.IO;
using UnityEditor;
using UnityEngine;


namespace Exporter.Editor
{
    public class ExporterEditor : EditorWindow
    {
        private static readonly string NAME = "[ExporterEditor]: "; // name for debug

        [MenuItem("Assets/Artist Tools/Import-Export/Export #_e")]
        public static void ShowWindow()
        {
            Init();
        }

        private static void Init()
        {
            var exportObject = Selection.activeObject; // get selected object
            var sourcePath = Path.GetFullPath(AssetDatabase.GetAssetPath(exportObject));

            var targetPath =
                EditorUtility.SaveFilePanel("Export File", "", exportObject.name + ".fbx",
                    "fbx"); // open dialog window for export and get target path from this

            if (!string.IsNullOrEmpty(targetPath))
            {
                ExporterUtility.Export(sourcePath, targetPath);
            }
            else
            {
                Debug.Log($"{NAME}wrong path");
            }
        }
    }
}