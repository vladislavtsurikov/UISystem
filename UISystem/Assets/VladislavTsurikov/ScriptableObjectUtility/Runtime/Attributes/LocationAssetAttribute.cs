using System;
using VladislavTsurikov.Core.Runtime;

namespace VladislavTsurikov.ScriptableObjectUtility.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LocationAssetAttribute : Attribute
    {
        private readonly string _relativePath;
        private readonly bool _includePublisher;
        private string _filePath;

        public LocationAssetAttribute(string relativePath, bool includePublisher = true)
        {
            _relativePath = relativePath;
            _includePublisher = includePublisher;
        }

        public string RelativePath => _includePublisher
            ? CommonPath.CombinePath(CommonPath.Publisher, _relativePath)
            : _relativePath;

        public string FilePath
        {
            get
            {
                if (_filePath != null)
                {
                    return _filePath;
                }

                var pathToFolder = _includePublisher
                    ? CommonPath.CombinePath(CommonPath.PathToResources, _relativePath)
                    : CommonPath.CombinePath("Assets/Resources", _relativePath);

                _filePath = pathToFolder + ".asset";

                return _filePath;
            }
        }
    }
}
