using System;
using System.Linq;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SceneFilterAttribute : FilterAttribute
    {
        public SceneFilterAttribute(params string[] sceneNames) => SceneNames = sceneNames ?? Array.Empty<string>();

        public string[] SceneNames { get; }

        public bool Matches(string sceneName) => SceneNames.Contains(sceneName, StringComparer.OrdinalIgnoreCase);
    }
}
