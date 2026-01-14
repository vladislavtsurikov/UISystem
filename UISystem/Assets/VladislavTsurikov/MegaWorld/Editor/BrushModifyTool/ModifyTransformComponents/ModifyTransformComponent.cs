using System;
using UnityEngine;
using VladislavTsurikov.UnityUtility.Runtime;
using Component = VladislavTsurikov.Nody.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents
{
    [Serializable]
    public abstract class ModifyTransformComponent : Node
    {
        public virtual void ModifyTransform(ref Instance spawnInfo, ref ModifyInfo modifyInfo, float moveLenght,
            Vector3 strokeDirection, float fitness, Vector3 normal)
        {
        }
    }
}
