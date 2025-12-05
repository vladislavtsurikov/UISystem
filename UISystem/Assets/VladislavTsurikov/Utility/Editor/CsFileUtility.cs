using System.IO;
using UnityEditor;

namespace VladislavTsurikov.Utility.Editor
{
    public class CsFileUtility
    {
        public static string FindPath(string name)
        {
            var guids = AssetDatabase.FindAssets($"{name} t:Script");
            if (guids == null || guids.Length == 0)
            {
                return string.Empty;
            }

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var fileName = Path.GetFileNameWithoutExtension(path);
                if (fileName == name)
                {
                    return path;
                }
            }

            return string.Empty;
        }
    }
}
