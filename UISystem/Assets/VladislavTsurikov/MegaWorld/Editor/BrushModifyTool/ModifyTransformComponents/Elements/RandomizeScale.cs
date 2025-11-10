using UnityEngine;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents
{
    [Name("Randomize Scale")]
    public class RandomizeScale : ModifyTransformComponent
    {
        public Vector3 MaxScale = new(1.2f, 1.2f, 1.2f);
        public Vector3 MinScale = new(0.8f, 0.8f, 0.8f);
        public bool UniformScale = true;

        public override void ModifyTransform(ref Instance instance, ref ModifyInfo modifyInfo, float moveLenght,
            Vector3 strokeDirection, float fitness, Vector3 normal)
        {
            Vector3 scale;
            if (UniformScale)
            {
                var resScale = Random.Range(MinScale.x, MaxScale.x);
                scale = new Vector3(resScale, resScale, resScale);
            }
            else
            {
                scale = new Vector3(
                    Random.Range(MinScale.x, MaxScale.x),
                    Random.Range(MinScale.y, MaxScale.y),
                    Random.Range(MinScale.z, MaxScale.z));
            }

            instance.Scale = Vector3.Lerp(instance.Scale, scale, fitness);
        }
    }
}
