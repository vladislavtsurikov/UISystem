#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings.OverlapChecks;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.OverlapCheckSettings
{
    [ElementEditor(typeof(Runtime.Common.Settings.OverlapCheckSettings.OverlapCheckSettings))]
    public class OverlapCheckSettingsEditor : IMGUIElementEditor
    {
        private Runtime.Common.Settings.OverlapCheckSettings.OverlapCheckSettings _settings;

        [NonSerialized]
        public GUIContent OverlapShape = new("Overlap Shape",
            "What shape will be checked for intersection with other prototypes. Overlap Shape only works with added prototypes in MegaWorld. Overlap Chap can be Bounds and Sphere.");

        public override void OnEnable() =>
            _settings = (Runtime.Common.Settings.OverlapCheckSettings.OverlapCheckSettings)Target;

        public override void OnGUI()
        {
            _settings.OverlapShapeEnum =
                (OverlapShapeEnum)CustomEditorGUILayout.EnumPopup(OverlapShape, _settings.OverlapShapeEnum);

            EditorGUI.indentLevel++;

            switch (_settings.OverlapShapeEnum)
            {
                case OverlapShapeEnum.OBB:
                {
                    OBBCheck obbCheck = _settings.ObbCheck;

                    obbCheck.BoundsType =
                        (BoundsCheckType)CustomEditorGUILayout.EnumPopup(BoundsType, obbCheck.BoundsType);

                    if (obbCheck.BoundsType == BoundsCheckType.Custom)
                    {
                        obbCheck.UniformBoundsSize =
                            CustomEditorGUILayout.Toggle(UniformBoundsSize, obbCheck.UniformBoundsSize);

                        if (obbCheck.UniformBoundsSize)
                        {
                            obbCheck.BoundsSize.x = CustomEditorGUILayout.FloatField(BoundsSize, obbCheck.BoundsSize.x);

                            obbCheck.BoundsSize.z = obbCheck.BoundsSize.x;
                            obbCheck.BoundsSize.y = obbCheck.BoundsSize.x;
                        }
                        else
                        {
                            obbCheck.BoundsSize = CustomEditorGUILayout.Vector3Field(BoundsSize, obbCheck.BoundsSize);
                        }

                        obbCheck.MultiplyBoundsSize =
                            CustomEditorGUILayout.Slider(MultiplyBoundsSize, obbCheck.MultiplyBoundsSize, 0, 5);
                    }
                    else if (obbCheck.BoundsType == BoundsCheckType.BoundsPrefab)
                    {
                        obbCheck.MultiplyBoundsSize =
                            CustomEditorGUILayout.Slider(MultiplyBoundsSize, obbCheck.MultiplyBoundsSize, 0, 5);
                    }

                    break;
                }
                case OverlapShapeEnum.Sphere:
                {
                    SphereCheck sphereCheck = _settings.SphereCheck;

                    sphereCheck.VegetationMode =
                        CustomEditorGUILayout.Toggle(VegetationMode, sphereCheck.VegetationMode);

                    if (sphereCheck.VegetationMode)
                    {
                        sphereCheck.Priority = CustomEditorGUILayout.IntField(Priority, sphereCheck.Priority);
                        sphereCheck.TrunkSize = CustomEditorGUILayout.Slider(TrunkSize, sphereCheck.TrunkSize, 0, 10);
                        sphereCheck.ViabilitySize =
                            CustomEditorGUILayout.FloatField(ViabilitySize, sphereCheck.ViabilitySize);

                        if (sphereCheck.ViabilitySize < _settings.SphereCheck.TrunkSize)
                        {
                            sphereCheck.ViabilitySize = _settings.SphereCheck.TrunkSize;
                        }
                    }
                    else
                    {
                        sphereCheck.Size = CustomEditorGUILayout.FloatField(Size, sphereCheck.Size);
                    }

                    break;
                }
            }

            EditorGUI.indentLevel--;

            CollisionCheck collisionCheck = _settings.CollisionCheck;

            collisionCheck.CollisionCheckType = CustomEditorGUILayout.Toggle(new GUIContent("Collision Check"),
                collisionCheck.CollisionCheckType);

            if (collisionCheck.CollisionCheckType)
            {
                EditorGUI.indentLevel++;

                collisionCheck.MultiplyBoundsSize =
                    CustomEditorGUILayout.Slider(MultiplyBoundsSize, collisionCheck.MultiplyBoundsSize, 0, 10);
                collisionCheck.CheckCollisionLayers =
                    CustomEditorGUILayout.LayerField(new GUIContent("Check Collision Layers"),
                        collisionCheck.CheckCollisionLayers);

                EditorGUI.indentLevel--;
            }
        }

        #region Bounds Check

        [NonSerialized]
        public GUIContent BoundsType = new("Bounds Type", "Which Bounds will be used.");

        [NonSerialized]
        public GUIContent UniformBoundsSize =
            new("Uniform Bounds Size", "Each side of the Bounds has the same size value.");

        [NonSerialized]
        public GUIContent BoundsSize = new("Bounds Size", "Lets you choose the size of the vector for bounds size.");

        [NonSerialized]
        public GUIContent MultiplyBoundsSize = new("Multiply Bounds Size", "Allows you to resize the bounds.");

        #endregion

        #region Sphere Variables

        [NonSerialized]
        public GUIContent VegetationMode = new("Vegetation Mode",
            "Allows you to use the priority system, which allows for example small trees to spawn under a large tree.");

        [NonSerialized]
        public GUIContent Priority = new("Priority",
            "Sets the ability of the object so that the object can spawn around the Viability Size of another object whose this value is less.");

        [NonSerialized]
        public GUIContent TrunkSize = new("Trunk Size",
            "Sets the size of the trunk. Other objects will never be spawn in this size.");

        [NonSerialized]
        public GUIContent ViabilitySize = new("Viability Size",
            " This is size in which other objects will not be spawned if Priority is less.");

        [NonSerialized]
        public GUIContent Size = new("Size", "The size of the sphere that will not spawn.");

        #endregion
    }
}
#endif
