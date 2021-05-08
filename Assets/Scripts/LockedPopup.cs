#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GitLock
{
    public class LockedPopup : EditorWindow
    {
        public static void Init()
        {
            LockedPopup window = ScriptableObject.CreateInstance<LockedPopup>();
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
            window.ShowPopup();
        }

        void OnGUI()
        {
            GUI.color = Color.red; 
            
            EditorGUILayout.LabelField("Файл используется другим пользователем", EditorStyles.wordWrappedLabel);
            GUILayout.Space(70);
            if (GUILayout.Button("Я только посмотерть"))
                this.Close();
        }
    }
}
#endif