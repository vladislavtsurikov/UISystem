#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Editor.Core.MonoBehaviour;
using VladislavTsurikov.MegaWorld.Editor.TextureStamperTool;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper.AutoRespawn;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.TerrainSpawner
{
    [Name("Terrain Spawner")]
    [CustomEditor(typeof(Runtime.TerrainSpawner.TerrainSpawner))]
    public sealed class TerrainSpawnerEditor : MonoBehaviourToolEditor
    {
        private Runtime.TerrainSpawner.TerrainSpawner _stamperTool;

        private TextureStamperAreaEditor _textureStamperAreaEditor;

        protected override void OnInit()
        {
            _stamperTool = (Runtime.TerrainSpawner.TerrainSpawner)target;

            _stamperTool.Area.SetAreaBounds(_stamperTool);
        }

        public override void OnChangeGUIGroup(Group group)
        {
            if (MaskFilterComponentSettingsEditor.ChangedGUI)
            {
                _stamperTool.StamperVisualisation.StamperMaskFilterVisualisation.NeedUpdateMask = true;
                MaskFilterComponentSettingsEditor.ChangedGUI = false;
            }

            if (!_stamperTool.StamperControllerSettings.AutoRespawn)
            {
                return;
            }

            _stamperTool.AutoRespawnController.StartAutoRespawn(_stamperTool.StamperControllerSettings.DelayAutoRespawn,
                new RespawnGroup(_stamperTool));
        }

        public override void OnChangeGUIPrototype(Prototype proto)
        {
            if (MaskFilterComponentSettingsEditor.ChangedGUI)
            {
                _stamperTool.StamperVisualisation.StamperMaskFilterVisualisation.NeedUpdateMask = true;
                MaskFilterComponentSettingsEditor.ChangedGUI = false;
            }

            if (!_stamperTool.StamperControllerSettings.AutoRespawn)
            {
                return;
            }

            if (proto.GetType() == typeof(PrototypeTerrainDetail))
            {
                var prototypeTerrainDetail = (PrototypeTerrainDetail)proto;
                _stamperTool.AutoRespawnController.StartAutoRespawn(
                    _stamperTool.StamperControllerSettings.DelayAutoRespawn,
                    new RespawnUnityTerrainDetail(prototypeTerrainDetail, _stamperTool));
                return;
            }

            _stamperTool.AutoRespawnController.StartAutoRespawn(_stamperTool.StamperControllerSettings.DelayAutoRespawn,
                new RespawnGroup(_stamperTool));
        }

        [MenuItem("GameObject/MegaWorld/Add Terrain Spawner", false, 14)]
        public static void AddStamper(MenuCommand menuCommand)
        {
            var stamper = new GameObject("Terrain Spawner") { transform = { localScale = new Vector3(500, 500, 500) } };
            stamper.AddComponent<Runtime.TerrainSpawner.TerrainSpawner>();
            UnityEditor.Undo.RegisterCreatedObjectUndo(stamper, "Created " + stamper.name);
            Selection.activeObject = stamper;
        }
    }
}
#endif
