using System;
using System.Reflection;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.FieldProcessors
{
    public sealed class DisableIfFieldStateProcessorMatcher : FieldStateProcessorMatcher
    {
        public override bool CanProcess(Attribute attribute) => attribute is DisableIfAttribute;
        public override Type ProcessorType => typeof(DisableIfFieldStateProcessor);
    }

    public sealed class DisableIfFieldStateProcessor : FieldStateProcessor
    {
        private DisableIfAttribute _attribute;

        public override void Initialize(Attribute attribute)
        {
            base.Initialize(attribute);
            _attribute = attribute as DisableIfAttribute;
        }

        public override void Apply(FieldInfo field, object target, FieldState state)
        {
            if (_attribute == null || state == null)
            {
                return;
            }

            if (!ConditionMemberUtility.TryGetConditionValue(target, _attribute.ConditionMemberName, out var conditionValue))
            {
                return;
            }

            if (ConditionMemberUtility.IsTruthy(conditionValue))
            {
                state.IsEnabled = false;
            }
        }
    }
}
