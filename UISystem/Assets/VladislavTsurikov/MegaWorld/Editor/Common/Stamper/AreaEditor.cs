#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Stamper
{
    [ElementEditor(typeof(Area))]
    public class AreaEditor : IMGUIElementEditor
    {
        private Area Area => (Area)Target;

        public override void OnGUI()
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
