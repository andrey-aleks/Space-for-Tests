#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace GitLock
{
    [InitializeOnLoad]
    public static class ChangeMonitor
    {
        static ChangeMonitor()
        {
            PrefabStage.prefabStageOpened += OnPrefabStageOpened;
            EditorSceneManager.sceneDirtied += OnSceneDirty;
        }

        private static void OnSceneDirty(Scene scene) => CheckGitLfsLock(scene.path); 

        private static void OnPrefabStageOpened(PrefabStage obj) => CheckGitLfsLock(obj.prefabAssetPath);

        private static void CheckGitLfsLock(string file)
        {
            if(GitLockManger.CheckLocks(file))
                LockedPopup.Init();
        }
    }
}
#endif
