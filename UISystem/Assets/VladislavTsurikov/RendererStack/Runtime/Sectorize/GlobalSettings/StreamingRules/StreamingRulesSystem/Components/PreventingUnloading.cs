using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem
{
    [Name("Preventing Unloading")]
    public class PreventingUnloading : StreamingRule
    {
        private AsynchronousLoading _asynchronousLoading;
        private ImmediatelyLoading _immediatelyLoading;

        [OdinSerialize]
        private float _maxDistance = 4400;

        private ImmediatelyLoading ImmediatelyLoading =>
            _immediatelyLoading ??= StreamingRuleComponentStack.GetElement<ImmediatelyLoading>();

        public AsynchronousLoading AsynchronousLoading =>
            _asynchronousLoading ??=
                StreamingRuleComponentStack.GetAndAutoUpdateComponent<AsynchronousLoading>(component =>
                    _asynchronousLoading = component);

        public float MaxDistance
        {
            get
            {
                var maxLoadingDistance = GetMaxLoadingDistance();
                if (_maxDistance < maxLoadingDistance)
                {
                    _maxDistance = maxLoadingDistance;
                }

                return Mathf.Max(maxLoadingDistance, _maxDistance);
            }
            set => _maxDistance = Mathf.Max(GetMaxLoadingDistance(), value);
        }

        private float GetMaxLoadingDistance() =>
            Mathf.Max(ImmediatelyLoading.MaxDistance,
                AsynchronousLoading.IsValid() ? AsynchronousLoading.MaxDistance : 0);
    }
}
