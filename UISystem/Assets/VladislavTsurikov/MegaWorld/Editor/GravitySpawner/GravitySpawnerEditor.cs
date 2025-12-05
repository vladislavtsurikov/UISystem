#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Editor.Common.PhysXPainter.Settings;
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Editor.Core.MonoBehaviour;
using VladislavTsurikov.MegaWorld.Editor.TextureStamperTool;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper.AutoRespawn;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.PhysicsSimulator.Runtime;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.GravitySpawner
{
    [Name("Gravity Spawner")]
    [CustomEditor(typeof(Runtime.GravitySpawner.GravitySpawner))]
    public sealed class GravitySpawnerEditor : MonoBehaviourToolEditor
    {
        private Runtime.GravitySpawner.GravitySpawner _stamperTool;

        private TextureStamperAreaEditor _textureStamperAreaEditor;

        protected override void OnInit()
        {
            _stamperTool = (Runtime.GravitySpawner.GravitySpawner)target;

            _stamperTool.Area.SetAreaBounds(_stamperTool);
        }

        public override void DrawFirstSettings() =>
            PhysicsSimulatorSettingsEditor.OnGUI<ObjectTimeDisablePhysicsMode>(PhysicsSimulatorSettings.Instance);

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

            _stamperTool.AutoRespawnController.StartAutoRespawn(_stamperTool.StamperControllerSettings.DelayAutoRespawn,
                new RespawnGroup(_stamperTool));
        }

        [MenuItem("GameObject/MegaWorld/Add Gravity Spawner", false, 14)]
        public static void AddStamper(MenuCommand menuCommand)
        {
            var stamper = new GameObject("Gravity Spawner") { transform = { localScale = new Vector3(500, 500, 500) } };
            stamper.AddComponent<Runtime.GravitySpawner.GravitySpawner>();
            UnityEditor.Undo.RegisterCreatedObjectUndo(stamper, "Created " + stamper.name);
            Selection.activeObject = stamper;
        }
    }
}
#endif
