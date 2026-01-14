using UnityEngine;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents
{
    [Name("Vegetation Rotation")]
    public class VegetationRotation : ModifyTransformComponent
    {
        [Range(0f, 20f)]
        public float RotationXZ = 3;
        [Range(0f, 100f)]
        public float StrengthXY = 10;
        [Range(0f, 20f)]
        public float StrengthY = 7;

        public override void ModifyTransform(ref Instance instance, ref ModifyInfo modifyInfo, float moveLenght,
            Vector3 strokeDirection, float fitness, Vector3 normal)
        {
            var localstrengthRotationY = StrengthY * fitness;
            var localstrengthRotationXY = StrengthXY * fitness;

            Vector3 randomVector = modifyInfo.RandomRotation * 0.5f;
            var angleXZ = RotationXZ * 3.6f;
            var t = localstrengthRotationXY / 100;

            var rotationY = localstrengthRotationY * 3.6f * randomVector.y + instance.Rotation.eulerAngles.y;
            var rotationX = angleXZ;
            var rotationZ = angleXZ;

            instance.Rotation = Quaternion.Euler(new Vector3(instance.Rotation.eulerAngles.x, rotationY,
                instance.Rotation.eulerAngles.z));

            var rotation = Quaternion.Euler(new Vector3(rotationX, rotationY, rotationZ));
            var finalRotation = Quaternion.Lerp(instance.Rotation, rotation, t);

            instance.Rotation = finalRotation;
        }
    }
}
