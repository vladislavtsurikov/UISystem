using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper.AutoRespawn
{
    public class RespawnGroup : Respawn
    {
        public RespawnGroup(StamperTool stamperToolTool) : base(stamperToolTool)
        {
        }

        public override void OnRespawn()
        {
            UnspawnUtility.UnspawnGroups(StamperTool.Data.SelectedData.SelectedGroupList, false);

            StamperTool.SpawnStamper();
        }
    }
}
