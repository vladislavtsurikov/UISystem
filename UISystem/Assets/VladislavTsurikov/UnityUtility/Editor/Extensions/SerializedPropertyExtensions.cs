#if UNITY_EDITOR
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;

namespace VladislavTsurikov.UnityUtility.Editor
{
    public static class SerializedPropertyExtensions
    {
        private static readonly Regex ArrayIndexCapturePattern = new(@"\[(\d*)\]");

        public static T GetTarget<T>(this SerializedProperty prop)
        {
            var propertyNames = prop.propertyPath.Split('.');
            object target = prop.serializedObject.targetObject;
            var isNextPropertyArrayIndex = false;
            for (var i = 0; i < propertyNames.Length && target != null; ++i)
            {
                var propName = propertyNames[i];
                if (propName == "Array")
                {
                    isNextPropertyArrayIndex = true;
                }
                else if (isNextPropertyArrayIndex)
                {
                    isNextPropertyArrayIndex = false;
                    var arrayIndex = ParseArrayIndex(propName);
                    var targetAsArray = (object[])target;
                    target = targetAsArray[arrayIndex];
                }
                else
                {
                    target = GetField(target, propName);
                }
            }

            return (T)target;
        }

        private static object GetField(object target, string name, Type targetType = null)
        {
            if (targetType == null)
            {
                targetType = target.GetType();
            }

            FieldInfo fi = targetType.GetField(name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fi != null)
            {
                return fi.GetValue(target);
            }

            // If not found, search in parent
            if (targetType.BaseType != null)
            {
                return GetField(target, name, targetType.BaseType);
            }

            return null;
        }

        private static int ParseArrayIndex(string propName)
        {
            Match match = ArrayIndexCapturePattern.Match(propName);
            if (!match.Success)
            {
                throw new Exception($"Invalid array index parsing in {propName}");
            }

            return int.Parse(match.Groups[1].Value);
        }
    }
}
#endif
