#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.ResourceController;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.MonoBehaviour;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.ReflectionUtility;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Editor.Core.MonoBehaviour
{
    public class MonoBehaviourToolEditor : EditorBase, IToolEditor
    {
        private bool _commonSettingsFoldout = true;
        private IMGUIComponentStackEditor<Runtime_Core_Component, IMGUIElementEditor> _componentStackEditor;

        private bool _groupSettingsFoldout = true;
        private bool _prototypeSettingsFoldout = true;
        private bool _toolSettingsFoldout = true;
        private Vector2 _windowScrollPos;

        protected SelectionDataDrawer SelectionDataDrawer;
        public MonoBehaviourTool Target => (MonoBehaviourTool)target;

        public void OnEnable()
        {
            //When the code was written, somehow Editor On Enable was called first rather than Target On Enable, so IsSetup is checked here
            if (!Target.IsSetup)
            {
                Target.Setup();
            }

            DrawersSelectionDatasAttribute drawersSelectionDatasAttribute =
                GetType().GetAttribute<DrawersSelectionDatasAttribute>();

            if (drawersSelectionDatasAttribute == null)
            {
                SelectionDataDrawer = new SelectionDataDrawer(typeof(IconGroupsDrawer), typeof(IconPrototypesDrawer),
                    Target.Data, target.GetType());
            }
            else
            {
                SelectionDataDrawer = new SelectionDataDrawer(drawersSelectionDatasAttribute.SelectionGroupWindowType,
                    drawersSelectionDatasAttribute.SelectionPrototypeWindowType,
                    Target.Data, target.GetType());
            }

            _componentStackEditor =
                new IMGUIComponentStackEditor<Runtime_Core_Component, IMGUIElementEditor>(Target.ComponentStack);

            OnInit();
        }

        public SelectionData SelectionData => Target.Data;
        public Type TargetType => Target.GetType();

        protected virtual void OnInit()
        {
        }

        public virtual void DrawFirstSettings()
        {
        }

        public virtual void DrawToolButtons()
        {
        }

        public virtual void OnChangeGUIGroup(Group group)
        {
        }

        public virtual void OnChangeGUIPrototype(Prototype prototype)
        {
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.indentLevel = 0;

            CustomEditorGUILayout.IsInspector = true;

            OnGUI();

            Target.Data.SaveAllData();
            GlobalSettings.Instance.Save();
        }

        public void OnGUI()
        {
            Target.Data.OnGUI(SelectionDataDrawer, target.GetType());

            if (Target.Data.SelectedData.SelectedGroupList.Count == 0)
            {
                return;
            }

            if (!ToolEditorUtility.DrawWarningAboutUnsupportedResourceType(SelectionData, TargetType))
            {
                return;
            }

#if !RENDERER_STACK
            if (Target.Data.SelectedData.HasOneSelectedGroup())
            {
                if (Target.Data.SelectedData.SelectedGroup.PrototypeType == typeof(PrototypeTerrainObject))
                {
                    CustomEditorGUILayout.HelpBox(
                        "Terrain Object Renderer is missing in the project. Terrain Object Renderer is only available by sponsor through Patreon.");

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                        CustomEditorGUILayout.DrawHelpBanner(
                            "https://docs.google.com/document/d/1jIPRTMlCR3jsuUrT9CedmDwRC8SsPAf0qc_flbhMOLM/edit#heading=h.1mzix67heftb",
                            "Learn more about Terrain Object Renderer");
                        GUILayout.Space(5);
                    }
                    GUILayout.EndHorizontal();

                    return;
                }
            }
#endif

            if (Target.Data.SelectedData.HasOneSelectedGroup())
            {
                if (Target.Data.SelectedData.SelectedGroup.PrototypeList.Count == 0)
                {
                    CustomEditorGUILayout.HelpBox("This group does not contain more than one prototype.");
                    ResourcesControllerEditor.DrawResourceController(SelectionData, false);
                    return;
                }

                if (ResourcesControllerEditor.HasSyncError(Target.Data.SelectedData.SelectedGroup))
                {
                    ResourcesControllerEditor.DrawResourceController(SelectionData, false);
                    return;
                }
            }

            _windowScrollPos = EditorGUILayout.BeginScrollView(_windowScrollPos);

            DrawToolButtons();
            DrawFirstSettings();
            DrawToolElements();
            ResourcesControllerEditor.DrawResourceController(SelectionData);

            AddGeneralGroupComponentsAttribute addGeneralGroupComponentsAttribute =
                target.GetType().GetAttribute<AddGeneralGroupComponentsAttribute>();
            AddGroupComponentsAttribute addGroupComponentsAttribute =
                target.GetType().GetAttribute<AddGroupComponentsAttribute>();

            AddGeneralPrototypeComponentsAttribute addGeneralPrototypeComponentsAttribute =
                target.GetType().GetAttribute<AddGeneralPrototypeComponentsAttribute>();
            AddPrototypeComponentsAttribute addPrototypeComponentsAttribute =
                target.GetType().GetAttribute<AddPrototypeComponentsAttribute>();

            if (addGeneralGroupComponentsAttribute != null || addGroupComponentsAttribute != null)
            {
                DrawGroupSettings();
            }

            if (addGeneralPrototypeComponentsAttribute != null || addPrototypeComponentsAttribute != null)
            {
                DrawPrototypeSettings();
            }

            DrawCommonSettings();

            EditorGUILayout.EndScrollView();
        }

        protected virtual void DrawCommonSettings()
        {
            AddGlobalCommonComponentsAttribute addGlobalCommonComponentsAttribute =
                Target.GetType().GetAttribute<AddGlobalCommonComponentsAttribute>();

            if (addGlobalCommonComponentsAttribute == null)
            {
                return;
            }

            _commonSettingsFoldout = CustomEditorGUILayout.Foldout(_commonSettingsFoldout, "Common Settings");

            if (_commonSettingsFoldout)
            {
                EditorGUI.indentLevel++;

                GlobalSettings.Instance.CommonComponentStackEditor.DrawElements(addGlobalCommonComponentsAttribute.Types
                    .ToList());

                EditorGUI.indentLevel--;
            }
        }

        protected virtual void DrawGroupSettings(Group group)
        {
            EditorGUI.BeginChangeCheck();

            group.ComponentStackManager.DrawToolElements(TargetType);

            if (EditorGUI.EndChangeCheck())
            {
                OnChangeGUIGroup(group);
            }
        }

        protected virtual void DrawPrototypeSettings(Prototype proto)
        {
            EditorGUI.BeginChangeCheck();

            proto.ComponentStackManager.DrawToolElements(TargetType);

            if (EditorGUI.EndChangeCheck())
            {
                OnChangeGUIPrototype(proto);
            }
        }

        private void DrawPrototypeSettings()
        {
            if (!SelectionData.SelectedData.HasOneSelectedGroup())
            {
                return;
            }

            if (SelectionData.SelectedData.HasOneSelectedPrototype())
            {
                Prototype proto = SelectionData.SelectedData.SelectedPrototype;

                List<Type> drawTypes = proto.ComponentStackManager.GetAllElementTypes(TargetType);

                if (drawTypes.Count != 0)
                {
                    _prototypeSettingsFoldout = CustomEditorGUILayout.HeaderWithMenu(
                        "Prototype Settings (" + proto.Name + ")", _prototypeSettingsFoldout,
                        () => proto.ComponentStackManager.ResetElementsMenu(TargetType));

                    if (_prototypeSettingsFoldout)
                    {
                        EditorGUI.indentLevel++;

                        DrawPrototypeSettings(proto);

                        EditorGUI.indentLevel--;
                    }
                }
            }
            else
            {
                CustomEditorGUILayout.HelpBox("Select one prototype to display prototype settings.");
            }
        }

        private void DrawGroupSettings()
        {
            if (SelectionData.SelectedData.HasOneSelectedGroup())
            {
                Group group = SelectionData.SelectedData.SelectedGroup;

                List<Type> drawTypes = group.ComponentStackManager.GetAllElementTypes(TargetType);

                if (drawTypes.Count != 0)
                {
                    _groupSettingsFoldout = CustomEditorGUILayout.HeaderWithMenu("Group Settings (" + group.name + ")",
                        _groupSettingsFoldout,
                        () => group.ComponentStackManager.ResetElementsMenu(TargetType));

                    if (_groupSettingsFoldout)
                    {
                        EditorGUI.indentLevel++;

                        DrawGroupSettings(group);

                        EditorGUI.indentLevel--;
                    }
                }
            }
            else
            {
                CustomEditorGUILayout.HelpBox("Select one group to display group settings");
            }
        }

        public void DrawToolElements()
        {
            _toolSettingsFoldout = CustomEditorGUILayout.Foldout(_toolSettingsFoldout, GetNameToolSettings());

            if (_toolSettingsFoldout)
            {
                EditorGUI.indentLevel++;

                _componentStackEditor.OnGUI();

                EditorGUI.indentLevel--;
            }
        }

        private string GetNameToolSettings() =>
            "Tool Settings (" + Target.GetType().GetAttribute<NameAttribute>().Name + ")";
    }
}
#endif
