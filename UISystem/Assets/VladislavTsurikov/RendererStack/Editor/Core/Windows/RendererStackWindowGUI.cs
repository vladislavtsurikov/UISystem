#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.SceneDataSystem.Editor.Utility;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;

namespace VladislavTsurikov.RendererStack.Editor.Core.Windows
{
    public partial class RendererStackWindow
    {
        protected override void OnGUI()
        {
            base.OnGUI();

            List<RendererStackManager> sceneDatas = SceneDataStackUtility.GetAllSceneData<RendererStackManager>();

            if (sceneDatas.Count > 1)
            {
                CustomEditorGUILayout.HelpBox(
                    "There are several types of Renderer Stack Manager at the level, but there should be only one");
                CustomEditorGUILayout.HelpBox(
                    "Open the Scene Data Manager Window via Windows -> VladislavTsurikov -> Scene Data Manager. You need to remove the Renderer Stack Manager from the inactive scene");
            }
            else if (sceneDatas.Count == 0)
            {
                Rect backgroundRect = EditorGUILayout.GetControlRect(true, 30);

                Rect settingsRect = CustomEditorGUILayout.ScreenRect;
                settingsRect.width = 350;
                settingsRect.height = 30;
                settingsRect.y = backgroundRect.y;
                settingsRect.y += 5;
                settingsRect.x = CustomEditorGUILayout.ScreenRect.width / 2 - settingsRect.width / 2;

                if (CustomEditorGUI.ClickButton(settingsRect, "Create Level Renders for the active scene",
                        ButtonStyle.Add))
                {
                    SceneDataSystemUtility.SetActiveSceneAsParentSceneType();
                    RequiredSceneData.Create<RendererStackManager>();
                }

                GUILayout.Space(5);
            }
            else
            {
                RendererStackManager rendererStackManager = sceneDatas[0];
                if (!rendererStackManager.SceneDataManager.IsSetup)
                {
                    rendererStackManager.SceneDataManager.Setup();
                }

                rendererStackManager.RendererStackStackEditor?.OnGUI();
            }
        }
    }
}
#endif
