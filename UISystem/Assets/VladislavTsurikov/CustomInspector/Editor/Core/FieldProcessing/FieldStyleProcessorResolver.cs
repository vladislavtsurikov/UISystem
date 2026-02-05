using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public static class FieldStyleProcessorResolver
    {
        private static readonly List<FieldStyleProcessorMatcher> _matchers = new();

        static FieldStyleProcessorResolver() => RegisterProcessors();

        private static void RegisterProcessors()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                Type[] matcherTypes = assembly.GetTypes()
                    .Where(t => typeof(FieldStyleProcessorMatcher).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                    .ToArray();

                foreach (Type matcherType in matcherTypes)
                {
                    var instance = (FieldStyleProcessorMatcher)Activator.CreateInstance(matcherType);
                    _matchers.Add(instance);
                }
            }
        }

        public static List<FieldStyleProcessor> CreateProcessors(FieldInfo field)
        {
            var processors = new List<FieldStyleProcessor>();

            foreach (Attribute attribute in field.GetCustomAttributes())
            {
                foreach (FieldStyleProcessorMatcher matcher in _matchers)
                {
                    if (!matcher.CanProcess(attribute))
                    {
                        continue;
                    }

                    Type processorType = matcher.ProcessorType;
                    if (processorType == null || !typeof(FieldStyleProcessor).IsAssignableFrom(processorType))
                    {
                        continue;
                    }

                    var processor = (FieldStyleProcessor)Activator.CreateInstance(processorType);
                    processor.Initialize(attribute);
                    processors.Add(processor);
                    break;
                }
            }

            return processors;
        }
    }
}

