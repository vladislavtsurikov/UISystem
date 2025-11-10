using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace VladislavTsurikov.AttributeUtility.Runtime
{
    public class AttributeCache
    {
        // Important to use Attribute.GetCustomAttributes, because MemberInfo.GetCustomAttributes
        // ignores the inherited parameter on properties and events

        // However, Attribute.GetCustomAttributes seems to have at least two obscure Mono 2.0 bugs.

        // 1. Basically, when a parameter is optional and is marked as [OptionalAttribute],
        // the custom attributes array is typed object[] instead of Attribute[], which
        // makes Mono throw an exception in Attribute.GetCustomAttributes when trying
        // to cast the array. After some testing, it appears this only happens for
        // non-inherited calls, and only for parameter infos (although I'm not sure why).
        // I *believe* the offending line in the Mono source is this one:
        // https://github.com/mono/mono/blob/mono-2-0/mcs/class/corlib/System/MonoCustomAttrs.cs#L143

        // 2. For some other implementation reason, on iOS, GetCustomAttributes on MemberInfo fails.
        // https://support.ludiq.io/forums/5-bolt/topics/729-systeminvalidcastexception-in-attributecache-on-ios/

        // As a fallback, we will use the GetCustomAttributes from the type itself,
        // which doesn't seem to be bugged (ugh). But because this method ignores the
        // inherited parameter on some occasions, we will warn if the inherited fetch fails.

        // Additionally, some Unity built-in attributes use threaded API methods in their
        // constructors and will therefore throw an error if GetCustomAttributes is called
        // from the serialization thread or from a secondary thread. We'll generally fallback
        // and warn on any exception to make sure not to block anything more than needed.
        // https://support.ludiq.io/communities/5/topics/2024-/

        public AttributeCache(MemberInfo element)
        {
            try
            {
                try
                {
                    Cache(Attribute.GetCustomAttributes(element, true), inheritedAttributes);
                }
                catch (InvalidCastException ex)
                {
                    Cache(element.GetCustomAttributes(true).Cast<Attribute>().ToArray(), inheritedAttributes);
                    Debug.LogWarning($"Failed to fetch inherited attributes on {element}.\n{ex}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to fetch inherited attributes on {element}.\n{ex}");
            }

            try
            {
                try
                {
                    Cache(Attribute.GetCustomAttributes(element, false), definedAttributes);
                }
                catch (InvalidCastException)
                {
                    Cache(element.GetCustomAttributes(false).Cast<Attribute>().ToArray(), definedAttributes);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to fetch defined attributes on {element}.\n{ex}");
            }
        }

        public AttributeCache(ParameterInfo element)
        {
            try
            {
                try
                {
                    Cache(Attribute.GetCustomAttributes(element, true), inheritedAttributes);
                }
                catch (InvalidCastException ex)
                {
                    Cache(element.GetCustomAttributes(true).Cast<Attribute>().ToArray(), inheritedAttributes);
                    Debug.LogWarning($"Failed to fetch inherited attributes on {element}.\n{ex}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to fetch inherited attributes on {element}.\n{ex}");
            }

            try
            {
                try
                {
                    Cache(Attribute.GetCustomAttributes(element, false), definedAttributes);
                }
                catch (InvalidCastException)
                {
                    Cache(element.GetCustomAttributes(false).Cast<Attribute>().ToArray(), definedAttributes);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to fetch defined attributes on {element}.\n{ex}");
            }
        }

        public AttributeCache(IAttributeProvider element)
        {
            try
            {
                Cache(element.GetCustomAttributes(true), inheritedAttributes);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to fetch inherited attributes on {element}.\n{ex}");
            }

            try
            {
                Cache(element.GetCustomAttributes(false), definedAttributes);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to fetch defined attributes on {element}.\n{ex}");
            }
        }

        // Using lists instead of hashsets because:
        //  - Insertion will be faster
        //  - Iteration will be just as fast
        //  - We don't need contains lookups
        public List<Attribute> inheritedAttributes { get; } = new();
        public List<Attribute> definedAttributes { get; } = new();

        private void Cache(Attribute[] attributeObjects, List<Attribute> cache)
        {
            foreach (Attribute attributeObject in attributeObjects)
            {
                cache.Add(attributeObject);
            }
        }

        private bool HasAttribute(Type attributeType, List<Attribute> cache)
        {
            for (var i = 0; i < cache.Count; i++)
            {
                Attribute attribute = cache[i];

                if (attributeType.IsInstanceOfType(attribute))
                {
                    return true;
                }
            }

            return false;
        }

        private Attribute GetAttribute(Type attributeType, List<Attribute> cache)
        {
            for (var i = 0; i < cache.Count; i++)
            {
                Attribute attribute = cache[i];

                if (attributeType.IsInstanceOfType(attribute))
                {
                    return attribute;
                }
            }

            return null;
        }

        private IEnumerable<Attribute> GetAttributes(Type attributeType, List<Attribute> cache)
        {
            for (var i = 0; i < cache.Count; i++)
            {
                Attribute attribute = cache[i];

                if (attributeType.IsInstanceOfType(attribute))
                {
                    yield return attribute;
                }
            }
        }

        public bool HasAttribute(Type attributeType, bool inherit = true)
        {
            if (inherit)
            {
                return HasAttribute(attributeType, inheritedAttributes);
            }

            return HasAttribute(attributeType, definedAttributes);
        }

        public Attribute GetAttribute(Type attributeType, bool inherit = true)
        {
            if (inherit)
            {
                return GetAttribute(attributeType, inheritedAttributes);
            }

            return GetAttribute(attributeType, definedAttributes);
        }

        public IEnumerable<Attribute> GetAttributes(Type attributeType, bool inherit = true)
        {
            if (inherit)
            {
                return GetAttributes(attributeType, inheritedAttributes);
            }

            return GetAttributes(attributeType, definedAttributes);
        }

        public bool HasAttribute<TAttribute>(bool inherit = true)
            where TAttribute : Attribute =>
            HasAttribute(typeof(TAttribute), inherit);

        public TAttribute GetAttribute<TAttribute>(bool inherit = true)
            where TAttribute : Attribute =>
            (TAttribute)GetAttribute(typeof(TAttribute), inherit);

        public IEnumerable<TAttribute> GetAttributes<TAttribute>(bool inherit = true)
            where TAttribute : Attribute =>
            GetAttributes(typeof(TAttribute), inherit).Cast<TAttribute>();
    }
}
