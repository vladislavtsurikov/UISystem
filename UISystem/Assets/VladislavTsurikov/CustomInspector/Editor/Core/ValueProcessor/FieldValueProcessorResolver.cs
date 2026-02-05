using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public static class FieldValueProcessorResolver
    {
        private static readonly List<FieldValueProcessorMatcher> _matchers = new();

        static FieldValueProcessorResolver() => RegisterProcessors();

        private static void RegisterProcessors()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                Type[] matcherTypes = assembly.GetTypes()
                    .Where(t => typeof(FieldValueProcessorMatcher).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                    .ToArray();

                foreach (Type matcherType in matcherTypes)
                {
                    var instance = (FieldValueProcessorMatcher)Activator.CreateInstance(matcherType);
                    _matchers.Add(instance);
                }
            }
        }

        public static List<FieldValueProcessor> CreateProcessors(FieldInfo field)
        {
            var processors = new List<FieldValueProcessor>();

            foreach (Attribute attribute in field.GetCustomAttributes())
            {
                foreach (FieldValueProcessorMatcher matcher in _matchers)
                {
                    if (!matcher.CanProcess(attribute))
                    {
                        continue;
                    }

                    Type processorType = matcher.ProcessorType;
                    if (processorType == null || !typeof(FieldValueProcessor).IsAssignableFrom(processorType))
                    {
                        continue;
                    }

                    var processor = (FieldValueProcessor)Activator.CreateInstance(processorType);
                    processor.Initialize(attribute);
                    processors.Add(processor);
                    break;
                }
            }

            return processors;
        }
    }
}
