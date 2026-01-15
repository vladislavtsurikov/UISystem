using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    public abstract class Scatter : Node
    {
        protected ScatterStack ScatterStack => (ScatterStack)Stack;

        public abstract UniTask Samples(CancellationToken token, BoxArea boxArea, List<Vector2> samples,
            Action<Vector2> onSpawn = null);
    }
}
