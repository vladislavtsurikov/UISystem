using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    public abstract class MaskFilter : Node
    {
        public virtual void Eval(MaskFilterContext maskFilterContext, int index)
        {
        }
    }
}
