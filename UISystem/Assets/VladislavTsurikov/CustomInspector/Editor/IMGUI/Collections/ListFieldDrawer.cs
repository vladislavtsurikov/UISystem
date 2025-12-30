#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;

namespace VladislavTsurikov.CustomInspector.Editor.Collections
{
    public sealed class ListFieldDrawer : IMGUIFieldDrawer
    {
        private readonly IMGUIInspectorFieldsDrawer _fieldsDrawer = new IMGUIInspectorFieldsDrawer(
            new List<Type>()
        );

        private ReorderableList _reorderableList;
        private IList _list;
        private Type _elementType;
        private GUIContent _label;

        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value)
        {
            IList list = value as IList;

            Setup(list, fieldType, label);
            _reorderableList.DoList(rect);

            return value;
        }

        public override float GetFieldsHeight(object target)
        {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        private void Setup(IList list, Type fieldType, GUIContent label)
        {
            _label = label;

            if (_reorderableList == null)
            {
                _list = list;
                _elementType = fieldType.GetGenericArguments()[0];

                _reorderableList = new ReorderableList(_list, _elementType, true, true, true, true);
                _reorderableList.drawHeaderCallback = DrawHeader;
                _reorderableList.elementHeightCallback = GetElementHeight;
                _reorderableList.drawElementCallback = DrawElement;
                _reorderableList.onAddCallback = OnAdd;

                return;
            }

            if (ReferenceEquals(_list, list) == false)
            {
                _list = list;
                _reorderableList.list = _list;
            }

            _elementType = fieldType.GetGenericArguments()[0];
        }

        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, _label);
        }

        private float GetElementHeight(int index)
        {
            if (_elementType == null)
            {
                return EditorGUIUtility.singleLineHeight + 4;
            }

            if (typeof(UnityEngine.Object).IsAssignableFrom(_elementType))
            {
                return EditorGUIUtility.singleLineHeight + 4;
            }

            object element = index >= 0 && index < _list.Count ? _list[index] : null;
            object target = EnsureElementInstance(_elementType, element);

            if (target == null)
            {
                return EditorGUIUtility.singleLineHeight + 4;
            }

            return _fieldsDrawer.GetFieldsHeight(target) + 4;
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index < 0 || index >= _list.Count)
            {
                return;
            }

            Rect contentRect = new Rect(rect.x, rect.y + 1, rect.width, rect.height - 2);

            object element = _list[index];

            if (typeof(UnityEngine.Object).IsAssignableFrom(_elementType))
            {
                UnityEngine.Object obj = element as UnityEngine.Object;
                UnityEngine.Object newObj = EditorGUI.ObjectField(
                    new Rect(contentRect.x, contentRect.y, contentRect.width, EditorGUIUtility.singleLineHeight),
                    GUIContent.none,
                    obj,
                    _elementType,
                    false);

                _list[index] = newObj;
                return;
            }

            object target = EnsureElementInstance(_elementType, element);
            if (target == null)
            {
                EditorGUI.LabelField(
                    new Rect(contentRect.x, contentRect.y, contentRect.width, EditorGUIUtility.singleLineHeight),
                    "Element is null and cannot be created");
                return;
            }

            float height = _fieldsDrawer.GetFieldsHeight(target);
            Rect fieldsRect = new Rect(contentRect.x, contentRect.y, contentRect.width, height);
            _fieldsDrawer.DrawFields(target, fieldsRect);

            _list[index] = target;
        }

        private void OnAdd(ReorderableList list)
        {
            object newElement = CreateDefaultElement(_elementType);
            _list.Add(newElement);
        }

        private static object CreateDefaultElement(Type elementType)
        {
            if (elementType == null)
            {
                return null;
            }

            if (typeof(UnityEngine.Object).IsAssignableFrom(elementType))
            {
                return null;
            }

            if (elementType.IsValueType)
            {
                return Activator.CreateInstance(elementType);
            }

            if (elementType == typeof(string))
            {
                return string.Empty;
            }

            if (elementType.GetConstructor(Type.EmptyTypes) != null)
            {
                return Activator.CreateInstance(elementType);
            }

            return null;
        }

        private static object EnsureElementInstance(Type elementType, object value)
        {
            if (value != null)
            {
                return value;
            }

            return CreateDefaultElement(elementType);
        }

        public override bool CanDraw(Type fieldType)
        {
            return fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>);
        }
    }
}
#endif
