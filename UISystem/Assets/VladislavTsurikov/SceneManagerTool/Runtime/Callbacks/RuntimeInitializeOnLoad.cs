using UnityEngine;
using VladislavTsurikov.SceneManagerTool.Runtime.Utility;

namespace VladislavTsurikov.SceneManagerTool.Runtime.Callbacks
{
    public static class RuntimeInitializeOnLoad
    {
        [RuntimeInitializeOnLoadMethod]
        internal static void OnLoad()
        {
            if (!SceneManagerData.IsValidSceneManager())
            {
                return;
            }
#if UNITY_EDITOR
            if (!SceneManagerData.Instance.SceneManagerEditorData.RunAsBuildMode)
            {
                return;
            }
#endif

            RuntimeUtility.Start();
        }
    }
}
