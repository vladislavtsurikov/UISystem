using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public static class FieldVisibilityProcessorResolver
    {
        private static readonly List<FieldVisibilityProcessorMatcher> _matchers = new();

        static FieldVisibilityProcessorResolver() => RegisterProcessors();

        private static void RegisterProcessors()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                Type[] matcherTypes = assembly.GetTypes()
                    .Where(t => typeof(FieldVisibilityProcessorMatcher).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                    .ToArray();

                foreach (Type matcherType in matcherTypes)
                {
                    var instance = (FieldVisibilityProcessorMatcher)Activator.CreateInstance(matcherType);
                    _matchers.Add(instance);
                }
            }
        }

        public static List<FieldVisibilityProcessor> CreateProcessors(FieldInfo field)
        {
            var processors = new List<FieldVisibilityProcessor>();

            foreach (Attribute attribute in field.GetCustomAttributes())
            {
                foreach (FieldVisibilityProcessorMatcher matcher in _matchers)
                {
                    if (!matcher.CanProcess(attribute))
                    {
                        continue;
                    }

                    Type processorType = matcher.ProcessorType;
                    if (processorType == null || !typeof(FieldVisibilityProcessor).IsAssignableFrom(processorType))
                    {
                        continue;
                    }

                    var processor = (FieldVisibilityProcessor)Activator.CreateInstance(processorType);
                    processor.Initialize(attribute);
                    processors.Add(processor);
                    break;
                }
            }

            return processors;
        }
    }
}

