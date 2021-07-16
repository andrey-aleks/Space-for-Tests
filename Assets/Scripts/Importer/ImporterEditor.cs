using UnityEditor;
using UnityEngine;

namespace Importer
{
    public class ImporterEditor : EditorWindow
    {
        [MenuItem("Assets/-- Custom Import")]
        public static void ShowWindow()
        {
            Init();
        }

        private void OnEnable()
        {
            Debug.Log("enabled");
        }

        private static void Init()
        {
            string path = EditorUtility.OpenFilePanel("Open File", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                ImporterUtility.Import(path);
            }

            // ImportSettings.Instance.path = EditorUtility.OpenFilePanel("Open File", "", "");
        }
    }
}