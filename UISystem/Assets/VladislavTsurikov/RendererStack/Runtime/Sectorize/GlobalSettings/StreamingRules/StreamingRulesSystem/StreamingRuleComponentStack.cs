using System;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem
{
    public class StreamingRuleComponentStack : ComponentStackOnlyDifferentTypes<StreamingRule>
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
                if (type.GetAttribute<PersistentComponentAttribute>() != null)
                {
                    CreateElementIfMissingType(type);
                }
            }
        }
    }
}
