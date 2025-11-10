#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem.SelectionData;
using VladislavTsurikov.RendererStack.Editor.Core.RendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.Preferences;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.Console;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.RenderModelData;
using LOD = VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.RenderModelData.LOD;

namespace VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem
{
    public class PrototypeRendererEditor : RendererEditor
    {
        private static PrototypeSettingsType _prototypeSettingsType = PrototypeSettingsType.Prototypes;

        private GroupsSelectedEditor _groupsSelectedEditor;
        private PrototypeSelectedEditor _prototypeSelectedEditor;

        private bool _selectRenderModelDataFoldout = true;
        private Vector2 _windowScrollPos;
        private PrototypeRenderer PrototypeRendererTarget => (PrototypeRenderer)Target;

        protected override void InitElement()
        {
            _groupsSelectedEditor = new GroupsSelectedEditor(PrototypeRendererTarget.SelectionData);
            _prototypeSelectedEditor = new PrototypeSelectedEditor(PrototypeRendererTarget.SelectionData);
        }

        public override void OnGUI()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(3);
                if (CustomEditorGUILayout.ToggleButton("Prototypes",
                        _prototypeSettingsType == PrototypeSettingsType.Prototypes, ButtonStyle.General))
                {
                    _prototypeSettingsType = PrototypeSettingsType.Prototypes;
                }

                GUILayout.Space(3);
                if (CustomEditorGUILayout.ToggleButton("Scene Settings",
                        _prototypeSettingsType == PrototypeSettingsType.ActiveScene, ButtonStyle.General))
                {
                    _prototypeSettingsType = PrototypeSettingsType.ActiveScene;
                }

                GUILayout.Space(3);
                if (CustomEditorGUILayout.ToggleButton("Global Settings",
                        _prototypeSettingsType == PrototypeSettingsType.GlobalSettings, ButtonStyle.General))
                {
                    _prototypeSettingsType = PrototypeSettingsType.GlobalSettings;
                }

                GUILayout.Space(5);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(3);

            switch (_prototypeSettingsType)
            {
                case PrototypeSettingsType.Prototypes:
                {
                    DrawPrototypeWindow();
                    break;
                }
                case PrototypeSettingsType.ActiveScene:
                {
                    RendererStackManager.Instance.SceneComponentStackEditor.OnGUI();
                    break;
                }
                case PrototypeSettingsType.GlobalSettings:
                {
                    Runtime.Core.GlobalSettings.GlobalSettings.Instance.RenderersGlobalComponentStackEditor.OnGUI();
                    break;
                }
            }
        }

        private void DrawPrototypeWindow()
        {
            PrototypeRendererTarget.SelectionData.SelectedData.RefreshSelectedParameters(PrototypeRendererTarget
                .SelectionData);

            DrawSelectedPrototypeWindow();

            _windowScrollPos = EditorGUILayout.BeginScrollView(_windowScrollPos);

            if (PrototypeRendererTarget.SelectionData.SelectedData.HasOneSelectedProto())
            {
                Prototype selectedPrototype = PrototypeRendererTarget.SelectionData.SelectedData.GetLastPrototype();

                DrawWarningTextPrototype(selectedPrototype);

                if (!selectedPrototype.PrototypeConsole.HasError())
                {
                    DrawPrototypeSettings(selectedPrototype, selectedPrototype.RenderModel);
                }
            }
            else
            {
                if (PrototypeRendererTarget.SelectionData.PrototypeList.Count != 0)
                {
                    CustomEditorGUILayout.HelpBox("Select one prototype to display prototype settings");
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawPrototypeSettings(Prototype selectedPrototype, RenderModel renderModel)
        {
            if (selectedPrototype == null)
            {
                return;
            }

            EditorGUI.BeginChangeCheck();

            if (RendererStackSettings.Instance.ShowRenderModelData)
            {
                DrawRenderModelSettings(renderModel);
            }

            selectedPrototype.PrototypeComponentStackEditor.OnGUI();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(PrototypesStorage.Instance);
            }
        }

        private void DrawSelectedPrototypeWindow()
        {
            _groupsSelectedEditor.OnGUI();
            _prototypeSelectedEditor.OnGUI();
        }

        private void DrawWarningTextPrototype(Prototype selectedPrototype)
        {
            foreach (PrototypeLog log in selectedPrototype.PrototypeConsole.PrototypeLogList)
            {
                if (log.Error)
                {
                    EditorGUILayout.HelpBox(log.Header + ". " + log.Text, MessageType.Error);
                }
                else
                {
                    EditorGUILayout.HelpBox(log.Header + ". " + log.Text, MessageType.Warning);
                }
            }
        }

        //This is for debugging
        private void DrawRenderModelSettings(RenderModel renderModel)
        {
            _selectRenderModelDataFoldout =
                CustomEditorGUILayout.Foldout(_selectRenderModelDataFoldout, "Render Model Data");

            if (_selectRenderModelDataFoldout)
            {
                EditorGUI.indentLevel++;

                for (var lodIndex = 0; lodIndex < renderModel.LODs.Count; lodIndex++)
                {
                    LOD lod = renderModel.LODs[lodIndex];

                    CustomEditorGUILayout.Label("LOD Index: " + lodIndex);

                    lod.Mesh = (Mesh)CustomEditorGUILayout.ObjectField(new GUIContent("Mesh"), lod.Mesh, typeof(Mesh));

                    for (var matIndex = 0; matIndex < lod.Materials.Count; matIndex++)
                    {
                        lod.Materials[matIndex] =
                            (Material)CustomEditorGUILayout.ObjectField(new GUIContent("Material"),
                                lod.Materials[matIndex], typeof(Material));
                    }
                }

                EditorGUI.indentLevel--;
            }
        }
    }
}
#endif
