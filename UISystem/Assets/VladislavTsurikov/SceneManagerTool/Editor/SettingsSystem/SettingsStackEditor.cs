#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;

namespace VladislavTsurikov.SceneManagerTool.Editor.SettingsSystem
{
    public class SettingsStackEditor : ReorderableListStackEditor<SettingsComponent, ReorderableListComponentEditor>
    {
        private readonly bool _sceneCollection;

        public SettingsStackEditor(GUIContent reorderableListName, bool sceneCollection,
            NodeStackOnlyDifferentTypes<SettingsComponent> list) : base(reorderableListName, list, true)
        {
            _sceneCollection = sceneCollection;
            CopySettings = true;
            ShowActiveToggle = false;
        }

        private NodeStackOnlyDifferentTypes<SettingsComponent> NodeStackOnlyDifferentTypes =>
            (NodeStackOnlyDifferentTypes<SettingsComponent>)Stack;

        protected override void ShowAddMenu()
        {
            var menu = new GenericMenu();

            foreach (KeyValuePair<Type, Type> type in AllEditorTypes<SettingsComponent>.Types)
            {
                Type settingsType = type.Key;

                var exists = NodeStackOnlyDifferentTypes.GetElement(settingsType) != null;

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
                        () => NodeStackOnlyDifferentTypes.CreateIfMissingType(settingsType));
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
