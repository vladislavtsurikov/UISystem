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
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;

namespace VladislavTsurikov.SceneManagerTool.Editor.SettingsSystem
{
    public class SettingsStackEditor : ReorderableListStackEditor<SettingsComponent, ReorderableListComponentEditor>
    {
        private readonly bool _sceneCollection;

        public SettingsStackEditor(GUIContent reorderableListName, bool sceneCollection,
            ComponentStackOnlyDifferentTypes<SettingsComponent> list) : base(reorderableListName, list, true)
        {
            _sceneCollection = sceneCollection;
            CopySettings = true;
            ShowActiveToggle = false;
        }

        private ComponentStackOnlyDifferentTypes<SettingsComponent> ComponentStackOnlyDifferentTypes =>
            (ComponentStackOnlyDifferentTypes<SettingsComponent>)Stack;

        protected override void ShowAddMenu()
        {
            var menu = new GenericMenu();

            foreach (KeyValuePair<Type, Type> type in AllEditorTypes<SettingsComponent>.Types)
            {
                Type settingsType = type.Key;

                var exists = ComponentStackOnlyDifferentTypes.GetElement(settingsType) != null;

                var context = settingsType.GetAttribute<NameAttribute>().Name;

                if (_sceneCollection)
                {
                    if (settingsType.GetAttribute<SceneComponentAttribute>() != null)
                    {
                        continue;
                    }
                }
                else
                {
                    if (settingsType.GetAttribute<SceneCollectionComponentAttribute>() != null)
                    {
                        continue;
                    }
                }

                if (!exists)
                {
                    menu.AddItem(new GUIContent(context), false,
                        () => ComponentStackOnlyDifferentTypes.CreateIfMissingType(settingsType));
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
