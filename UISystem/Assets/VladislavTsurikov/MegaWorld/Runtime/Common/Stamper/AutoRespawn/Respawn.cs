namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper.AutoRespawn
{
    public abstract class Respawn
    {
        protected readonly StamperTool StamperTool;

        protected Respawn(StamperTool stamperTool) => StamperTool = stamperTool;

        public abstract void OnRespawn();
    }
}
