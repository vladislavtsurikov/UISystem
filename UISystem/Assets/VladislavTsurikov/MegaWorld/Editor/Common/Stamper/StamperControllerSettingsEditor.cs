#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Stamper
{
    [ElementEditor(typeof(StamperControllerSettings))]
    public class StamperControllerSettingsEditor : IMGUIElementEditor
    {
        private readonly GUIContent _autoRespawn = new("Auto Respawn",
            "Allows you to do automatic deletion and then spawn when you changed the settings.");

        private readonly GUIContent _delayAutoSpawn = new("Delay Auto Spawn", "Respawn delay in seconds.");

        private readonly GUIContent _visualisation =
            new("Visualisation", "Allows you to see the Mask Filter Settings visualization.");

        private StamperControllerSettings _stamperControllerSettings => (StamperControllerSettings)Target;

        public override void OnGUI()
        {
            _stamperControllerSettings.Visualisation =
                CustomEditorGUILayout.Toggle(_visualisation, _stamperControllerSettings.Visualisation);

            _stamperControllerSettings.AutoRespawn =
                CustomEditorGUILayout.Toggle(_autoRespawn, _stamperControllerSettings.AutoRespawn);

            if (_stamperControllerSettings.AutoRespawn)
            {
                EditorGUI.indentLevel++;
                _stamperControllerSettings.DelayAutoRespawn = CustomEditorGUILayout.Slider(_delayAutoSpawn,
                    _stamperControllerSettings.DelayAutoRespawn, 0, 3);
                EditorGUI.indentLevel--;

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                    if (CustomEditorGUILayout.ClickButton("Respawn", ButtonStyle.Add, ButtonSize.ClickButton))
                    {
                        UnspawnUtility.UnspawnGroups(_stamperControllerSettings.StamperTool.Data.GroupList, false);
                        _stamperControllerSettings.StamperTool.SpawnStamper(true);
                    }

                    GUILayout.Space(5);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(3);
            }
            else
            {
                DrawSpawnControls();
            }

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                if (CustomEditorGUILayout.ClickButton("Unspawn Selected Prototypes", ButtonStyle.Remove,
                        ButtonSize.ClickButton))
                {
                    if (EditorUtility.DisplayDialog("WARNING!",
                            "Are you sure you want to remove all resource instances that have been selected from the scene?",
                            "OK", "Cancel"))
                    {
                        UnspawnUtility.UnspawnGroups(
                            _stamperControllerSettings.StamperTool.Data.SelectedData.SelectedGroupList, true);
                        GUILayout.BeginHorizontal();
                    }
                }

                GUILayout.Space(5);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(3);
        }

        private void DrawSpawnControls()
        {
            if (_stamperControllerSettings.StamperTool.IsSpawning)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                    if (CustomEditorGUILayout.ClickButton("Cancel", ButtonStyle.Remove))
                    {
                        _stamperControllerSettings.StamperTool.CancelSpawn();
                    }

                    GUILayout.Space(5);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(3);
            }
            else
            {
                if (_stamperControllerSettings.StamperTool.Data.SelectedData
                        .GetSelectedPrototypes(typeof(PrototypeTerrainTexture)).Count == 0)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                        if (CustomEditorGUILayout.ClickButton("Respawn", ButtonStyle.Add, ButtonSize.ClickButton))
                        {
                            UnspawnUtility.UnspawnGroups(_stamperControllerSettings.StamperTool.Data.GroupList, false);
                            _stamperControllerSettings.StamperTool.SpawnStamper(true);
                        }

                        GUILayout.Space(5);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(3);
                }

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                    if (CustomEditorGUILayout.ClickButton("Spawn", ButtonStyle.Add))
                    {
                        _stamperControllerSettings.StamperTool.SpawnStamper(true);
                    }

                    GUILayout.Space(5);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(3);
            }
        }
    }
}
#endif
