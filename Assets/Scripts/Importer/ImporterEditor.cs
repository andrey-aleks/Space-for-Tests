﻿using UnityEditor;
using Utilities.Importer;

namespace Importer
{
    public class ImporterEditor : EditorWindow
    {
        private static readonly ImportSettings Settings = ImportSettings.Instance;

        [MenuItem("Assets/Custom Import")]
        public static void ShowWindow()
        {
            Init();
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


        /*
        private void DrawDragAndDrop()
        {
            if (Event.current.type == EventType.DragPerform)
            {
                Debug.Log("path " + DragAndDrop.paths.First());
            }
        }
        */
    }
}