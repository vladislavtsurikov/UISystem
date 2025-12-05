using OdinSerializer;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings;

namespace VladislavTsurikov.RendererStack.Runtime.Common.PrototypeSettings
{
    [Name("Distance Culling")]
    public class DistanceCulling : PrototypeComponent
    {
        [OdinSerialize]
        private float _distanceRandomOffset;

        [OdinSerialize]
        private float _maxDistance = 8000;

        public float DistanceRandomOffset
        {
            get => _distanceRandomOffset;
            set
            {
                if (value < 0)
                {
                    _distanceRandomOffset = 0;
                }
                else
                {
                    _distanceRandomOffset = value;
                }
            }
        }

        public float MaxDistance
        {
            get => _maxDistance;
            set
            {
                if (value < 0)
                {
                    _maxDistance = 0;
                }
                else
                {
                    _maxDistance = value;
                }
            }
        }
    }
}
