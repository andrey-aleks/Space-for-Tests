#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Utilities
{
    public class FindObjectByGUID : EditorWindow
    {
        public string guid;
        
        [MenuItem("Tools/Custom Tools/Find object by GUID")]
        public static void ShowWindow()
        {
            GetWindow<FindObjectByGUID >(false, "Find object by GUID", true);
        }
        
        void OnGUI()
        {
            Rect panel = new Rect(0, 0, position.width, position.height);
            guid = EditorGUILayout.TextField("GUID", guid);

            if(GUILayout.Button("Find by GUID"))
                FindFileByGUID();
        }

        private void FindFileByGUID()
        {
            if (guid != null)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log(guid + " ---> " + assetPath);
            }
            else Debug.Log("[FindObjectByGUID] GUID is null");
        }
    }
}
#endif