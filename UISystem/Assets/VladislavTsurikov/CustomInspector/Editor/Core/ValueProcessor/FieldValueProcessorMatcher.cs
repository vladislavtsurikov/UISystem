using System;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class FieldValueProcessorMatcher
    {
        public abstract bool CanProcess(Attribute attribute);
        public abstract Type ProcessorType { get; }
    }
}
