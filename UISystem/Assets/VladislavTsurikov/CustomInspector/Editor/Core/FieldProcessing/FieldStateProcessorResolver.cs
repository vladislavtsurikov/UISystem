using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public static class FieldStateProcessorResolver
    {
        private static readonly List<FieldStateProcessorMatcher> _matchers = new();

        static FieldStateProcessorResolver() => RegisterProcessors();

        private static void RegisterProcessors()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                Type[] matcherTypes = assembly.GetTypes()
                    .Where(t => typeof(FieldStateProcessorMatcher).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                    .ToArray();

                foreach (Type matcherType in matcherTypes)
                {
                    var instance = (FieldStateProcessorMatcher)Activator.CreateInstance(matcherType);
                    _matchers.Add(instance);
                }
            }
        }

        public static List<FieldStateProcessor> CreateProcessors(FieldInfo field)
        {
            var processors = new List<FieldStateProcessor>();

            foreach (Attribute attribute in field.GetCustomAttributes())
            {
                foreach (FieldStateProcessorMatcher matcher in _matchers)
                {
                    if (!matcher.CanProcess(attribute))
                    {
                        continue;
                    }

                    Type processorType = matcher.ProcessorType;
                    if (processorType == null || !typeof(FieldStateProcessor).IsAssignableFrom(processorType))
                    {
                        continue;
                    }

                    var processor = (FieldStateProcessor)Activator.CreateInstance(processorType);
                    processor.Initialize(attribute);
                    processors.Add(processor);
                    break;
                }
            }

            return processors;
        }
    }
}

