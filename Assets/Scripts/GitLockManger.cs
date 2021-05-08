#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;

namespace GitLock
{
    [Serializable]
    public class LockedObject
    {
        public string filePath;
        public string lockedBy;
        public string lockId;
    }

    public class GitLockManger : EditorWindow
    {
        private const string _lfsLocks = "lfs locks";
        private const string _lockFile = "lfs lock ";
        private const string _unlockFile = "lfs unlock ";

        public static List<LockedObject> LockedObjects = new List<LockedObject>();
        
        public static Action OnListRefreshed;  
        
        public static bool CheckLocks(string file)
        {
            foreach (var lockedFile in LockedFilesUpdate())
                if (lockedFile.filePath.Equals(file))
                    return true;

            return false;
        }
        
        [MenuItem("Assets/Git/Lock")]
        public static void LockFile()
        {
            string file = AssetDatabase.GetAssetPath(Selection.activeObject) ;
            LockFile(file);
        }

        public static void LockFile(string file)
        {
            if (CheckLocks(file))
            {
                LockedPopup.Init();
                return;
            }

            Execute( _lockFile + "\"" + file+ "\"");
        }
        
        [MenuItem("Assets/Git/Unlock")]
        public static void UnlockFile()
        {
            string file = AssetDatabase.GetAssetPath(Selection.activeObject) ;
            UnlockFile(file);
        }

        public static void UnlockFile(string file) => Execute( _unlockFile + "\"" +file + "\"");

        public static List<LockedObject> LockedFilesUpdate()
        {
            return ParseData(Execute(_lfsLocks));
        }

// // Testing area begin
//         [MenuItem("git/GitLocks cmd")]
//         private static void Init()
//         {
//             LockedFiles();
//             GitLockManger window = ScriptableObject.CreateInstance<GitLockManger>();
//             window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
//             window.ShowPopup();
//         }
//
//         void OnGUI() => Close();
// // Testing area end

        private static string Execute(string command)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = "git";
            p.StartInfo.Arguments = command;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            if (!command.Equals(_lfsLocks))
                LockedFilesUpdate();
            
            return output;
        }

        private static List<LockedObject> ParseData(string data)
        {
            LockedObjects.Clear();
            
            var dataArray = data.Trim().Split('\t', '\n').Select(p => p.Trim()).
                                        Where(s => s != string.Empty).ToList();
            for (int i = 0; i < dataArray.Count; i += 3)
            {
                LockedObjects.Add(new LockedObject()
                {
                    filePath = dataArray[i],
                    lockedBy = dataArray[i + 1],
                    lockId = dataArray[i + 2]
                });
            }
            OnListRefreshed?.Invoke();
            return LockedObjects;
        }
    }
}
#endif