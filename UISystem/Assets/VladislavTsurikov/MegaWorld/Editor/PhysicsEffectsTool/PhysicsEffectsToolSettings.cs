#if UNITY_EDITOR
using System;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.PhysicsEffectsSystem;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool
{
    [Serializable]
    [Name("Physics Effects Tool Settings")]
    public class PhysicsEffectsToolSettings : Node
    {
        public float Spacing = 5;
        public NodeStackOnlyDifferentTypes<PhysicsEffect> List = new();

        protected override void SetupComponent(object[] setupData = null)
        {
            List.CreateAllElementTypes();
        }
    }
}
#endif
