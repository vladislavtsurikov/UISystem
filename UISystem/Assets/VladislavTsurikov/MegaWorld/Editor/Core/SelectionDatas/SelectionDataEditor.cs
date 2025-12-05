#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas
{
    public class SelectionDataEditor
    {
        public void OnGUI(SelectionData selectionData, SelectionDataDrawer selectionDataDrawer, Type toolType)
        {
            selectionDataDrawer.OnGUI(selectionData, toolType);

            if (selectionData.SelectedData.HasOneSelectedGroup())
            {
                RenameGroupGUI(selectionData.SelectedData.SelectedGroup);
            }
        }

        private static void RenameGroupGUI(Runtime.Core.SelectionDatas.Group.Group group)
        {
            if (group.Renaming == false)
            {
                return;
            }

            GUIStyle barStyle = CustomEditorGUILayout.GetStyle(StyleName.ActiveBar);
            GUIStyle labelStyle = CustomEditorGUILayout.GetStyle(StyleName.LabelButton);
            GUIStyle labelTextStyle = CustomEditorGUILayout.GetStyle(StyleName.LabelText);

            Color initialGUIColor = GUI.color;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(5);

                if (EditorGUIUtility.isProSkin)
                {
                    CustomEditorGUILayout.Button(group.name, labelStyle, barStyle, EditorColors.Instance.orangeNormal,
                        EditorColors.Instance.orangeDark.WithAlpha(0.3f), 20);
                }
                else
                {
                    CustomEditorGUILayout.Button(group.name, labelStyle, barStyle, EditorColors.Instance.orangeDark,
                        EditorColors.Instance.orangeNormal.WithAlpha(0.3f), 20);
                }

                GUILayout.Space(5);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace() + 15);

                    GUILayout.Label(new GUIContent("Rename to"), labelTextStyle);

                    GUI.color = EditorColors.Instance.orangeNormal;

                    group.RenamingName =
                        EditorGUILayout.TextField(GUIContent.none, group.RenamingName); //rename to field

                    GUI.color = initialGUIColor;

                    if (CustomEditorGUILayout.DrawIcon(StyleName.IconButtonOk, EditorColors.Instance.Green) ||
                        (Event.current.keyCode == KeyCode.Return &&
                         Event.current.type == EventType.KeyUp)) //rename OK button
                    {
                        group.Renaming = false;

                        var assetPath = AssetDatabase.GetAssetPath(group);
                        AssetDatabase.RenameAsset(assetPath, group.RenamingName);
                        AssetDatabase.SaveAssets();

                        Event.current.Use();
                    }

                    if (CustomEditorGUILayout.DrawIcon(StyleName.IconButtonCancel,
                            EditorColors.Instance.Red)) //rename CANCEL button
                    {
                        group.RenamingName = group.name;
                        group.Renaming = false;

                        Event.current.Use();
                    }

                    GUILayout.Space(5);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(2);
            }
            GUILayout.EndVertical();
            GUI.color = initialGUIColor;

            GUILayout.Space(15);
        }
    }
}
#endif
