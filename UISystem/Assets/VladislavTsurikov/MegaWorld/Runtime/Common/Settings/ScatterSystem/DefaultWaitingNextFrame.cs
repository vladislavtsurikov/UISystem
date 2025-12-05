using System.Diagnostics;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    public class DefaultWaitingNextFrame : WaitingNextFrame
    {
        private readonly float _secondsUntilNextFrame;
        private Stopwatch _stopwatch;

        public DefaultWaitingNextFrame(float secondsUntilNextFrame) => _secondsUntilNextFrame = secondsUntilNextFrame;

        private float MillisecondsUntilNextFrame => _secondsUntilNextFrame * 1000;

        public override bool IsWaitForNextFrame()
        {
            if (_stopwatch == null)
            {
                _stopwatch = new Stopwatch();
                _stopwatch.Start();
                return false;
            }

            if (_stopwatch.ElapsedMilliseconds >= MillisecondsUntilNextFrame)
            {
                _stopwatch.Restart();
                return true;
            }

            return false;
        }
    }
}
