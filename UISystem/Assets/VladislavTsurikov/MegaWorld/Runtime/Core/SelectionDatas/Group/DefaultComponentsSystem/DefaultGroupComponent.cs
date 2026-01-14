using Cysharp.Threading.Tasks;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.DefaultComponentsSystem
{
    public class DefaultGroupComponent : Node
    {
        protected Group Group;

        protected override void SetupComponent(object[] setupData = null)
        {
            Group = (Group)setupData[0];
        }
    }
}
