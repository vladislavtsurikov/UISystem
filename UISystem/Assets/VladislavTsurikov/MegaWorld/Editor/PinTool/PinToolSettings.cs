using System;
using UnityEngine;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.ReflectionUtility;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Editor.PinTool
{
    public enum TransformMode
    {
        Free,
        Snap,
        Fixed
    }

    [Serializable]
    [Name("Pin Tool Settings")]
    public class PinToolSettings : Component
    {
        #region Position

        public float Offset;

        #endregion

        #region Rotation

        public TransformMode RotationTransformMode = TransformMode.Free;
        public FromDirection FromDirection;
        public float WeightToNormal = 1;
        public float SnapRotationValue = 30;
        public Vector3 FixedRotationValue;

        #endregion

        #region Scale

        public TransformMode ScaleTransformMode = TransformMode.Free;
        public Vector3 FixedScaleValue;
        public float SnapScaleValue = 0.3f;

        #endregion
    }
}
