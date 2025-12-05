using UnityEngine;
using UnityEngine.Rendering;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.RendererStack.Runtime.Core.GlobalSettings;

namespace VladislavTsurikov.RendererStack.Runtime.Common.GlobalSettings
{
    [Name("Quality")]
    public class Quality : GlobalComponent
    {
        [SerializeField]
        private float _maxRenderDistance = 8000;

        public bool IsShadowCasting = true;

        public float LODBias = 1;

        public float MaxRenderDistance
        {
            get => _maxRenderDistance;
            set
            {
                if (value < 0)
                {
                    _maxRenderDistance = 0;
                }
                else
                {
                    _maxRenderDistance = value;
                }
            }
        }

        public ShadowCastingMode GetShadowCastingMode() =>
            IsShadowCasting ? ShadowCastingMode.On : ShadowCastingMode.Off;
    }
}
