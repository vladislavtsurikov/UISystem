using System;
using System.Reflection;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.FieldProcessors
{
    public sealed class ReadOnlyFieldStateProcessorMatcher : FieldStateProcessorMatcher
    {
        public override bool CanProcess(Attribute attribute) => attribute is ReadOnlyAttribute;
        public override Type ProcessorType => typeof(ReadOnlyFieldStateProcessor);
    }

    public sealed class ReadOnlyFieldStateProcessor : FieldStateProcessor
    {
        public override void Apply(FieldInfo field, object target, FieldState state)
        {
            if (state == null)
            {
                return;
            }

            state.IsReadOnly = true;
        }
    }
}
