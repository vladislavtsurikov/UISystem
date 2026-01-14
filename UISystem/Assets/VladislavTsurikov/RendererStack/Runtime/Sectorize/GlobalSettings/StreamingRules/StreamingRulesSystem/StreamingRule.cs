using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem
{
    public abstract class StreamingRule : Node
    {
        protected NodeStackOnlyDifferentTypes<StreamingRule> StreamingRuleComponentStack =>
            (NodeStackOnlyDifferentTypes<StreamingRule>)Stack;
    }
}
