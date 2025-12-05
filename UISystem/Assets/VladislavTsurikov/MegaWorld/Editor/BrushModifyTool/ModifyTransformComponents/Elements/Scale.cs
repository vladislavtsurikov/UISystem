using UnityEngine;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents
{
    [Name("Scale")]
    public class Scale : ModifyTransformComponent
    {
        public float Strength = 0.3f;
        public float StrengthRandomize = 100;

        public override void ModifyTransform(ref Instance spawnInfo, ref ModifyInfo modifyInfo, float moveLenght,
            Vector3 strokeDirection, float fitness, Vector3 normal)
        {
            var randomScale = modifyInfo.RandomScale * (StrengthRandomize / 100f);

            var localStrengthScale = fitness * moveLenght;

            var addScale = Strength * localStrengthScale * 0.005f;
            addScale = Mathf.Lerp(addScale, 0, randomScale);

            spawnInfo.Scale = new Vector3(spawnInfo.Scale.x + addScale, spawnInfo.Scale.y + addScale,
                spawnInfo.Scale.z + addScale);
        }
    }
}
