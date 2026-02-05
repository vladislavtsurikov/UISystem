#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;

namespace VladislavTsurikov.CustomInspector.Editor.Collections
{
    public sealed class HashSetFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field) =>
            field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(HashSet<>);

        public override Type DrawerType => typeof(HashSetFieldDrawer);
    }

    public sealed class HashSetFieldDrawer : IMGUIFieldDrawer
    {
        private readonly IMGUIInspectorFieldsDrawer _fieldsDrawer = new IMGUIInspectorFieldsDrawer(
            new List<Type>()
        );

        private Type _elementType;
        private GUIContent _label;
        private object _hashSet;
        private List<object> _elements;
        private object _newElement;
        private IMGUIFieldDrawer _elementDrawer;
        private FieldInfo _field;
        private string _foldoutKey;
        private string _isAddingElementKey;

        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            _label = label;
            _hashSet = value;
            _field = field;

            if (value == null)
            {
                EditorGUI.LabelField(rect, label, new GUIContent("HashSet is null"));
                return value;
            }

            _elementType = field.FieldType.GetGenericArguments()[0];
            _elementDrawer = FieldDrawerResolver<IMGUIFieldDrawer>.CreateDrawer(_elementType);

            _foldoutKey = $"HashSetFieldDrawer_{field.DeclaringType?.FullName}_{field.Name}_Foldout";
            _isAddingElementKey = $"HashSetFieldDrawer_{field.DeclaringType?.FullName}_{field.Name}_IsAdding";

            bool currentFoldout = SessionState.GetBool(_foldoutKey, false);
            bool isAddingElement = SessionState.GetBool(_isAddingElementKey, false);

            if (_elements == null)
            {
                _elements = new List<object>();
            }

            _elements.Clear();
            foreach (object element in (System.Collections.IEnumerable)value)
            {
                _elements.Add(element);
            }

            var setType = typeof(HashSet<>).MakeGenericType(_elementType);
            var countProperty = setType.GetProperty("Count");
            int count = countProperty != null ? (int)countProperty.GetValue(value) : _elements.Count;

            Rect headerRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

            float addButtonWidth = 25;
            string countText = $"{count} items";
            Vector2 countSize = EditorStyles.label.CalcSize(new GUIContent(countText));

            // Вычисляем ширину для foldout, исключая кнопку и счётчик
            float rightReservedWidth = addButtonWidth + countSize.x + 15;
            Rect foldoutRect = new Rect(headerRect.x, headerRect.y, headerRect.width - rightReservedWidth, headerRect.height);

            bool newFoldout = EditorGUI.Foldout(foldoutRect, currentFoldout, label.text, true);
            if (newFoldout != currentFoldout)
            {
                SessionState.SetBool(_foldoutKey, newFoldout);
                currentFoldout = newFoldout;
                GUI.changed = true;
            }

            // Рисуем счётчик и кнопку справа
            Rect countRect = new Rect(headerRect.xMax - addButtonWidth - countSize.x - 10, headerRect.y, countSize.x, headerRect.height);
            EditorGUI.LabelField(countRect, countText);

            Rect addButtonRect = new Rect(headerRect.xMax - addButtonWidth, headerRect.y, addButtonWidth, headerRect.height);
            if (GUI.Button(addButtonRect, "+"))
            {
                isAddingElement = !isAddingElement;
                SessionState.SetBool(_isAddingElementKey, isAddingElement);
                if (isAddingElement)
                {
                    _newElement = FieldUtility.GetOrCreateTypeInstance(_elementType, _elementDrawer);
                }
                GUI.changed = true;
            }

            if (!currentFoldout)
            {
                return value;
            }

            Rect contentRect = new Rect(rect.x,
                rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                rect.width,
                rect.height - EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing);

            EditorGUI.indentLevel++;

            if (isAddingElement)
            {
                float addFormHeight = GetAddFormHeight();
                Rect addFormRect = new Rect(contentRect.x, contentRect.y, contentRect.width, addFormHeight);

                DrawAddForm(addFormRect, setType, value);

                contentRect.y += addFormHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            var removeMethod = setType.GetMethod("Remove", new[] { _elementType });
            var addMethod = setType.GetMethod("Add", new[] { _elementType });
            var containsMethod = setType.GetMethod("Contains", new[] { _elementType });

            object elementToRemove = null;
            bool shouldRemove = false;

            float currentY = contentRect.y;
            for (int i = 0; i < _elements.Count; i++)
            {
                float elementHeight = GetElementHeight(i);
                float deleteButtonWidth = 20;
                Rect elementRect = new Rect(contentRect.x, currentY, contentRect.width - deleteButtonWidth - 4, elementHeight);

                GUI.Box(elementRect, GUIContent.none, EditorStyles.helpBox);

                Rect removeButtonRect = new Rect(elementRect.xMax + 4, elementRect.y + 2, deleteButtonWidth, 18);
                if (GUI.Button(removeButtonRect, "X"))
                {
                    elementToRemove = _elements[i];
                    shouldRemove = true;
                }

                Rect fieldRect = new Rect(elementRect.x + 4, elementRect.y + 2, elementRect.width - 8, elementRect.height - 4);
                object newElement = DrawElementField(fieldRect, _elements[i]);

                if (!Equals(newElement, _elements[i]))
                {
                    bool containsNew = containsMethod != null && (bool)containsMethod.Invoke(value, new[] { newElement });
                    if (!containsNew)
                    {
                        removeMethod?.Invoke(value, new[] { _elements[i] });
                        addMethod?.Invoke(value, new[] { newElement });
                    }
                    else
                    {
                        Debug.LogWarning("Element already exists in HashSet.");
                    }
                }

                currentY += elementHeight + 2;
            }

            if (shouldRemove && elementToRemove != null)
            {
                removeMethod?.Invoke(value, new[] { elementToRemove });
            }

            EditorGUI.indentLevel--;

            return value;
        }

        public override float GetFieldsHeight(object target, FieldInfo field, object value)
        {
            float headerHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            if (target == null)
            {
                return headerHeight;
            }

            if (_field != null)
            {
                _foldoutKey = $"HashSetFieldDrawer_{_field.DeclaringType?.FullName}_{_field.Name}_Foldout";
                _isAddingElementKey = $"HashSetFieldDrawer_{_field.DeclaringType?.FullName}_{_field.Name}_IsAdding";
            }

            bool currentFoldout = SessionState.GetBool(_foldoutKey, false);
            if (!currentFoldout)
            {
                return headerHeight;
            }

            RefreshElements(target);

            float contentHeight = 0f;

            bool isAddingElement = SessionState.GetBool(_isAddingElementKey, false);
            if (isAddingElement)
            {
                contentHeight += GetAddFormHeight() + EditorGUIUtility.standardVerticalSpacing;
            }

            for (int i = 0; i < _elements.Count; i++)
            {
                contentHeight += GetElementHeight(i) + 2;
            }

            if (contentHeight <= 0f)
            {
                contentHeight = EditorGUIUtility.singleLineHeight;
            }

            return headerHeight + contentHeight;
        }

        private float GetElementHeight(int index)
        {
            if (index < 0 || index >= _elements.Count)
            {
                return EditorGUIUtility.singleLineHeight + 4;
            }

            object element = _elements[index];
            return GetElementFieldHeight(element) + 4;
        }

        private float GetAddFormHeight()
        {
            float elementHeight = GetElementFieldHeight(_newElement);
            float headerHeight = EditorGUIUtility.singleLineHeight;
            return 8 + Math.Max(headerHeight, elementHeight) + 4;
        }

        private void DrawAddForm(Rect rect, Type setType, object setInstance)
        {
            GUI.Box(rect, GUIContent.none, EditorStyles.helpBox);

            float currentY = rect.y + 4;
            float buttonWidth = 25;
            float spacing = 4;

            Rect okButtonRect = new Rect(rect.xMax - buttonWidth - spacing - buttonWidth - spacing, currentY, buttonWidth, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(okButtonRect, "✓"))
            {
                var addMethod = setType.GetMethod("Add");
                var containsMethod = setType.GetMethod("Contains", new[] { _elementType });

                bool exists = containsMethod != null && (bool)containsMethod.Invoke(setInstance, new[] { _newElement });
                if (!exists)
                {
                    try
                    {
                        addMethod?.Invoke(setInstance, new[] { _newElement });
                        SessionState.SetBool(_isAddingElementKey, false);
                        GUI.changed = true;
                    }
                    catch
                    {
                        Debug.LogWarning("Failed to add element.");
                    }
                }
                else
                {
                    Debug.LogWarning("Element already exists in HashSet.");
                }
            }

            Rect cancelButtonRect = new Rect(rect.xMax - buttonWidth - spacing, currentY, buttonWidth, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(cancelButtonRect, "X"))
            {
                SessionState.SetBool(_isAddingElementKey, false);
                GUI.changed = true;
            }

            Rect labelRect = new Rect(rect.x + 4, currentY, 70, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, "Element:");

            float elementHeight = GetElementFieldHeight(_newElement);
            Rect fieldRect = new Rect(labelRect.xMax + 4, currentY, okButtonRect.x - labelRect.xMax - 8, elementHeight);
            _newElement = DrawElementField(fieldRect, _newElement);
        }

        private float GetElementFieldHeight(object value)
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(_elementType) || IsSimpleType(_elementType))
            {
                return EditorGUIUtility.singleLineHeight;
            }

            if (value == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return _fieldsDrawer.GetFieldsHeight(value);
        }

        private object DrawElementField(Rect rect, object value)
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(_elementType))
            {
                return EditorGUI.ObjectField(rect, value as UnityEngine.Object, _elementType, true);
            }

            if (IsSimpleType(_elementType))
            {
                return DrawSimpleField(rect, _elementType, value);
            }

            if (value == null)
            {
                EditorGUI.LabelField(rect, "Element is null");
                return value;
            }

            _fieldsDrawer.DrawFields(value, rect);
            return value;
        }

        private bool IsSimpleType(Type type)
        {
            return type == typeof(string) ||
                   type == typeof(int) ||
                   type == typeof(float) ||
                   type == typeof(bool) ||
                   type == typeof(Vector2) ||
                   type == typeof(Vector3) ||
                   type.IsEnum;
        }

        private object DrawSimpleField(Rect rect, Type fieldType, object value)
        {
            if (fieldType == typeof(string))
            {
                return EditorGUI.TextField(rect, value as string ?? string.Empty);
            }

            if (fieldType == typeof(int))
            {
                return EditorGUI.IntField(rect, value != null ? (int)value : 0);
            }

            if (fieldType == typeof(float))
            {
                return EditorGUI.FloatField(rect, value != null ? (float)value : 0f);
            }

            if (fieldType == typeof(bool))
            {
                return EditorGUI.Toggle(rect, value != null && (bool)value);
            }

            if (fieldType.IsEnum)
            {
                return EditorGUI.EnumPopup(rect, value as Enum ?? (Enum)Enum.GetValues(fieldType).GetValue(0));
            }

            if (fieldType == typeof(Vector2))
            {
                return EditorGUI.Vector2Field(rect, GUIContent.none, value != null ? (Vector2)value : Vector2.zero);
            }

            if (fieldType == typeof(Vector3))
            {
                return EditorGUI.Vector3Field(rect, GUIContent.none, value != null ? (Vector3)value : Vector3.zero);
            }

            return value;
        }

        private void RefreshElements(object target)
        {
            if (target == null)
            {
                return;
            }

            Type fieldType = target.GetType();
            _elementType = fieldType.GetGenericArguments()[0];
            _elementDrawer = FieldDrawerResolver<IMGUIFieldDrawer>.CreateDrawer(_elementType);

            if (_elements == null)
            {
                _elements = new List<object>();
            }

            _elements.Clear();
            foreach (object element in (System.Collections.IEnumerable)target)
            {
                _elements.Add(element);
            }
        }
    }
}
#endif


