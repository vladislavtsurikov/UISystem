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
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.GlobalSettings.ExtensionSystem;

namespace VladislavTsurikov.RendererStack.Editor.TerrainObjectRenderer.GlobalSettings.ExtensionSystem
{
    public class ExtensionStackEditor : ReorderableListStackEditor<Extension, ReorderableListComponentEditor>
    {
        private readonly ComponentStackOnlyDifferentTypes<Extension> _componentStackOnlyDifferentTypes;

        public ExtensionStackEditor(ComponentStackOnlyDifferentTypes<Extension> list) : base(new GUIContent(), list,
            true) =>
            _componentStackOnlyDifferentTypes = list;

        protected override void ShowAddMenu()
        {
            var menu = new GenericMenu();

            foreach (KeyValuePair<Type, Type> item in AllEditorTypes<Extension>.Types)
            {
                Type extensionType = item.Key;

                var context = extensionType.GetAttribute<NameAttribute>().Name;

                ContextMenuAttribute contextMenuAttribute = item.Value.GetAttribute<ContextMenuAttribute>();

                if (contextMenuAttribute != null)
                {
                    context = contextMenuAttribute.ContextMenu;
                }

                if (_componentStackOnlyDifferentTypes.GetElement(extensionType) == null)
                {
                    menu.AddItem(new GUIContent(context), false,
                        () => _componentStackOnlyDifferentTypes.CreateIfMissingType(extensionType));
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
