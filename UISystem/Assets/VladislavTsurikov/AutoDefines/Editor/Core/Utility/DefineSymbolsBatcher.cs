#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace VladislavTsurikov.AutoDefines.Editor.Core
{
    public static class DefineSymbolsBatcher
    {
        private static readonly Dictionary<string, bool> s_defineSymbols = new();
        private static bool s_commitScheduled;

        public static void ApplyDefine(string defineSymbol, bool enable)
        {
            s_defineSymbols[defineSymbol] = enable;

            if (!s_commitScheduled)
            {
                s_commitScheduled = true;
                EditorApplication.update += CommitDefinesNextFrame;
            }
        }

        private static void CommitDefinesNextFrame()
        {
            EditorApplication.update -= CommitDefinesNextFrame;
            s_commitScheduled = false;

            foreach (NamedBuildTarget target in GetPlatformTargets())
            {
                if (target == NamedBuildTarget.Unknown)
                {
                    continue;
                }

                List<string> defines = GetCurrentDefines(target);
                UpdateDefines(defines);
                SaveDefines(target, defines);
            }

            s_defineSymbols.Clear();
        }

        private static List<string> GetCurrentDefines(NamedBuildTarget target)
        {
            var current = PlayerSettings.GetScriptingDefineSymbols(target);
            return new List<string>(
                current.Split(';').Where(s => !string.IsNullOrWhiteSpace(s))
            );
        }

        private static void UpdateDefines(List<string> defines)
        {
            foreach (var (symbol, enable) in s_defineSymbols)
            {
                if (enable)
                {
                    if (!defines.Contains(symbol))
                    {
                        defines.Add(symbol);
                    }
                }
                else
                {
                    defines.Remove(symbol);
                }
            }
        }

        private static void SaveDefines(NamedBuildTarget target, List<string> defines)
        {
            var joined = string.Join(";", defines);
            PlayerSettings.SetScriptingDefineSymbols(target, joined);
        }

        private static IEnumerable<NamedBuildTarget> GetPlatformTargets()
        {
            return new[]
            {
                NamedBuildTarget.Standalone,
                NamedBuildTarget.Android,
                NamedBuildTarget.iOS,
                NamedBuildTarget.WebGL,
                NamedBuildTarget.Server,
                NamedBuildTarget.Unknown
            };
        }
    }
}
#endif
