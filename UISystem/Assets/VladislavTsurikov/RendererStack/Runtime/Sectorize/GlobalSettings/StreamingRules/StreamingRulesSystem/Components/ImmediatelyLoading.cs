using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem
{
    [PersistentComponent]
    [Name("Immediately Loading")]
    public class ImmediatelyLoading : StreamingRule
    {
        public float MaxDistance = 2000;

        public override bool ShowActiveToggle() => false;
    }
}
