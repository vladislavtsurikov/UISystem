#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Editor.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.TextureStamperTool;

namespace VladislavTsurikov.MegaWorld.Editor.TextureStamperTool
{
    [ElementEditor(typeof(TextureStamperArea))]
    public class TextureStamperAreaEditor : IMGUIElementEditor
    {
        private readonly GUIContent _cellSize = new("Cell Size", "Sets the cell size in meters.");
        private readonly GUIContent _showCells = new("Show Cells", "Shows all available cells.");
        private CustomMasksEditor _customMasksEditor;

        public TextureStamperArea Area => (TextureStamperArea)Target;

        public override void OnEnable() => _customMasksEditor = new CustomMasksEditor(Area.CustomMasks);

        public override void OnGUI()
        {
            Area.SelectSettingsFoldout = CustomEditorGUILayout.Foldout(Area.SelectSettingsFoldout, "Area Settings");

            if (Area.SelectSettingsFoldout)
            {
                EditorGUI.indentLevel++;

                DrawFitToTerrainSizeButton();

                Area.UseSpawnCells =
                    CustomEditorGUILayout.Toggle(new GUIContent("Use Spawn Cells"), Area.UseSpawnCells);

                if (Area.UseSpawnCells)
                {
                    CustomEditorGUILayout.HelpBox(
                        "It is recommended to enable \"Use Cells\" when your terrain is more than 4 km * 4 km. This parameter creates smaller cells, \"Stamper Tool\" will spawn each cell in turn. Why this parameter is needed, too long spawn delay can disable Unity.");

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                        if (CustomEditorGUILayout.ClickButton("Refresh Cells"))
                        {
                            Area.CreateCells();
                        }

                        GUILayout.Space(3);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(3);

                    Area.CellSize = CustomEditorGUILayout.FloatField(_cellSize, Area.CellSize);
                    CustomEditorGUILayout.Label("Cell Count: " + Area.CellList.Count);
                    Area.ShowCells = CustomEditorGUILayout.Toggle(_showCells, Area.ShowCells);
                }
                else
                {
                    Area.UseMask = CustomEditorGUILayout.Toggle(new GUIContent("Use Mask"), Area.UseMask);

                    if (Area.UseMask)
                    {
                        _customMasksEditor.OnGUI();
                    }
                }

                EditorGUI.indentLevel--;
            }
        }

        public void DrawFitToTerrainSizeButton()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                if (CustomEditorGUILayout.ClickButton("Fit To Terrain Size"))
                {
                    Area.FitToTerrainSize(Area.StamperTool);
                }

                GUILayout.Space(3);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(3);
        }
    }
}
#endif
