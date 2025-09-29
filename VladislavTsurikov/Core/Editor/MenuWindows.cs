#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.Core.Editor
{
    public static class MenuWindows
    {
        [MenuItem("Window/Vladislav Tsurikov/Patreon", false, 1000)]
        public static void Patreon() => Application.OpenURL("https://www.patreon.com/user/posts?u=62137729");

        [MenuItem("Window/Vladislav Tsurikov/Discord Server", false, 1000)]
        public static void Discord() => Application.OpenURL("https://discord.gg/fVAmyXs8GH");
    }
}
#endif
