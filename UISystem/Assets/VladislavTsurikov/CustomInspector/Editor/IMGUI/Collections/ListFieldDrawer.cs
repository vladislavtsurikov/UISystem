#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;

namespace VladislavTsurikov.CustomInspector.Editor.Collections
{
    public sealed class ListFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field)
        {
            if (!field.FieldType.IsGenericType)
            {
                return false;
            }

            if (typeof(IList).IsAssignableFrom(field.FieldType))
            {
                return true;
            }

            return field.FieldType.GetInterface("IList`1") != null;
        }

        public override Type DrawerType => typeof(ListFieldDrawer);
    }

    public sealed class ListFieldDrawer : IMGUIFieldDrawer
    {
        private readonly IMGUIInspectorFieldsDrawer _fieldsDrawer = new IMGUIInspectorFieldsDrawer(
            new List<Type>()
        );

        private ReorderableList _reorderableList;
        private IList _list;
        private Type _elementType;
        private IMGUIFieldDrawer _elementDrawer;
        private GUIContent _label;
        private FieldInfo _field;
        private object _target;

        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            IList list = value as IList;

            _field = field;
            _target = target;
            Setup(list, field.FieldType, label);
            _reorderableList.DoList(rect);

            return value;
        }

        public override float GetFieldsHeight(object target, FieldInfo field, object value)
        {
            if (target is not IList list)
            {
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            if (_reorderableList == null || !ReferenceEquals(_list, list))
            {
                Setup(list, target.GetType(), _label ?? GUIContent.none);
            }

            return _reorderableList != null
                ? _reorderableList.GetHeight()
                : EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        private void Setup(IList list, Type fieldType, GUIContent label)
        {
            _label = label;

            if (_reorderableList == null)
            {
                _list = list;
                _elementType = fieldType.GetGenericArguments()[0];
                _elementDrawer = FieldDrawerResolver<IMGUIFieldDrawer>.CreateDrawer(_elementType);

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
            _elementDrawer = FieldDrawerResolver<IMGUIFieldDrawer>.CreateDrawer(_elementType);
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

            object element = index >= 0 && index < _list.Count ? _list[index] : null;
            if (_elementDrawer != null)
            {
                return _elementDrawer.GetFieldsHeight(_target, _field, element) + 4;
            }

            if (element == null)
            {
                return EditorGUIUtility.singleLineHeight + 4;
            }

            return _fieldsDrawer.GetFieldsHeight(element) + 4;
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index < 0 || index >= _list.Count)
            {
                return;
            }

            Rect contentRect = new Rect(rect.x, rect.y + 1, rect.width, rect.height - 2);

            object element = _list[index];

            if (_elementDrawer != null)
            {
                object newElement = _elementDrawer.Draw(
                    new Rect(contentRect.x, contentRect.y, contentRect.width, contentRect.height),
                    GUIContent.none,
                    _field,
                    _target,
                    element);
                _list[index] = newElement;
                return;
            }

            if (element == null)
            {
                EditorGUI.LabelField(
                    new Rect(contentRect.x, contentRect.y, contentRect.width, EditorGUIUtility.singleLineHeight),
                    "Element is null");
                return;
            }

            float height = _fieldsDrawer.GetFieldsHeight(element);
            Rect fieldsRect = new Rect(contentRect.x, contentRect.y, contentRect.width, height);
            _fieldsDrawer.DrawFields(element, fieldsRect);

            _list[index] = element;
        }

        private void OnAdd(ReorderableList list)
        {
            object newElement = FieldUtility.GetOrCreateTypeInstance(_elementType, _elementDrawer);
            _list.Add(newElement);
        }

    }
}
#endif


