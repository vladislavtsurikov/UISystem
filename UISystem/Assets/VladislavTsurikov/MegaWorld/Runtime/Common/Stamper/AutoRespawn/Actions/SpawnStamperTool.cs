namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper.AutoRespawn
{
    public class SpawnStamperTool : Respawn
    {
        public SpawnStamperTool(StamperTool stamperToolTool) : base(stamperToolTool)
        {
        }

        public override void OnRespawn() => StamperTool.SpawnStamper();
    }
}
