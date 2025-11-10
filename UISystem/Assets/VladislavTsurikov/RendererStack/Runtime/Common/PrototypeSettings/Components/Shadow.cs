using UnityEngine;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings;

namespace VladislavTsurikov.RendererStack.Runtime.Common.PrototypeSettings
{
    [Name("Shadow")]
    public class Shadow : PrototypeComponent
    {
        [SerializeField]
        private float _shadowDistance = 4000;

        public float[] ShadowLODMap = { 0, 4, 0, 0, 1, 5, 0, 0, 2, 6, 0, 0, 3, 7, 0, 0 };

        public float ShadowDistance
        {
            get => _shadowDistance;
            set
            {
                if (value < 0)
                {
                    _shadowDistance = 0;
                }
                else
                {
                    _shadowDistance = value;
                }
            }
        }
    }
}
