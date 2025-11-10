#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.SceneManagerTool.Runtime.BuildSceneCollectionSystem;
using VladislavTsurikov.UnityUtility.Editor;
using Object = System.Object;

namespace VladislavTsurikov.SceneManagerTool.Editor.BuildSceneCollectionSystem
{
    public class
        BuildSceneCollectionStackEditor : TabComponentStackEditor<BuildSceneCollection,
        DefaultBuildSceneCollectionEditor>
    {
        private readonly BuildSceneCollectionStack _buildSceneCollectionStack;

        public BuildSceneCollectionStackEditor(BuildSceneCollectionStack stack) : base(stack)
        {
            _buildSceneCollectionStack = (BuildSceneCollectionStack)Stack;

            _tabStackEditor.AddCallback = ShowAddManu;
            _tabStackEditor.IconColor = SetTabColor;
            _tabStackEditor.TabWidthFromName = true;
            _tabStackEditor.EnableRename = true;
        }

        public override void OnTabStackGUI()
        {
            _tabStackEditor.OnGUI();

            if (Stack.ElementList.Count == 0)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Click on the plus button to add Build Scene Collection");
                EditorGUILayout.EndVertical();
            }
            else if (Stack.SelectedElement == null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("No Build Scene Collection Selected");
                EditorGUILayout.EndVertical();
            }
        }

        protected override GenericMenu TabMenu(int currentTabIndex)
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Delete"), false, ContextMenuUtility.ContextMenuCallback,
                new Action(() => { Stack.Remove(currentTabIndex); }));

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Active"),
                _buildSceneCollectionStack.ActiveBuildSceneCollection == Stack.ElementList[currentTabIndex],
                ContextMenuUtility.ContextMenuCallback, new Action(() =>
                {
                    var buildSceneCollectionStack = (BuildSceneCollectionStack)Stack;
                    buildSceneCollectionStack.ActiveBuildSceneCollection = Stack.ElementList[currentTabIndex];
                }));

            return menu;
        }

        protected override void ShowAddManu()
        {
            var menu = new GenericMenu();

            Dictionary<Type, Type> types = AllEditorTypes<BuildSceneCollection>.Types;

            if (types.Count == 1)
            {
                EditorApplication.delayCall += () =>
                {
                    _buildSceneCollectionStack.CreateComponent(typeof(DefaultBuildSceneCollection));
                };
            }
            else
            {
                foreach (KeyValuePair<Type, Type> type in AllEditorTypes<BuildSceneCollection>.Types)
                {
                    Type settingsType = type.Key;

                    if (settingsType.ToString() == typeof(DefaultBuildSceneCollection).ToString())
                    {
                        continue;
                    }

                    var context = settingsType.GetAttribute<NameAttribute>().Name;

                    menu.AddItem(new GUIContent(context), false,
                        () => _buildSceneCollectionStack.CreateComponent(settingsType));
                }

                menu.AddSeparator("");

                menu.AddItem(new GUIContent("Create Default"), false,
                    () => _buildSceneCollectionStack.CreateComponent(typeof(DefaultBuildSceneCollection)));
            }

            menu.ShowAsContext();
        }

        private void SetTabColor(Object tab, out Color barColor, out Color labelColor)
        {
            var buildSceneCollection = (BuildSceneCollection)tab;
            barColor = EditorColors.Instance.LabelColor;

            if (_buildSceneCollectionStack.ActiveBuildSceneCollection != buildSceneCollection)
            {
                if (buildSceneCollection.Selected)
                {
                    labelColor = EditorColors.Instance.orangeDark;
                    barColor = EditorColors.Instance.orangeNormal;
                }
                else
                {
                    labelColor = EditorColors.Instance.orangeNormal;
                    barColor = EditorColors.Instance.orangeDark.WithAlpha(0.3f);
                }
            }
            else
            {
                labelColor = EditorColors.Instance.LabelColor;

                if (buildSceneCollection.Selected)
                {
                    barColor = EditorColors.Instance.ToggleButtonActiveColor;
                }
                else
                {
                    barColor = EditorColors.Instance.ToggleButtonInactiveColor;
                }
            }
        }
    }
}
#endif
