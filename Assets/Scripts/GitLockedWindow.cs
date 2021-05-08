using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GitLock
{
    public class GitLockedWindow : EditorWindow
    {
        [MenuItem("Git/Locked files")]
        public static void ShowWindow()
        {
            GetWindow<GitLockedWindow >(false, "Git locked files", true);
        }

        private void OnEnable()
        {
            UpdateList();
            GitLockManger.OnListRefreshed += Repaint;
        }

        void OnGUI()
        {
            Rect panel = new Rect(0, 0, position.width, position.height);
            GUILayout.BeginArea(panel);
            if(GUILayout.Button("Refresh"))
                UpdateList();
            
            foreach (var lockedFile in GitLockManger.LockedObjects)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(lockedFile.filePath);
                EditorGUILayout.LabelField(lockedFile.lockedBy, GUILayout.MaxWidth(60));
                if (GUILayout.Button("Unlock"))
                {
                    GitLockManger.UnlockFile(lockedFile.filePath);
                    return;
                }

                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }

        private void UpdateList()
        {
            GitLockManger.LockedFilesUpdate();
            Repaint();
        }
    }
}
