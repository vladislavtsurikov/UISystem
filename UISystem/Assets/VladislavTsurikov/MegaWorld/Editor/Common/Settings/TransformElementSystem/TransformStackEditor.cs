#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Attributes;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.TransformElementSystem
{
    public class TransformStackEditor : ReorderableListStackEditor<TransformComponent, ReorderableListComponentEditor>
    {
        private readonly List<Type> _dontShowTransformTypes = new();
        private readonly bool _useSimpleComponent;

        public TransformStackEditor(GUIContent reorderableListName, TransformComponentStack list,
            List<Type> dontShowTransformTypes, bool useSimpleComponent) : base(reorderableListName, list, true)
        {
            DisplayHeaderText = false;
            _dontShowTransformTypes = dontShowTransformTypes;
            _useSimpleComponent = useSimpleComponent;
        }

        public TransformStackEditor(GUIContent reorderableListName, TransformComponentStack list,
            bool useSimpleComponent) : base(reorderableListName, list, true)
        {
            DisplayHeaderText = false;
            _useSimpleComponent = useSimpleComponent;
        }

        public TransformStackEditor(GUIContent reorderableListName, TransformComponentStack list) : base(
            reorderableListName, list, true)
        {
            DisplayHeaderText = false;
            _useSimpleComponent = false;
        }

        private ComponentStackOnlyDifferentTypes<TransformComponent> ComponentStackOnlyDifferentTypes =>
            (ComponentStackOnlyDifferentTypes<TransformComponent>)Stack;

        protected override void ShowAddMenu()
        {
            var menu = new GenericMenu();

            foreach (KeyValuePair<Type, Type> type in AllEditorTypes<TransformComponent>.Types)
            {
                if (_dontShowTransformTypes.Contains(type.Key))
                {
                    continue;
                }

                var context = type.Key.GetAttribute<NameAttribute>().Name;

                if (_useSimpleComponent)
                {
                    if (type.Key.GetAttribute<SimpleAttribute>() == null)
                    {
                        continue;
                    }
                }
                else
                {
                    if (type.Key.GetAttribute<SimpleAttribute>() != null)
                    {
                        context = "Simple/" + context;
                    }
                    else
                    {
                        context = "Advanced/" + context;
                    }
                }

                var exists = ComponentStackOnlyDifferentTypes.HasType(type.Key);

                if (!exists)
                {
                    menu.AddItem(new GUIContent(context), false,
                        () => ComponentStackOnlyDifferentTypes.CreateIfMissingType(type.Key));
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(context));
                }
            }

            menu.ShowAsContext();
        }
    }
}
#endif
