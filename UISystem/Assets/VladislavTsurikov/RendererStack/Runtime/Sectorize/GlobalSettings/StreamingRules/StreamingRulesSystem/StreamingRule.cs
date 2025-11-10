using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem
{
    public abstract class StreamingRule : Component
    {
        protected ComponentStackOnlyDifferentTypes<StreamingRule> StreamingRuleComponentStack =>
            (ComponentStackOnlyDifferentTypes<StreamingRule>)Stack;
    }
}
