using VladislavTsurikov.UnityTimer.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper.AutoRespawn
{
    public class AutoRespawnController
    {
        private Timer _timer;

        public void StartAutoRespawn(float duration, Respawn respawn)
        {
            if (_timer == null || _timer.IsDone)
            {
                _timer = Timer.Register(duration, respawn.OnRespawn);
            }
        }
    }
}
