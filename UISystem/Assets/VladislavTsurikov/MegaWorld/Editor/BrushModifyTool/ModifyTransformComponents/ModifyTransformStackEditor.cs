#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents
{
    public class
        ModifyTransformStackEditor : ReorderableListStackEditor<ModifyTransformComponent,
        ReorderableListComponentEditor>
    {
        public ModifyTransformStackEditor(GUIContent label,
            ComponentStackOnlyDifferentTypes<ModifyTransformComponent> stack) : base(label, stack, true) =>
            DisplayHeaderText = false;

        private ComponentStackOnlyDifferentTypes<ModifyTransformComponent> ComponentStackOnlyDifferentTypes =>
            (ComponentStackOnlyDifferentTypes<ModifyTransformComponent>)Stack;

        protected override void ShowAddMenu()
        {
            var menu = new GenericMenu();

            foreach (KeyValuePair<Type, Type> type in AllEditorTypes<ModifyTransformComponent>.Types)
            {
                Type modifyTransformComponentType = type.Key;

                var context = modifyTransformComponentType.GetAttribute<NameAttribute>().Name;

                var exists = ComponentStackOnlyDifferentTypes.HasType(modifyTransformComponentType);

                if (!exists)
                {
                    menu.AddItem(new GUIContent(context), false,
                        () => ComponentStackOnlyDifferentTypes.CreateIfMissingType(modifyTransformComponentType));
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
