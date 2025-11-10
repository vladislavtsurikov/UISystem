namespace VladislavTsurikov.Core.Runtime
{
    public static class CommonPath
    {
        public const string Publisher = "VladislavTsurikov";

        public const string ShaderVariantCollectionName = "Shader Variant Collection";
        private static readonly string Resources = CombinePath("Resources", Publisher);

        public static readonly string PathToResources = CombinePath("Assets", Resources);

        public static readonly string PathToShaderVariantCollection =
            CombinePath(PathToResources, ShaderVariantCollectionName);

        public static string CombinePath(string path1, string path2) => path1 + "/" + path2;
    }
}
