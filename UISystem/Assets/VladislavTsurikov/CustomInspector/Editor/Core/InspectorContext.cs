using System;
using System.Collections.Generic;
using System.Reflection;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public static class InspectorContext
    {
        [ThreadStatic] private static Stack<InspectorContextData> _stack;

        public static InspectorContextData Current => _stack != null && _stack.Count > 0 ? _stack.Peek() : null;

        public static IDisposable Push(object target, FieldInfo field, int? elementIndex)
        {
            _stack ??= new Stack<InspectorContextData>();
            var data = new InspectorContextData(target, field, elementIndex);
            _stack.Push(data);
            return new InspectorContextScope();
        }

        public sealed class InspectorContextData
        {
            public InspectorContextData(object target, FieldInfo field, int? elementIndex)
            {
                Target = target;
                Field = field;
                ElementIndex = elementIndex;
            }

            public object Target { get; }
            public FieldInfo Field { get; }
            public int? ElementIndex { get; }
        }

        private sealed class InspectorContextScope : IDisposable
        {
            public void Dispose()
            {
                if (_stack == null || _stack.Count == 0)
                {
                    return;
                }

                _stack.Pop();
            }
        }
    }
}
