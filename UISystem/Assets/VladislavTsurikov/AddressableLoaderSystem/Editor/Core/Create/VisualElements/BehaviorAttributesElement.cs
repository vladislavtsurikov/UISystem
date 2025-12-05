#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core.Behavior;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create
{
    public class BehaviorAttributesElement : VisualElement
    {
        private readonly List<BehaviorAttributeData> _behaviors;
        private readonly VisualElement _listContainer;

        public BehaviorAttributesElement(List<BehaviorAttributeData> behaviors)
        {
            _behaviors = behaviors;

            _listContainer = new VisualElement();
            Add(_listContainer);

            var addButton = new Button(AddBehavior) { text = "+ Add Behavior" };
            addButton.style.marginTop = 4;
            Add(addButton);

            RefreshList();
        }

        private void AddBehavior()
        {
            _behaviors.Add(new BehaviorAttributeData());
            RefreshList();
        }

        private void RefreshList()
        {
            _listContainer.Clear();

            for (int i = 0; i < _behaviors.Count; i++)
            {
                int idx = i;
                var data = _behaviors[idx];

                var box = new Box();
                box.style.marginBottom = 6;
                box.style.paddingLeft = 4;
                box.style.paddingTop = 4;
                box.style.paddingBottom = 4;

                var behaviorTypes = AllTypesDerivedFrom<LoaderBehavior>.Types
                    .Where(t => t.IsClass && !t.IsAbstract)
                    .ToList();
                var choices = behaviorTypes.Select(t => t.Name).ToList();

                int selectedIndex = 0;
                if (data.BehaviorType != null)
                {
                    int found = choices.FindIndex(n => n == data.BehaviorType.Name);
                    if (found >= 0) selectedIndex = found;
                }

                var headerRow = new VisualElement { style = { flexDirection = FlexDirection.Row } };

                var typeField = new PopupField<string>("Behavior Type", choices, selectedIndex);
                typeField.style.flexGrow = 1;
                data.BehaviorType = behaviorTypes[selectedIndex];

                typeField.RegisterValueChangedCallback(evt =>
                {
                    int newIndex = choices.FindIndex(n => n == evt.newValue);
                    if (newIndex < 0) newIndex = 0;
                    data.BehaviorType = behaviorTypes[newIndex];
                });

                var removeButton = new Button(() =>
                {
                    _behaviors.RemoveAt(idx);
                    RefreshList();
                })
                {
                    text = "X"
                };

                removeButton.style.width = 22;
                removeButton.style.height = 18;
                removeButton.style.unityTextAlign = TextAnchor.MiddleCenter;
                removeButton.style.marginLeft = 6;
                removeButton.style.alignSelf = Align.Center;
                removeButton.style.backgroundColor = new Color(0.4f, 0.1f, 0.1f);

                headerRow.Add(typeField);
                headerRow.Add(removeButton);

                box.Add(headerRow);

                var contextsField = new TextField("Contexts (comma-separated)")
                {
                    value = data.Contexts != null && data.Contexts.Count > 0
                        ? string.Join(", ", data.Contexts)
                        : string.Empty
                };

                contextsField.RegisterValueChangedCallback(evt =>
                {
                    data.Contexts = evt.newValue.Split(',')
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToList();
                });

                box.Add(contextsField);
                _listContainer.Add(box);
            }
        }
    }
}
#endif
