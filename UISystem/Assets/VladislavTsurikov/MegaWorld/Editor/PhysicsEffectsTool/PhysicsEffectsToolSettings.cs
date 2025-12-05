#if UNITY_EDITOR
using System;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.PhysicsEffectsSystem;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool
{
    [Serializable]
    [Name("Physics Effects Tool Settings")]
    public class PhysicsEffectsToolSettings : Component
    {
        public float Spacing = 5;
        public ComponentStackOnlyDifferentTypes<PhysicsEffect> List = new();

        protected override void SetupComponent(object[] setupData = null)
        {
            List.CreateAllElementTypes();
        }
    }
}
#endif
