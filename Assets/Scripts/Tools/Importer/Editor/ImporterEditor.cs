using UnityEditor;

namespace Importer.Editor
{
    public class ImporterEditor : EditorWindow
    {
        private static readonly ImporterSettings Settings = ImporterSettings.Instance;

        [MenuItem("Assets/Artist Tools/Import-Export/Import #_q")]
        public static void ShowWindow()
        {
            Init();
        }

        private static void Init()
        {
            string path = EditorUtility.OpenFilePanel("Import File", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                ImporterUtility.Import(path);
            }
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