#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.ResourceController;
using VladislavTsurikov.MegaWorld.Editor.Core.Window.ElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;

namespace VladislavTsurikov.MegaWorld.Editor.Core.Window
{
    public class ToolWindowEditor : IMGUIElementEditor, IToolEditor
    {
        private bool _commonSettingsFoldout = true;

        private bool _groupSettingsFoldout = true;
        private bool _prototypeSettingsFoldout = true;
        private bool _toolSettingsFoldout = true;
        private Vector2 _windowScrollPos;

        public SelectionDataDrawer SelectionDataDrawer;
        public SelectionData SelectionData => WindowData.Instance.SelectionData;
        public Type TargetType => Target.GetType();

        protected override void InitElement()
        {
            DrawersSelectionDatasAttribute drawersSelectionDatasAttribute =
                GetType().GetAttribute<DrawersSelectionDatasAttribute>();

            if (drawersSelectionDatasAttribute == null)
            {
                SelectionDataDrawer = new SelectionDataDrawer(typeof(IconGroupsDrawer), typeof(IconPrototypesDrawer),
                    SelectionData, Target.GetType());
            }
            else
            {
                SelectionDataDrawer = new SelectionDataDrawer(drawersSelectionDatasAttribute.SelectionGroupWindowType,
                    drawersSelectionDatasAttribute.SelectionPrototypeWindowType,
                    SelectionData, Target.GetType());
            }

            OnEnable();
        }

        public override void OnGUI()
        {
            if (!SelectionWindow.IsOpen)
            {
                SelectionData.OnGUI(SelectionDataDrawer, Target.GetType());
            }

            if (SelectionData.SelectedData.SelectedGroupList.Count == 0)
            {
                return;
            }

            if (!ToolEditorUtility.DrawWarningAboutUnsupportedResourceType(SelectionData, TargetType))
            {
                return;
            }

#if !RENDERER_STACK
            if (SelectionData.SelectedData.HasOneSelectedGroup())
            {
                if (SelectionData.SelectedData.SelectedGroup.PrototypeType == typeof(PrototypeTerrainObject))
                {
                    CustomEditorGUILayout.HelpBox(
                        "Terrain Object Renderer is a free hyper optimization tool that is by far the best alternative to Unity Terrain Tree.");

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                        CustomEditorGUILayout.DrawHelpBanner(
                            "https://docs.google.com/document/d/1jIPRTMlCR3jsuUrT9CedmDwRC8SsPAf0qc_flbhMOLM/edit#heading=h.1mzix67heftb",
                            "Learn more about Terrain Object Renderer");
                        GUILayout.Space(5);
                    }
                    GUILayout.EndHorizontal();

                    CustomEditorGUILayout.HelpBox(
                        "This is only available to users of the Discord server, join, write to the developer Vladislav Tsurikov in PM to get all the tools, also tell something about yourself, you can also ask any questions.");

                    CustomEditorGUILayout.HelpBox(
                        "I hope you will become part of the community and be active on Discord, suggest ideas, write feedback, sponsor development by helping me literally create revolutionary tools, there are still a lot of ideas, this is just the beginning, I am creating a large ecosystem of tools.");

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                        CustomEditorGUILayout.DrawHelpBanner("https://discord.gg/fVAmyXs8GH",
                            "Join Discord server (get all tools)");
                        GUILayout.Space(5);
                    }
                    GUILayout.EndHorizontal();

                    return;
                }
            }
#endif
            if (SelectionData.SelectedData.HasOneSelectedGroup())
            {
                if (SelectionData.SelectedData.SelectedGroup.PrototypeList.Count == 0)
                {
                    CustomEditorGUILayout.HelpBox("This group does not contain more than one prototype.");
                    ResourcesControllerEditor.DrawResourceController(SelectionData, false);
                    return;
                }

                if (ResourcesControllerEditor.HasSyncError(SelectionData.SelectedData.SelectedGroup))
                {
                    ResourcesControllerEditor.DrawResourceController(SelectionData, false);
                    return;
                }
            }

            _windowScrollPos = EditorGUILayout.BeginScrollView(_windowScrollPos);

            DrawButtons();
            DrawFirstSettings();
            DrawToolSettings();
            ResourcesControllerEditor.DrawResourceController(SelectionData);

            AddGeneralGroupComponentsAttribute addGeneralGroupComponentsAttribute =
                Target.GetType().GetAttribute<AddGeneralGroupComponentsAttribute>();
            AddGroupComponentsAttribute addGroupComponentsAttribute =
                Target.GetType().GetAttribute<AddGroupComponentsAttribute>();

            AddGeneralPrototypeComponentsAttribute addGeneralPrototypeComponentsAttribute =
                Target.GetType().GetAttribute<AddGeneralPrototypeComponentsAttribute>();
            AddPrototypeComponentsAttribute addPrototypeComponentsAttribute =
                Target.GetType().GetAttribute<AddPrototypeComponentsAttribute>();

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

        public virtual void DrawButtons()
        {
        }

        public virtual void DrawFirstSettings()
        {
        }

        protected virtual void OnChangeGUIGroup(Group group)
        {
        }

        protected virtual void OnChangeGUIPrototype(Prototype prototype)
        {
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

        protected virtual void DrawToolSettings()
        {
            AddToolComponentsAttribute addToolComponentsAttribute =
                Target.GetType().GetAttribute<AddToolComponentsAttribute>();

            if (addToolComponentsAttribute == null)
            {
                return;
            }

            if (GetType().GetAttribute<DontDrawFoldoutAttribute>() == null)
            {
                _toolSettingsFoldout = CustomEditorGUILayout.HeaderWithMenu(GetNameToolSettings(Target),
                    _toolSettingsFoldout,
                    () => GlobalSettings.Instance.ToolsComponentStackEditor.ResetStackMenu(TargetType));

                if (_toolSettingsFoldout)
                {
                    EditorGUI.indentLevel++;

                    foreach (Type type in addToolComponentsAttribute.Types)
                    {
                        ToolsComponentStackEditor.OnGUI(Target.GetType(), type);
                    }

                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                foreach (Type type in addToolComponentsAttribute.Types)
                {
                    ToolsComponentStackEditor.OnGUI(Target.GetType(), type);
                }
            }
        }

        private static string GetNameToolSettings(Element target) => "Tool Settings (" + target.Name + ")";
    }
}
#endif
