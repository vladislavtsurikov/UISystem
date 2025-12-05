using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    public abstract class Scatter : Runtime_Core_Component
    {
        protected ScatterStack ScatterStack => (ScatterStack)Stack;

        public abstract UniTask Samples(CancellationToken token, BoxArea boxArea, List<Vector2> samples,
            Action<Vector2> onSpawn = null);
    }
}
