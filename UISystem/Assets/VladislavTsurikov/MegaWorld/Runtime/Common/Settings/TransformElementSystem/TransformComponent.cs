using UnityEngine;
using VladislavTsurikov.UnityUtility.Runtime;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    public abstract class TransformComponent : Runtime_Core_Component
    {
        public virtual void SetInstanceData(ref Instance instance, float fitness, Vector3 normal)
        {
        }
    }
}
