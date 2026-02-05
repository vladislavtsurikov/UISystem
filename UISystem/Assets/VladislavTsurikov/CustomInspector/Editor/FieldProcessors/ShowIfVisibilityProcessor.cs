using System;
using System.Reflection;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.FieldProcessors
{
    public sealed class ShowIfVisibilityProcessorMatcher : FieldVisibilityProcessorMatcher
    {
        public override bool CanProcess(Attribute attribute) => attribute is ShowIfAttribute;
        public override Type ProcessorType => typeof(ShowIfVisibilityProcessor);
    }

    public sealed class ShowIfVisibilityProcessor : FieldVisibilityProcessor
    {
        private ShowIfAttribute _attribute;

        public override void Initialize(Attribute attribute)
        {
            base.Initialize(attribute);
            _attribute = attribute as ShowIfAttribute;
        }

        public override bool IsVisible(FieldInfo field, object target)
        {
            if (_attribute == null)
            {
                return true;
            }

            if (!ConditionMemberUtility.TryGetConditionValue(target, _attribute.ConditionMemberName, out var conditionValue))
            {
                return true;
            }

            bool result = ConditionMemberUtility.IsTruthy(conditionValue);
            return _attribute.Value == result;
        }
    }
}
