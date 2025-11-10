using System;
using VladislavTsurikov.Core.Runtime;

namespace VladislavTsurikov.ScriptableObjectUtility.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LocationAssetAttribute : Attribute
    {
        private readonly string _relativePath;
        private string _filePath;

        public LocationAssetAttribute(string relativePath) => _relativePath = relativePath;

        public string RelativePath => CommonPath.CombinePath(CommonPath.Publisher, _relativePath);

        public string FilePath
        {
            get
            {
                if (_filePath != null)
                {
                    return _filePath;
                }

                var pathToFolder = CommonPath.CombinePath(CommonPath.PathToResources, _relativePath);

                _filePath = pathToFolder + ".asset";

                return _filePath;
            }
        }
    }
}
