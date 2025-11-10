using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem
{
    [Name("Asynchronous Loading")]
    public class AsynchronousLoading : StreamingRule
    {
        private ImmediatelyLoading _immediatelyLoading;

        [OdinSerialize]
        private float _maxDistance = 4000;


        public float MaxLoadingScenePause = 1;

        private ImmediatelyLoading ImmediatelyLoading =>
            _immediatelyLoading ??= StreamingRuleComponentStack.GetElement<ImmediatelyLoading>();

        public float MaxDistance
        {
            get
            {
                if (_maxDistance < ImmediatelyLoading.MaxDistance)
                {
                    _maxDistance = ImmediatelyLoading.MaxDistance;
                }

                return Mathf.Max(ImmediatelyLoading.MaxDistance, _maxDistance);
            }
            set => _maxDistance = Mathf.Max(ImmediatelyLoading.MaxDistance, value);
        }
    }
}
