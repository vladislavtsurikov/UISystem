namespace VladislavTsurikov.PhysicsSimulator.Runtime
{
    public abstract class DisablePhysicsMode
    {
        internal abstract void DisablePhysicsSupport();

        internal virtual void OnRegisterSimulatedBody(SimulatedBody simulatedBody)
        {
        }

        internal virtual void OnUnregisterSimulatedBody(SimulatedBody simulatedBody)
        {
        }

        internal virtual void OnDisable()
        {
        }

        internal virtual void Reset()
        {
        }
    }
}
