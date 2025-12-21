#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core.Behavior;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public class BehaviorAttributesElement : VisualElement
    {
        private readonly List<BehaviorAttributeData> _behaviors;
        private readonly VisualElement _listContainer;
        private readonly Button _addButton;

        public BehaviorAttributesElement(List<BehaviorAttributeData> behaviors)
        {
            _behaviors = behaviors;

            _listContainer = new VisualElement();
            Add(_listContainer);

            _addButton = new Button(AddBehavior) { text = "+ Add Behavior" };
            _addButton.style.marginTop = 4;
            Add(_addButton);

            RefreshList();
        }

        private void AddBehavior()
        {
            if (!HasBehaviorTypes())
            {
                return;
            }

            _behaviors.Add(new BehaviorAttributeData());
            RefreshList();
        }

        private void RefreshList()
        {
            _listContainer.Clear();

            if (!TryGetBehaviorTypes(out var behaviorTypes))
            {
                _addButton.style.display = DisplayStyle.None;

                var warning = new HelpBox(
                    "No LoaderBehavior implementations found. Create a class that inherits from LoaderBehavior to add behaviors.",
                    HelpBoxMessageType.Warning);
                _listContainer.Add(warning);
                return;
            }

            _addButton.style.display = DisplayStyle.Flex;

            for (int i = 0; i < _behaviors.Count; i++)
            {
                int idx = i;
                var data = _behaviors[idx];

                var box = new Box();
                box.style.marginBottom = 6;
                box.style.paddingLeft = 4;
                box.style.paddingTop = 4;
                box.style.paddingBottom = 4;

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

        private static bool TryGetBehaviorTypes(out List<System.Type> behaviorTypes)
        {
            behaviorTypes = AllTypesDerivedFrom<LoaderBehavior>.Types
                .Where(t => t.IsClass && !t.IsAbstract)
                .ToList();

            return behaviorTypes.Count > 0;
        }

        private static bool HasBehaviorTypes()
        {
            return AllTypesDerivedFrom<LoaderBehavior>.Types.Any(t => t.IsClass && !t.IsAbstract);
        }
    }
}
#endif
