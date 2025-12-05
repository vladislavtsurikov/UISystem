using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.ColliderSystem.Runtime
{
    public interface IRaycast
    {
        public List<RayHit> RaycastAll(Ray ray, ObjectFilter objectFilter);
    }
}
