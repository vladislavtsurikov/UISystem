using System;
using System.Reflection;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.FieldProcessors
{
    public sealed class HideInInspectorVisibilityProcessorMatcher : FieldVisibilityProcessorMatcher
    {
        public override bool CanProcess(Attribute attribute) => attribute is HideInInspector;
        public override Type ProcessorType => typeof(HideInInspectorVisibilityProcessor);
    }

    public sealed class HideInInspectorVisibilityProcessor : FieldVisibilityProcessor
    {
        public override bool IsVisible(FieldInfo field, object target)
        {
            return CustomInspectorPreferences.Instance.ShowFieldWithHideInInspectorAttribute;
        }
    }
}
