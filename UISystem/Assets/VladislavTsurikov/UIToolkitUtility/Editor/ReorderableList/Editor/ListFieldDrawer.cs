#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.UIToolkitReorderableList.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit.Collections
{
    public sealed class ListFieldDrawerMatcher : FieldDrawerMatcher<UIToolkitFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field) =>
            field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>);

        public override Type DrawerType => typeof(ListFieldDrawer);
    }

    public sealed class ListFieldDrawer : UIToolkitFieldDrawer
    {
        private readonly UIToolkitInspectorFieldsDrawer _fieldsDrawer = new UIToolkitInspectorFieldsDrawer(
            new List<Type>()
        );

        private ReorderableList _reorderableList;
        private Type _elementType;

        public override VisualElement CreateField(string label, Type fieldType, object value, Action<object> onValueChanged)
        {
            IList list = value as IList;
            _elementType = fieldType.GetGenericArguments()[0];

            _reorderableList = new ReorderableList
            {
                HeaderTitle = label,
                ShowHeader = true,
                ShowFooter = true,
                ShowAddButton = true,
                ShowRemoveButton = true,
                Reorderable = true
            };

            _reorderableList.SetItemType(_elementType);
            _reorderableList.MakeItem = MakeListItem;
            _reorderableList.BindItem = BindListItem;
            _reorderableList.OnAdd = OnAddItem;
            _reorderableList.OnRemove = OnRemoveItem;
            _reorderableList.OnReorder = () => onValueChanged?.Invoke(list);

            if (list != null)
            {
                _reorderableList.ItemsSource = list;
            }

            return _reorderableList;
        }

        private VisualElement MakeListItem()
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(_elementType))
            {
                return CreateUnityObjectField();
            }

            if (IsPrimitiveType(_elementType))
            {
                return CreatePrimitiveField(_elementType);
            }

            return CreateComplexObjectField();
        }

        private void BindListItem(VisualElement element, int index)
        {
            IList list = _reorderableList.ItemsSource;
            if (list == null || index < 0 || index >= list.Count)
            {
                return;
            }

            object item = list[index];

            if (typeof(UnityEngine.Object).IsAssignableFrom(_elementType))
            {
                BindUnityObjectField(element, item, index);
            }
            else if (IsPrimitiveType(_elementType))
            {
                BindPrimitiveField(element, item, index);
            }
            else
            {
                BindComplexObjectField(element, item, index);
            }
        }

        private VisualElement CreateUnityObjectField()
        {
            var container = new VisualElement();
            container.style.paddingTop = 2;
            container.style.paddingBottom = 2;
            return container;
        }

        private void BindUnityObjectField(VisualElement element, object value, int index)
        {
            element.Clear();

            var objectField = new UnityEditor.UIElements.ObjectField
            {
                objectType = _elementType,
                value = value as UnityEngine.Object,
                allowSceneObjects = true
            };

            objectField.RegisterValueChangedCallback(evt =>
            {
                IList list = _reorderableList.ItemsSource;
                if (list != null && index >= 0 && index < list.Count)
                {
                    list[index] = evt.newValue;
                }
            });

            element.Add(objectField);
        }

        private VisualElement CreatePrimitiveField(Type type)
        {
            var container = new VisualElement();
            container.style.paddingTop = 2;
            container.style.paddingBottom = 2;
            return container;
        }

        private void BindPrimitiveField(VisualElement element, object value, int index)
        {
            element.Clear();

            UIToolkitFieldDrawer drawer = FieldDrawerResolver<UIToolkitFieldDrawer>.CreateDrawer(_elementType);
            if (drawer != null)
            {
                var field = drawer.CreateField("", _elementType, value, newValue =>
                {
                    IList list = _reorderableList.ItemsSource;
                    if (list != null && index >= 0 && index < list.Count)
                    {
                        list[index] = newValue;
                    }
                });

                element.Add(field);
            }
        }

        private VisualElement CreateComplexObjectField()
        {
            var container = new VisualElement();
            container.style.paddingTop = 4;
            container.style.paddingBottom = 4;
            container.style.paddingLeft = 4;
            container.style.paddingRight = 4;
            return container;
        }

        private void BindComplexObjectField(VisualElement element, object value, int index)
        {
            element.Clear();

            if (value == null)
            {
                value = CreateDefaultInstance(_elementType);
                IList list = _reorderableList.ItemsSource;
                if (list != null && index >= 0 && index < list.Count)
                {
                    list[index] = value;
                }
            }

            if (value != null)
            {
                var fieldsContainer = _fieldsDrawer.CreateFieldsContainer(value);
                element.Add(fieldsContainer);
            }
        }

        private void OnAddItem(IList list)
        {
            object newItem = CreateDefaultInstance(_elementType);
            list.Add(newItem);
            _reorderableList.Refresh();
        }

        private void OnRemoveItem(int index)
        {
            IList list = _reorderableList.ItemsSource;
            if (list != null && index >= 0 && index < list.Count)
            {
                list.RemoveAt(index);
                _reorderableList.Refresh();
            }
        }

        private static bool IsPrimitiveType(Type type)
        {
            return type.IsPrimitive ||
                   type == typeof(string) ||
                   type == typeof(decimal) ||
                   type.IsEnum ||
                   type == typeof(Vector2) ||
                   type == typeof(Vector3) ||
                   type == typeof(Vector4) ||
                   type == typeof(Vector2Int) ||
                   type == typeof(Vector3Int) ||
                   type == typeof(Color) ||
                   type == typeof(Rect) ||
                   type == typeof(RectInt) ||
                   type == typeof(Bounds) ||
                   type == typeof(BoundsInt);
        }

        private static object CreateDefaultInstance(Type type)
        {
            if (type == null)
            {
                return null;
            }

            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                return null;
            }

            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            if (type == typeof(string))
            {
                return string.Empty;
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
