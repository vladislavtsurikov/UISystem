using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem
{
    [Name("Scene Behavior")]
    [SceneComponent]
    public class SceneBehavior : SettingsComponent
    {
        public SceneCloseBehavior SceneCloseBehavior = SceneCloseBehavior.Close;
        public SceneOpenBehavior SceneOpenBehavior = SceneOpenBehavior.Open;
    }
}
