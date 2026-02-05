using System;
using System.Reflection;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.FieldProcessors
{
    public sealed class GUIColorFieldStyleProcessorMatcher : FieldStyleProcessorMatcher
    {
        public override bool CanProcess(Attribute attribute) => attribute is GUIColorAttribute;
        public override Type ProcessorType => typeof(GUIColorFieldStyleProcessor);
    }

    public sealed class GUIColorFieldStyleProcessor : FieldStyleProcessor
    {
        private GUIColorAttribute _attribute;

        public override void Initialize(Attribute attribute)
        {
            base.Initialize(attribute);
            _attribute = attribute as GUIColorAttribute;
        }

        public override void Apply(FieldInfo field, object target, FieldStyle style)
        {
            if (_attribute == null || style == null)
            {
                return;
            }

            style.Color = _attribute.GetColor(target);
        }
    }
}
