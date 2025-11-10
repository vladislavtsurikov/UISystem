#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.OperationSystem;

namespace VladislavTsurikov.SceneManagerTool.Editor.SettingsSystem.OperationSystem
{
    public class SceneOperationStackEditor : ReorderableListStackEditor<Operation, ReorderableListComponentEditor>
    {
        private readonly ComponentStackSupportSameType<Operation> _componentStackSupportSameType;
        private readonly SettingsTypes _settingsTypes;

        public SceneOperationStackEditor(SettingsTypes settingsTypes, ComponentStackSupportSameType<Operation> list) :
            base(new GUIContent("Actions"), list, true)
        {
            _componentStackSupportSameType = list;
            _settingsTypes = settingsTypes;
            CopySettings = true;
            ShowActiveToggle = false;
        }

        protected override void ShowAddMenu()
        {
            var menu = new GenericMenu();

            foreach (Type settingsType in GetComponentTypes())
            {
                switch (_settingsTypes)
                {
                    case SettingsTypes.AfterLoadScene:
                        if (settingsType.GetAttribute<AfterLoadSceneComponentAttribute>() == null)
                        {
                            continue;
                        }

                        break;
                    case SettingsTypes.BeforeLoadScene:
                        if (settingsType.GetAttribute<BeforeLoadSceneComponentAttribute>() == null)
                        {
                            continue;
                        }

                        break;
                    case SettingsTypes.AfterUnloadScene:
                        if (settingsType.GetAttribute<AfterUnloadSceneComponentAttribute>() == null)
                        {
                            continue;
                        }

                        break;
                    case SettingsTypes.BeforeUnloadScene:
                        if (settingsType.GetAttribute<BeforeUnloadSceneComponentAttribute>() == null)
                        {
                            continue;
                        }

                        break;
                }

                var context = settingsType.GetAttribute<NameAttribute>().Name;

                menu.AddItem(new GUIContent(context), false,
                    () => _componentStackSupportSameType.CreateComponent(settingsType));
            }

            menu.ShowAsContext();
        }
    }
}
#endif
