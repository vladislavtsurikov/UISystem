using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings;

namespace VladislavTsurikov.RendererStack.Runtime.Common.PrototypeSettings
{
    public enum GetAdditionalShadow
    {
        None,
        MinCullingDistance,
        IncreaseBoundingSphere,
        DirectionLightShadowVisible
    }

    [Name("Frustum Culling Settings")]
    public class FrustumCulling : PrototypeComponent
    {
        public GetAdditionalShadow GetAdditionalShadow = GetAdditionalShadow.DirectionLightShadowVisible;
        public float IncreaseBoundingSphere = 7f;
        public float IncreaseShadowsBoundingSphere;
        public float MinCullingDistanceForAdditionalShadow = 50;
    }
}
