using System;
using VladislavTsurikov.Core.Runtime;

namespace VladislavTsurikov.IMGUIUtility.Editor
{
    [Serializable]
    public class IMGUIContentPath : BasePathFinder<IMGUIContentPath>
    {
        private const string GUISkinFileName = "GUISkin.guiskin";
        public static string FoldoutRightFileName = "FoldoutRight.png";
        public static string FoldoutDownFileName = "FoldoutDown.png";

        public static string Images = "Images";
        public static string Foldout = "Foldout";

        public static string PathToImages = CombinePath(Path, Images);
        public static string PathToFoldout = CombinePath(PathToImages, Foldout);

        public static string FoldoutRightPath = CombinePath(PathToFoldout, FoldoutRightFileName);
        public static string FoldoutDownPath = CombinePath(PathToFoldout, FoldoutDownFileName);

        public static readonly string SkinPath = CombinePath(Path, GUISkinFileName);

        public static string ReplaceBackslashesWithForwardSlashes(string path) => path.Replace('\\', '/');

        public static string CombinePath(string path1, string path2) => path1 + "/" + path2;
    }
}
