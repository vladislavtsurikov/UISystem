using UnityEngine;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    public abstract class TransformComponent : Node
    {
        public virtual void SetInstanceData(ref Instance instance, float fitness, Vector3 normal)
        {
        }
    }
}
