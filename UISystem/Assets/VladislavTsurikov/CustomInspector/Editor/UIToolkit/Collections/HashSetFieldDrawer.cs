#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public sealed class HashSetFieldDrawerMatcher : FieldDrawerMatcher<UIToolkitFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field) =>
            field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(HashSet<>);

        public override Type DrawerType => typeof(HashSetFieldDrawer);
    }

    public sealed class HashSetFieldDrawer : UIToolkitFieldDrawer
    {
        private readonly UIToolkitInspectorFieldsDrawer _fieldsDrawer = new UIToolkitInspectorFieldsDrawer(
            new List<Type>()
        );

        private Type _elementType;
        private Type _setType;
        private MethodInfo _addMethod;
        private MethodInfo _removeMethod;
        private MethodInfo _containsMethod;
        private List<object> _elements;
        private bool _isAddingElement;
        private bool _foldout;
        private object _newElement;
        private UIToolkitFieldDrawer _elementDrawer;

        private Label _countLabel;
        private VisualElement _elementsContainer;
        private VisualElement _addFormContainer;

        public override VisualElement CreateField(string label, Type fieldType, object value, Action<object> onValueChanged)
        {
            var root = new VisualElement();

            if (value == null)
            {
                root.Add(new Label($"{label}: HashSet is null"));
                return root;
            }

            _elementType = fieldType.GetGenericArguments()[0];
            _elementDrawer = FieldDrawerResolver<UIToolkitFieldDrawer>.CreateDrawer(_elementType);
            _setType = typeof(HashSet<>).MakeGenericType(_elementType);
            _addMethod = _setType.GetMethod("Add");
            _removeMethod = _setType.GetMethod("Remove", new[] { _elementType });
            _containsMethod = _setType.GetMethod("Contains", new[] { _elementType });

            var foldout = new Foldout { text = label, value = _foldout };
            foldout.RegisterValueChangedCallback(evt => _foldout = evt.newValue);
            root.Add(foldout);

            var headerRow = new VisualElement();
            headerRow.style.flexDirection = FlexDirection.Row;
            headerRow.style.justifyContent = Justify.SpaceBetween;
            headerRow.style.alignItems = Align.Center;

            _countLabel = new Label();
            _countLabel.style.unityTextAlign = TextAnchor.MiddleLeft;

            var addButton = new Button(() =>
            {
                _isAddingElement = !_isAddingElement;
                if (_isAddingElement)
                {
                    _newElement = FieldUtility.GetOrCreateTypeInstance(_elementType, _elementDrawer);
                }

                UpdateAddFormVisibility(value, onValueChanged);
            })
            {
                text = "+"
            };

            headerRow.Add(_countLabel);
            headerRow.Add(addButton);
            foldout.Add(headerRow);

            _addFormContainer = new VisualElement();
            _addFormContainer.style.marginTop = 4;
            foldout.Add(_addFormContainer);

            _elementsContainer = new VisualElement();
            _elementsContainer.style.marginTop = 4;
            foldout.Add(_elementsContainer);

            RefreshElements(value);
            UpdateCount(value);
            UpdateAddFormVisibility(value, onValueChanged);
            RebuildElementsList(value, onValueChanged);

            return root;
        }

        private void UpdateCount(object setInstance)
        {
            if (_countLabel == null)
            {
                return;
            }

            int count = _elements?.Count ?? 0;
            _countLabel.text = $"{count} items";
        }

        private void UpdateAddFormVisibility(object setInstance, Action<object> onValueChanged)
        {
            if (_addFormContainer == null)
            {
                return;
            }

            _addFormContainer.Clear();
            _addFormContainer.style.display = _isAddingElement ? DisplayStyle.Flex : DisplayStyle.None;

            if (!_isAddingElement)
            {
                return;
            }

            BuildAddForm(_addFormContainer, setInstance, onValueChanged);
        }

        private void BuildAddForm(VisualElement container, object setInstance, Action<object> onValueChanged)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;

            var label = new Label("Element:");
            label.style.minWidth = 70;

            var fieldContainer = new VisualElement();
            fieldContainer.style.flexGrow = 1;

            var elementField = CreateElementField(string.Empty, _newElement, newValue => _newElement = newValue);
            fieldContainer.Add(elementField);

            var confirmButton = new Button(() =>
            {
                bool exists = _containsMethod != null && (bool)_containsMethod.Invoke(setInstance, new[] { _newElement });
                if (!exists)
                {
                    try
                    {
                        _addMethod?.Invoke(setInstance, new[] { _newElement });
                        _isAddingElement = false;
                        onValueChanged?.Invoke(setInstance);
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

                RefreshElements(setInstance);
                UpdateCount(setInstance);
                UpdateAddFormVisibility(setInstance, onValueChanged);
                RebuildElementsList(setInstance, onValueChanged);
            })
            {
                text = "?"
            };

            var cancelButton = new Button(() =>
            {
                _isAddingElement = false;
                UpdateAddFormVisibility(setInstance, onValueChanged);
            })
            {
                text = "X"
            };

            row.Add(label);
            row.Add(fieldContainer);
            row.Add(confirmButton);
            row.Add(cancelButton);

            container.Add(row);
        }

        private void RebuildElementsList(object setInstance, Action<object> onValueChanged)
        {
            if (_elementsContainer == null)
            {
                return;
            }

            _elementsContainer.Clear();
            RefreshElements(setInstance);

            for (int i = 0; i < _elements.Count; i++)
            {
                object element = _elements[i];

                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.alignItems = Align.Center;
                row.style.marginBottom = 2;

                var fieldContainer = new VisualElement();
                fieldContainer.style.flexGrow = 1;

                var fieldElement = CreateElementField(string.Empty, element, newValue =>
                {
                    if (Equals(newValue, element))
                    {
                        return;
                    }

                    bool exists = _containsMethod != null && (bool)_containsMethod.Invoke(setInstance, new[] { newValue });
                    if (!exists)
                    {
                        _removeMethod?.Invoke(setInstance, new[] { element });
                        _addMethod?.Invoke(setInstance, new[] { newValue });
                        onValueChanged?.Invoke(setInstance);
                    }
                    else
                    {
                        Debug.LogWarning("Element already exists in HashSet.");
                    }

                    RefreshElements(setInstance);
                    UpdateCount(setInstance);
                    RebuildElementsList(setInstance, onValueChanged);
                });

                fieldContainer.Add(fieldElement);

                var removeButton = new Button(() =>
                {
                    _removeMethod?.Invoke(setInstance, new[] { element });
                    onValueChanged?.Invoke(setInstance);

                    RefreshElements(setInstance);
                    UpdateCount(setInstance);
                    RebuildElementsList(setInstance, onValueChanged);
                })
                {
                    text = "X"
                };

                row.Add(fieldContainer);
                row.Add(removeButton);

                _elementsContainer.Add(row);
            }
        }

        private VisualElement CreateElementField(string label, object value, Action<object> onValueChanged)
        {
            if (_elementDrawer != null)
            {
                return _elementDrawer.CreateField(label, _elementType, value, onValueChanged);
            }

            if (value == null)
            {
                return new Label("Element is null");
            }

            return _fieldsDrawer.CreateFieldsContainer(value);
        }

        private void RefreshElements(object setInstance)
        {
            if (setInstance == null)
            {
                return;
            }

            if (_elements == null)
            {
                _elements = new List<object>();
            }

            _elements.Clear();
            foreach (object element in (System.Collections.IEnumerable)setInstance)
            {
                _elements.Add(element);
            }
        }
    }
}
#endif



