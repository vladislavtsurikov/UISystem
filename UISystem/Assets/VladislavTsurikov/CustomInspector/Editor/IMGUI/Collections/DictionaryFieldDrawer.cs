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
    public sealed class DictionaryFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field) =>
            field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>);

        public override Type DrawerType => typeof(DictionaryFieldDrawer);
    }

    public sealed class DictionaryFieldDrawer : IMGUIFieldDrawer
    {
        private readonly IMGUIInspectorFieldsDrawer _fieldsDrawer = new IMGUIInspectorFieldsDrawer(
            new List<Type>()
        );

        private Type _keyType;
        private Type _valueType;
        private GUIContent _label;
        private object _dictionary;
        private List<object> _keys;
        private List<object> _values;
        private Vector2 _scrollPosition;
        private bool _isAddingElement;
        private object _newKey;
        private object _newValue;
        private bool _foldout;

        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            _label = label;
            _dictionary = value;

            if (value == null)
            {
                EditorGUI.LabelField(rect, label, new GUIContent("Dictionary is null"));
                return value;
            }

            Type[] genericArgs = field.FieldType.GetGenericArguments();
            _keyType = genericArgs[0];
            _valueType = genericArgs[1];

            var dictionaryType = typeof(Dictionary<,>).MakeGenericType(_keyType, _valueType);
            var keysProperty = dictionaryType.GetProperty("Keys");
            var valuesProperty = dictionaryType.GetProperty("Values");
            var countProperty = dictionaryType.GetProperty("Count");

            if (_keys == null)
            {
                _keys = new List<object>();
            }

            if (_values == null)
            {
                _values = new List<object>();
            }

            _keys.Clear();
            _values.Clear();

            var keys = keysProperty.GetValue(value);
            var values = valuesProperty.GetValue(value);
            int count = (int)countProperty.GetValue(value);

            var keysEnumerator = ((System.Collections.IEnumerable)keys).GetEnumerator();
            var valuesEnumerator = ((System.Collections.IEnumerable)values).GetEnumerator();

            while (keysEnumerator.MoveNext() && valuesEnumerator.MoveNext())
            {
                _keys.Add(keysEnumerator.Current);
                _values.Add(valuesEnumerator.Current);
            }

            Rect headerRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

            float addButtonWidth = 25;
            Rect addButtonRect = new Rect(headerRect.xMax - addButtonWidth, headerRect.y, addButtonWidth, headerRect.height);
            if (GUI.Button(addButtonRect, "+"))
            {
                _isAddingElement = !_isAddingElement;
                if (_isAddingElement)
                {
                    _newKey = CreateDefaultValue(_keyType);
                    _newValue = CreateDefaultValue(_valueType);
                }
            }

            string countText = $"{count} items";
            Vector2 countSize = EditorStyles.label.CalcSize(new GUIContent(countText));
            Rect countRect = new Rect(headerRect.xMax - addButtonWidth - countSize.x - 10, headerRect.y, countSize.x, headerRect.height);
            EditorGUI.LabelField(countRect, countText);

            Rect foldoutRect = new Rect(headerRect.x, headerRect.y, countRect.x - headerRect.x - 5, headerRect.height);
            _foldout = EditorGUI.Foldout(foldoutRect, _foldout, label.text, true);

            if (!_foldout)
            {
                return value;
            }

            Rect contentRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, rect.width, rect.height - EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing);

            EditorGUI.indentLevel++;

            if (_isAddingElement)
            {
                float addFormHeight = GetAddFormHeight();
                Rect addFormRect = new Rect(contentRect.x, contentRect.y, contentRect.width, addFormHeight);

                DrawAddForm(addFormRect, dictionaryType, value);

                contentRect.y += addFormHeight + EditorGUIUtility.standardVerticalSpacing;
                contentRect.height -= addFormHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            float scrollViewHeight = contentRect.height;
            float totalContentHeight = CalculateTotalContentHeight();

            Rect scrollViewRect = new Rect(contentRect.x, contentRect.y, contentRect.width, scrollViewHeight);
            Rect viewRect = new Rect(0, 0, contentRect.width - 20, totalContentHeight);

            _scrollPosition = GUI.BeginScrollView(scrollViewRect, _scrollPosition, viewRect);

            var removeMethod = dictionaryType.GetMethod("Remove", new[] { _keyType });
            var addMethod2 = dictionaryType.GetMethod("Add");
            var containsKeyMethod = dictionaryType.GetMethod("ContainsKey");
            var indexer = dictionaryType.GetProperty("Item");

            object keyToRemove = null;
            bool shouldRemove = false;

            float currentY = 0;
            for (int i = 0; i < _keys.Count; i++)
            {
                float elementHeight = GetElementHeight(i);
                float deleteButtonWidth = 20;
                Rect elementRect = new Rect(0, currentY, viewRect.width - deleteButtonWidth - 4, elementHeight);

                GUI.Box(elementRect, GUIContent.none, EditorStyles.helpBox);

                float keyHeight = GetFieldHeight(_keyType, _keys[i]);
                float valueHeight = GetFieldHeight(_valueType, _values[i]);

                Rect removeButtonRect = new Rect(elementRect.xMax + 4, elementRect.y + 2, deleteButtonWidth, 18);
                if (GUI.Button(removeButtonRect, "×"))
                {
                    keyToRemove = _keys[i];
                    shouldRemove = true;
                }

                Rect keyFieldRect = new Rect(elementRect.x + 4, elementRect.y + 2, elementRect.width - 8, keyHeight);
                object newKey = DrawField(keyFieldRect, _keyType, _keys[i]);

                Rect valueFieldRect = new Rect(elementRect.x + 8, keyFieldRect.yMax + 2, elementRect.width - 12, valueHeight);
                object newValue = DrawField(valueFieldRect, _valueType, _values[i]);

                if (!Equals(newKey, _keys[i]))
                {
                    removeMethod.Invoke(value, new[] { _keys[i] });

                    bool keyExists = (bool)containsKeyMethod.Invoke(value, new[] { newKey });
                    if (!keyExists)
                    {
                        addMethod2.Invoke(value, new[] { newKey, newValue });
                    }
                    else
                    {
                        addMethod2.Invoke(value, new[] { _keys[i], _values[i] });
                        Debug.LogWarning("Key already exists. Cannot change key.");
                    }
                }
                else if (!Equals(newValue, _values[i]))
                {
                    indexer.SetValue(value, newValue, new[] { newKey });
                }

                currentY += elementHeight + 2;
            }

            if (shouldRemove && keyToRemove != null)
            {
                removeMethod.Invoke(value, new[] { keyToRemove });
            }

            GUI.EndScrollView();

            EditorGUI.indentLevel--;

            return value;
        }

        public override float GetFieldsHeight(object target, FieldInfo field, object value)
        {
            if (target == null)
            {
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            if (!_foldout)
            {
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            height += 200;

            return height;
        }

        private float CalculateTotalContentHeight()
        {
            float totalHeight = 0;
            for (int i = 0; i < _keys.Count; i++)
            {
                totalHeight += GetElementHeight(i) + 2;
            }
            return totalHeight;
        }

        private float GetElementHeight(int index)
        {
            if (index < 0 || index >= _keys.Count)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            float keyHeight = GetFieldHeight(_keyType, _keys[index]);
            float valueHeight = GetFieldHeight(_valueType, _values[index]);

            float height = 4 + keyHeight + 2 + valueHeight + 2;

            return height;
        }

        private float GetFieldHeight(Type fieldType, object value)
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(fieldType))
            {
                return EditorGUIUtility.singleLineHeight;
            }

            if (IsSimpleType(fieldType))
            {
                return EditorGUIUtility.singleLineHeight;
            }

            object target = EnsureFieldInstance(fieldType, value);
            if (target == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return _fieldsDrawer.GetFieldsHeight(target);
        }

        private object DrawField(Rect rect, Type fieldType, object value)
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(fieldType))
            {
                return EditorGUI.ObjectField(rect, value as UnityEngine.Object, fieldType, true);
            }

            if (IsSimpleType(fieldType))
            {
                return DrawSimpleField(rect, fieldType, value);
            }

            object target = EnsureFieldInstance(fieldType, value);
            if (target == null)
            {
                EditorGUI.LabelField(rect, $"Cannot create instance of {fieldType.Name}");
                return value;
            }

            _fieldsDrawer.DrawFields(target, rect);
            return target;
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

        private float GetAddFormHeight()
        {
            float keyHeight = GetFieldHeight(_keyType, _newKey);
            float valueHeight = GetFieldHeight(_valueType, _newValue);

            return 8 + EditorGUIUtility.singleLineHeight + keyHeight + 4 + EditorGUIUtility.singleLineHeight + valueHeight + 4;
        }

        private void DrawAddForm(Rect rect, Type dictionaryType, object dictionary)
        {
            GUI.Box(rect, GUIContent.none, EditorStyles.helpBox);

            float currentY = rect.y + 4;
            float buttonWidth = 25;
            float spacing = 4;

            Rect confirmButtonRect = new Rect(rect.xMax - buttonWidth - spacing - buttonWidth - spacing, currentY, buttonWidth, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(confirmButtonRect, "✓"))
            {
                var addMethod = dictionaryType.GetMethod("Add");
                var containsKeyMethod = dictionaryType.GetMethod("ContainsKey");

                bool keyExists = (bool)containsKeyMethod.Invoke(dictionary, new[] { _newKey });
                if (!keyExists)
                {
                    try
                    {
                        addMethod.Invoke(dictionary, new[] { _newKey, _newValue });
                        _isAddingElement = false;
                    }
                    catch
                    {
                        Debug.LogWarning("Failed to add element.");
                    }
                }
                else
                {
                    Debug.LogWarning("Key already exists.");
                }
            }

            Rect cancelButtonRect = new Rect(rect.xMax - buttonWidth - spacing, currentY, buttonWidth, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(cancelButtonRect, "×"))
            {
                _isAddingElement = false;
            }

            Rect keyLabelRect = new Rect(rect.x + 4, currentY, 40, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(keyLabelRect, "Key:");

            float keyHeight = GetFieldHeight(_keyType, _newKey);
            Rect keyFieldRect = new Rect(keyLabelRect.xMax + 4, currentY, confirmButtonRect.x - keyLabelRect.xMax - 8, keyHeight);
            _newKey = DrawField(keyFieldRect, _keyType, _newKey);

            currentY += keyHeight + 4;

            Rect valueLabelRect = new Rect(rect.x + 4, currentY, 40, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(valueLabelRect, "Value:");

            float valueHeight = GetFieldHeight(_valueType, _newValue);
            Rect valueFieldRect = new Rect(valueLabelRect.xMax + 4, currentY, rect.width - valueLabelRect.xMax - 8, valueHeight);
            _newValue = DrawField(valueFieldRect, _valueType, _newValue);
        }

        private static object EnsureFieldInstance(Type fieldType, object value)
        {
            if (value != null)
            {
                return value;
            }

            return CreateDefaultValue(fieldType);
        }

        private static object CreateDefaultValue(Type type)
        {
            if (type == typeof(string))
            {
                return string.Empty;
            }

            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                return null;
            }

            if (type.GetConstructor(Type.EmptyTypes) != null)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }
    }
}
#endif


