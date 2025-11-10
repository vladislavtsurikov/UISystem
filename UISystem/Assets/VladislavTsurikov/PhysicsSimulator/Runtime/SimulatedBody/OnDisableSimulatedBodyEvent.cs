namespace VladislavTsurikov.PhysicsSimulator.Runtime
{
    public abstract class OnDisableSimulatedBodyEvent
    {
        protected internal SimulatedBody SimulatedBody;

        protected internal abstract void OnDisablePhysics();
    }
}
