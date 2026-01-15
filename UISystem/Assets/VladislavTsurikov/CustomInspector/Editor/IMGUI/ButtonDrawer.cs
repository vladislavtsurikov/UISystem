#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public static class ButtonDrawer
    {
        private static readonly Dictionary<Type, List<MethodInfo>> _cachedMethods = new();

        public static void DrawButtons(object target, ref Rect rect)
        {
            if (target == null)
            {
                return;
            }

            List<MethodInfo> methods = GetMethodsWithButtonAttribute(target.GetType());

            foreach (MethodInfo method in methods)
            {
                var buttonAttribute = method.GetCustomAttribute<ButtonAttribute>();
                if (buttonAttribute == null)
                {
                    continue;
                }

                float buttonHeight = GetButtonHeight(buttonAttribute.ButtonSize);
                Rect buttonRect = new Rect(rect.x, rect.y, rect.width, buttonHeight);

                string buttonText = string.IsNullOrWhiteSpace(buttonAttribute.Name)
                    ? ObjectNames.NicifyVariableName(method.Name)
                    : buttonAttribute.Name;

                if (GUI.Button(buttonRect, buttonText))
                {
                    InvokeMethod(target, method);
                }

                rect.y += buttonHeight + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        public static float GetButtonsHeight(object target)
        {
            if (target == null)
            {
                return 0;
            }

            List<MethodInfo> methods = GetMethodsWithButtonAttribute(target.GetType());
            float totalHeight = 0;

            foreach (MethodInfo method in methods)
            {
                var buttonAttribute = method.GetCustomAttribute<ButtonAttribute>();
                if (buttonAttribute == null)
                {
                    continue;
                }

                totalHeight += GetButtonHeight(buttonAttribute.ButtonSize) + EditorGUIUtility.standardVerticalSpacing;
            }

            return totalHeight;
        }

        private static List<MethodInfo> GetMethodsWithButtonAttribute(Type targetType)
        {
            if (_cachedMethods.TryGetValue(targetType, out List<MethodInfo> methods))
            {
                return methods;
            }

            methods = targetType
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.GetCustomAttribute<ButtonAttribute>() != null)
                .ToList();

            _cachedMethods[targetType] = methods;

            return methods;
        }

        private static float GetButtonHeight(ButtonSize size)
        {
            return size switch
            {
                ButtonSize.Small => EditorGUIUtility.singleLineHeight,
                ButtonSize.Medium => EditorGUIUtility.singleLineHeight * 1.5f,
                ButtonSize.Large => EditorGUIUtility.singleLineHeight * 2f,
                _ => EditorGUIUtility.singleLineHeight
            };
        }

        private static void InvokeMethod(object target, MethodInfo method)
        {
            try
            {
                ParameterInfo[] parameters = method.GetParameters();

                if (parameters.Length == 0)
                {
                    method.Invoke(target, null);
                }
                else
                {
                    bool allOptional = parameters.All(p => p.HasDefaultValue);

                    if (allOptional)
                    {
                        object[] defaultParams = parameters.Select(p => p.DefaultValue).ToArray();
                        method.Invoke(target, defaultParams);
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"Button method '{method.Name}' has required parameters. Only methods with no parameters or optional parameters are supported.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error invoking button method '{method.Name}': {ex.Message}");
            }
        }
    }
}
#endif
