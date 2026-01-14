#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;

namespace VladislavTsurikov.CustomInspector.Editor.Collections
{
    public sealed class DictionaryFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) =>
            fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>);

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
        private object _newKey;
        private object _newValue;

        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value)
        {
            _label = label;
            _dictionary = value;

            if (value == null)
            {
                EditorGUI.LabelField(rect, label, new GUIContent("Dictionary is null"));
                return value;
            }

            Type[] genericArgs = fieldType.GetGenericArguments();
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

            Foldout = EditorGUI.Foldout(headerRect, Foldout, $"{label.text} (Count: {count})", true);

            if (!Foldout)
            {
                return value;
            }

            Rect contentRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, rect.width, rect.height - EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing);

            EditorGUI.indentLevel++;

            if (_newKey == null || _newKey.GetType() != _keyType)
            {
                _newKey = CreateDefaultValue(_keyType);
            }

            if (_newValue == null || _newValue.GetType() != _valueType)
            {
                _newValue = CreateDefaultValue(_valueType);
            }

            float labelHeight = EditorGUIUtility.singleLineHeight;
            float keyFieldHeight = GetFieldHeight(_keyType, _newKey);
            float valueFieldHeight = GetFieldHeight(_valueType, _newValue);
            float fieldsHeight = Mathf.Max(keyFieldHeight, valueFieldHeight);
            float addRowHeight = labelHeight + fieldsHeight;

            float addButtonWidth = 60f;
            float columnSpacing = 6f;
            float columnsWidth = contentRect.width - addButtonWidth - columnSpacing;
            float columnWidth = (columnsWidth - columnSpacing) / 2f;

            Rect keyLabelRect = new Rect(contentRect.x, contentRect.y, columnWidth, labelHeight);
            Rect valueLabelRect = new Rect(keyLabelRect.xMax + columnSpacing, contentRect.y, columnWidth, labelHeight);
            EditorGUI.LabelField(keyLabelRect, "Key");
            EditorGUI.LabelField(valueLabelRect, "Value");

            Rect keyFieldRect = new Rect(contentRect.x, contentRect.y + labelHeight, columnWidth, keyFieldHeight);
            Rect valueFieldRect = new Rect(valueLabelRect.x, contentRect.y + labelHeight, columnWidth, valueFieldHeight);
            _newKey = DrawField(keyFieldRect, _keyType, _newKey);
            _newValue = DrawField(valueFieldRect, _valueType, _newValue);

            float addButtonHeight = EditorGUIUtility.singleLineHeight;
            float addButtonY = contentRect.y + labelHeight + (fieldsHeight - addButtonHeight) / 2f;
            Rect addButtonRect = new Rect(valueLabelRect.xMax + columnSpacing, addButtonY, addButtonWidth, addButtonHeight);

            var addMethod = dictionaryType.GetMethod("Add");
            var containsKeyMethod = dictionaryType.GetMethod("ContainsKey");

            if (GUI.Button(addButtonRect, "Add"))
            {
                bool keyExists = (bool)containsKeyMethod.Invoke(value, new[] { _newKey });
                if (!keyExists)
                {
                    addMethod.Invoke(value, new[] { _newKey, _newValue });
                    _newKey = CreateDefaultValue(_keyType);
                    _newValue = CreateDefaultValue(_valueType);
                }
            }

            contentRect.y += addRowHeight + EditorGUIUtility.standardVerticalSpacing;
            contentRect.height -= addRowHeight + EditorGUIUtility.standardVerticalSpacing;

            float scrollViewHeight = contentRect.height;
            float totalContentHeight = CalculateTotalContentHeight();

            Rect scrollViewRect = new Rect(contentRect.x, contentRect.y, contentRect.width, scrollViewHeight);
            Rect viewRect = new Rect(0, 0, contentRect.width - 20, totalContentHeight);

            _scrollPosition = GUI.BeginScrollView(scrollViewRect, _scrollPosition, viewRect);

            float headerHeight = EditorGUIUtility.singleLineHeight;
            float columnSpacing = 6f;
            float removeButtonWidth = 30f;
            float keyColumnWidth = viewRect.width * 0.35f;
            float valueColumnWidth = viewRect.width - keyColumnWidth - removeButtonWidth - (columnSpacing * 2f);

            Rect headerKeyRect = new Rect(0, 0, keyColumnWidth, headerHeight);
            Rect headerValueRect = new Rect(keyColumnWidth + columnSpacing, 0, valueColumnWidth, headerHeight);

            EditorGUI.LabelField(headerKeyRect, "Key");
            EditorGUI.LabelField(headerValueRect, "Value");

            var removeMethod = dictionaryType.GetMethod("Remove", new[] { _keyType });
            var addMethod2 = dictionaryType.GetMethod("Add");
            var indexer = dictionaryType.GetProperty("Item");

            object keyToRemove = null;
            bool shouldRemove = false;

            float currentY = headerHeight + EditorGUIUtility.standardVerticalSpacing;
            for (int i = 0; i < _keys.Count; i++)
            {
                float elementHeight = GetElementHeight(i);
                float keyHeight = GetFieldHeight(_keyType, _keys[i]);
                float valueHeight = GetFieldHeight(_valueType, _values[i]);
                float rowHeight = Mathf.Max(keyHeight, valueHeight, EditorGUIUtility.singleLineHeight);

                Rect keyFieldRect = new Rect(0, currentY, keyColumnWidth, rowHeight);
                Rect valueFieldRect = new Rect(keyColumnWidth + columnSpacing, currentY, valueColumnWidth, rowHeight);
                float removeButtonY = currentY + ((rowHeight - EditorGUIUtility.singleLineHeight) / 2f);
                Rect removeButtonRect = new Rect(keyColumnWidth + columnSpacing + valueColumnWidth + columnSpacing, removeButtonY, removeButtonWidth, EditorGUIUtility.singleLineHeight);

                object newKey = DrawField(keyFieldRect, _keyType, _keys[i]);
                object newValue = DrawField(valueFieldRect, _valueType, _values[i]);

                if (GUI.Button(removeButtonRect, "X"))
                {
                    keyToRemove = _keys[i];
                    shouldRemove = true;
                }

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

                currentY += elementHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            if (shouldRemove && keyToRemove != null)
            {
                removeMethod.Invoke(value, new[] { keyToRemove });
            }

            GUI.EndScrollView();

            EditorGUI.indentLevel--;

            return value;
        }

        public override float GetFieldsHeight(object target)
        {
            if (target == null)
            {
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            if (!Foldout)
            {
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            float addRowHeight = EditorGUIUtility.singleLineHeight;
            if (_keyType != null && _valueType != null)
            {
                object keyValue = _newKey ?? CreateDefaultValue(_keyType);
                object valueValue = _newValue ?? CreateDefaultValue(_valueType);
                float keyHeight = GetFieldHeight(_keyType, keyValue);
                float valueHeight = GetFieldHeight(_valueType, valueValue);
                addRowHeight = EditorGUIUtility.singleLineHeight + Mathf.Max(keyHeight, valueHeight);
            }

            height += addRowHeight + EditorGUIUtility.standardVerticalSpacing;

            height += 200;

            return height;
        }

        private float CalculateTotalContentHeight()
        {
            float totalHeight = EditorGUIUtility.singleLineHeight;
            if (_keys.Count > 0)
            {
                totalHeight += EditorGUIUtility.standardVerticalSpacing;
            }
            for (int i = 0; i < _keys.Count; i++)
            {
                totalHeight += GetElementHeight(i) + EditorGUIUtility.standardVerticalSpacing;
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

            return Mathf.Max(keyHeight, valueHeight, EditorGUIUtility.singleLineHeight);
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
