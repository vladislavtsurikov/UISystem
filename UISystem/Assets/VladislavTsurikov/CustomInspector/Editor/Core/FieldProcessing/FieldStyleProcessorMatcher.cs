using System;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class FieldStyleProcessorMatcher
    {
        public abstract bool CanProcess(Attribute attribute);
        public abstract Type ProcessorType { get; }
    }
}

