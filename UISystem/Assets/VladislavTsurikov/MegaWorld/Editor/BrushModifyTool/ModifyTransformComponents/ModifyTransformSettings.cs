using UnityEngine;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents
{
    [Name("Modify Transform Components")]
    public class ModifyTransformSettings : Node
    {
        public NodeStackOnlyDifferentTypes<ModifyTransformComponent> ModifyTransformComponentStack = new();

        protected override void OnCreate() => ModifyTransformComponentStack.CreateIfMissingType(typeof(RandomizeScale));

        public void ModifyTransform(ref Instance instance, ref ModifyInfo modifyInfo, float moveLenght,
            Vector3 strokeDirection, float fitness, Vector3 normal)
        {
            foreach (ModifyTransformComponent item in ModifyTransformComponentStack.ElementList)
            {
                if (item.Active)
                {
                    item.ModifyTransform(ref instance, ref modifyInfo, moveLenght, strokeDirection, fitness, normal);
                }
            }
        }
    }
}
