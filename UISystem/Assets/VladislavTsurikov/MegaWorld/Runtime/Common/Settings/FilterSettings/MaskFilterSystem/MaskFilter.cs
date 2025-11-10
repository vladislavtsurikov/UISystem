using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    public abstract class MaskFilter : Component
    {
        public virtual void Eval(MaskFilterContext maskFilterContext, int index)
        {
        }
    }
}
