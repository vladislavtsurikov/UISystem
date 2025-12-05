using Cysharp.Threading.Tasks;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.DefaultComponentsSystem
{
    public class DefaultGroupComponent : Component
    {
        protected Group Group;

        protected override void SetupComponent(object[] setupData = null)
        {
            Group = (Group)setupData[0];
        }
    }
}
