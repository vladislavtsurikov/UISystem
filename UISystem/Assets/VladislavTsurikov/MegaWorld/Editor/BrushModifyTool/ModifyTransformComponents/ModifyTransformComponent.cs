using System;
using UnityEngine;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.UnityUtility.Runtime;

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
