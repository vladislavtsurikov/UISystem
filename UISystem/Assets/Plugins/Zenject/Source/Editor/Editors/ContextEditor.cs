#if !ODIN_INSPECTOR

namespace Zenject
{
    [NoReflectionBaking]
    public class ContextEditor : UnityInspectorListEditor
    {
        protected override string[] PropertyNames =>
            new[] { "_scriptableObjectInstallers", "_monoInstallers", "_installerPrefabs" };

        protected override string[] PropertyDisplayNames =>
            new[] { "Scriptable Object Installers", "Mono Installers", "Prefab Installers" };

        protected override string[] PropertyDescriptions =>
            new[]
            {
                "Drag any assets in your Project that implement ScriptableObjectInstaller here",
                "Drag any MonoInstallers that you have added to your Scene Hierarchy here.",
                "Drag any prefabs that contain a MonoInstaller on them here"
            };
    }
}

#endif
