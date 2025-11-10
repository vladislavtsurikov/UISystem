#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.ReflectionUtility.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings;
using VladislavTsurikov.UnityUtility.Editor;
using Object = System.Object;
using Renderer = VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem.Renderer;

namespace VladislavTsurikov.RendererStack.Editor.Core.RendererSystem
{
    public class RendererStackEditor : TabComponentStackEditor<Renderer, RendererEditor>
    {
        private readonly Runtime.Core.RendererSystem.RendererStack _rendererStack;

        public RendererStackEditor(Runtime.Core.RendererSystem.RendererStack stack) : base(stack)
        {
            _rendererStack = stack;

            _tabStackEditor.IconColor = SetTabColor;
            _tabStackEditor.TabHeight = 28;
        }

        public void OnGUI()
        {
            GUILayout.Space(5);

            EditorGUI.BeginChangeCheck();

            OnTabStackGUI();

            if (Stack.ElementList.Count == 0)
            {
                return;
            }

            if (!RendererStackManager.Instance.IsSetup)
            {
                CustomEditorGUILayout.Label("Initialization Failed:");

                foreach (Renderer renderer in Stack.ElementList)
                {
                    if (!renderer.IsSetup)
                    {
                        CustomEditorGUILayout.Label(renderer.Name + " (Renderer)");
                    }
                }

                foreach (SceneComponent rendererComponent in RendererStackManager.Instance.SceneComponentStack
                             .ElementList)
                {
                    if (!rendererComponent.IsSetup)
                    {
                        CustomEditorGUILayout.Label(rendererComponent.Name + " (Renderer Component)");
                    }
                }

                return;
            }

            GUILayout.Space(3);

            DrawSelectedSettings();

            if (EditorGUI.EndChangeCheck())
            {
                foreach (Renderer renderer in Stack.ElementList)
                {
                    renderer.ForceUpdateRendererData = true;
                }

                SceneView.RepaintAll();
            }
        }

        public override void OnTabStackGUI()
        {
            _tabStackEditor.OnGUI();

            if (Stack.ElementList.Count == 0)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Add rendering");
                EditorGUILayout.EndVertical();
            }
            else
            {
                if (SelectedEditor == null)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.LabelField("No Component Selected");
                    EditorGUILayout.EndVertical();
                }
            }
        }

        protected override GenericMenu TabMenu(int currentTabIndex)
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Delete"), false, ContextMenuUtility.ContextMenuCallback,
                new Action(() => { Stack.Remove(currentTabIndex); }));

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Refresh"), false, ContextMenuUtility.ContextMenuCallback,
                new Action(() => { RendererStackManager.Instance.Setup(true); }));

            menu.AddItem(new GUIContent("Active"), Stack.ElementList[currentTabIndex].Active,
                ContextMenuUtility.ContextMenuCallback,
                new Action(() =>
                {
                    Stack.ElementList[currentTabIndex].Active = !Stack.ElementList[currentTabIndex].Active;
                }));

            RendererEditor editor = Editors[currentTabIndex];

            if (editor.GetRendererMenu() != null)
            {
                menu.AddSeparator("");
                editor.GetRendererMenu().ShowGenericMenu(menu, editor.RendererTarget);
            }

            menu.AddItem(new GUIContent("Active"), Stack.ElementList[currentTabIndex].Active,
                ContextMenuUtility.ContextMenuCallback,
                new Action(() =>
                {
                    Stack.ElementList[currentTabIndex].Active = !Stack.ElementList[currentTabIndex].Active;
                }));

            if (Application.isPlaying == false)
            {
                menu.AddSeparator("");

                menu.AddItem(new GUIContent("Global Settings/Editor Play Mode Simulation"),
                    RendererStackManager.Instance.EditorPlayModeSimulation, ContextMenuUtility.ContextMenuCallback,
                    new Action(() =>
                    {
                        RendererStackManager.Instance.EditorPlayModeSimulation =
                            !RendererStackManager.Instance.EditorPlayModeSimulation;
                    }));
            }

            return menu;
        }

        private void SetTabColor(Object tab, out Color barColor, out Color labelColor)
        {
            var renderer = (Renderer)tab;
            barColor = EditorColors.Instance.LabelColor;

            if (!renderer.Active)
            {
                if (EditorGUIUtility.isProSkin)
                {
                    labelColor = EditorColors.Instance.orangeNormal;
                    barColor = EditorColors.Instance.orangeDark.WithAlpha(0.3f);
                }
                else
                {
                    labelColor = EditorColors.Instance.orangeDark;
                    barColor = EditorColors.Instance.orangeNormal.WithAlpha(0.3f);
                }
            }
            else
            {
                labelColor = EditorColors.Instance.LabelColor;

                if (renderer.Selected)
                {
                    barColor = EditorColors.Instance.ToggleButtonActiveColor;
                }
                else
                {
                    barColor = EditorColors.Instance.ToggleButtonInactiveColor;
                }
            }
        }

        protected override void ShowAddManu()
        {
            var contextMenu = new GenericMenu();

            foreach (Type type in AllTypesDerivedFrom<Renderer>.Types)
            {
                var name = type.GetAttribute<NameAttribute>().Name;

                if (_rendererStack.GetElement(type) == null)
                {
                    contextMenu.AddItem(new GUIContent(name), false, () => _rendererStack.CreateIfMissingType(type));
                }
                else
                {
                    contextMenu.AddDisabledItem(new GUIContent(name));
                }
            }

            contextMenu.ShowAsContext();
        }
    }
}
#endif
