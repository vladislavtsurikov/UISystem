using System;
using System.Diagnostics;

namespace Zenject
{
    [DebuggerStepThrough]
    public class Kernel : IInitializable, IDisposable, ITickable, ILateTickable, IFixedTickable, ILateDisposable
    {
        [InjectLocal]
        private DisposableManager _disposablesManager;

        [InjectLocal]
        private InitializableManager _initializableManager;

        [InjectLocal]
        private TickableManager _tickableManager;

        public virtual void Dispose() => _disposablesManager.Dispose();

        public virtual void FixedTick() => _tickableManager.FixedUpdate();

        public virtual void Initialize() => _initializableManager.Initialize();

        public virtual void LateDispose() => _disposablesManager.LateDispose();

        public virtual void LateTick() => _tickableManager.LateUpdate();

        public virtual void Tick() => _tickableManager.Update();
    }
}
