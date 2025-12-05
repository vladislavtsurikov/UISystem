using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem
{
    [Name("Caching")]
    public class Caching : StreamingRule
    {
        public float KeepScenes = 300;
        public float MaxLoadingCachedScenePause = 0.2f;
    }
}
