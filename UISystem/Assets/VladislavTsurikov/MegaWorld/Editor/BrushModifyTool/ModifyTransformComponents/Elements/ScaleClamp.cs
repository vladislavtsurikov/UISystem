using UnityEngine;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents
{
    [Name("Scale Clamp")]
    public class ScaleClamp : ModifyTransformComponent
    {
        public float MaxScale = 2;
        public float MinScale = 0.5f;

        public override void ModifyTransform(ref Instance instance, ref ModifyInfo modifyInfo, float moveLenght,
            Vector3 strokeDirection, float fitness, Vector3 normal)
        {
            Vector3 scale = instance.Scale;

            if (instance.Scale.x > MaxScale)
            {
                scale.x = MaxScale;
            }
            else if (instance.Scale.x < MinScale)
            {
                scale.x = MinScale;
            }

            if (instance.Scale.y > MaxScale)
            {
                scale.y = MaxScale;
            }
            else if (instance.Scale.y < MinScale)
            {
                scale.y = MinScale;
            }

            if (instance.Scale.z > MaxScale)
            {
                scale.z = MaxScale;
            }
            else if (instance.Scale.z < MinScale)
            {
                scale.z = MinScale;
            }

            instance.Scale = Vector3.Lerp(instance.Scale, scale, fitness);
        }
    }
}
