#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Editor.Core.MonoBehaviour;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper.AutoRespawn;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.TextureStamperTool;

namespace VladislavTsurikov.MegaWorld.Editor.TextureStamperTool
{
    [CustomEditor(typeof(TextureStamper))]
    public class TextureStamperEditor : MonoBehaviourToolEditor
    {
        private TextureStamper _textureStamper;

        protected override void OnInit()
        {
            _textureStamper = (TextureStamper)target;

            _textureStamper.Area.SetAreaBounds(_textureStamper);

            SelectionDataDrawer = new SelectionDataDrawer(typeof(IconGroupsDrawer), typeof(IconPrototypesDrawer),
                _textureStamper.Data, target.GetType());
        }

        public override void OnChangeGUIGroup(Group group)
        {
            if (MaskFilterComponentSettingsEditor.ChangedGUI)
            {
                _textureStamper.StamperVisualisation.StamperMaskFilterVisualisation.NeedUpdateMask = true;
                MaskFilterComponentSettingsEditor.ChangedGUI = false;
            }

            if (!_textureStamper.StamperControllerSettings.AutoRespawn)
            {
                return;
            }

            if (_textureStamper.Area.UseSpawnCells)
            {
                return;
            }

            _textureStamper.AutoRespawnController.StartAutoRespawn(
                _textureStamper.StamperControllerSettings.DelayAutoRespawn, new SpawnStamperTool(_textureStamper));
        }

        public override void OnChangeGUIPrototype(Prototype proto)
        {
            if (MaskFilterComponentSettingsEditor.ChangedGUI)
            {
                _textureStamper.StamperVisualisation.StamperMaskFilterVisualisation.NeedUpdateMask = true;
                MaskFilterComponentSettingsEditor.ChangedGUI = false;
            }

            if (!_textureStamper.StamperControllerSettings.AutoRespawn)
            {
                return;
            }

            if (_textureStamper.Area.UseSpawnCells)
            {
                return;
            }

            _textureStamper.AutoRespawnController.StartAutoRespawn(
                _textureStamper.StamperControllerSettings.DelayAutoRespawn, new SpawnStamperTool(_textureStamper));
        }

        [MenuItem("GameObject/MegaWorld/Add Texture Stamper", false, 14)]
        public static void AddStamper(MenuCommand menuCommand)
        {
            var stamper = new GameObject("Texture Stamper") { transform = { localScale = new Vector3(150, 150, 150) } };
            stamper.AddComponent<TextureStamper>();
            UnityEditor.Undo.RegisterCreatedObjectUndo(stamper, "Created " + stamper.name);
            Selection.activeObject = stamper;
        }
    }
}
#endif
