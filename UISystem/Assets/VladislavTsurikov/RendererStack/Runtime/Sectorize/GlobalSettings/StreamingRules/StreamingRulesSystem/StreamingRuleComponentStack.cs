using System;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem
{
    public class StreamingRuleComponentStack : NodeStackOnlyDifferentTypes<StreamingRule>
    {
        public StreamingRuleComponentStack() => CreateAllComponents();

        internal void CreateAllComponents()
        {
            OnCreateElements();

            foreach (Type type in AllTypesDerivedFrom<StreamingRule>.Types)
            {
                CreateIfMissingType(type);
            }
        }

        protected override void OnCreateElements()
        {
            foreach (Type type in AllTypesDerivedFrom<StreamingRule>.Types)
            {
                if (type.GetAttribute<PersistentNodeAttribute>() != null)
                {
                    CreateElementIfMissingType(type);
                }
            }
        }
    }
}
