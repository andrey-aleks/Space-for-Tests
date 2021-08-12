using UnityEditor;
using UnityEngine;

namespace FindObjectByGuid.Editor
{
    public class FindObjectByGuid : EditorWindow
    {
        public string guid;

        private readonly string NAME = "[FindObjectByGuid]: ";
        
        [MenuItem("Tools/Custom Tools/Find object by GUID")]
        public static void ShowWindow()
        {
            GetWindow<FindObjectByGuid >("Find object by GUID", true);
        }
        
        void OnGUI()
        {
            guid = EditorGUILayout.TextField("GUID", guid);

            if(GUILayout.Button("Find by GUID"))
                FindFileByGuid(guid);
        }

        private void FindFileByGuid(string targetGuid)
        {
            if (!string.IsNullOrEmpty(targetGuid))
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(targetGuid);
                if (string.IsNullOrEmpty(assetPath))
                {
                    Debug.LogError($"{NAME}GUID is not correct. Can't find asset path");
                    return;
                }

                Debug.Log($"{NAME}{targetGuid} ---> {assetPath}");
                
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(assetPath); // set active object
                EditorGUIUtility.PingObject(Selection.activeObject); // focus in project window
            }
            else Debug.LogError($"{NAME}GUID is null");
        }
    }
}