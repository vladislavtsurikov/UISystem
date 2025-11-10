using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.SceneSettings
{
    [Name("Additional Data")]
    public class Quality : SceneComponent
    {
        public Light DirectionalLight;
        public Vector3 FloatingOriginOffset;
        public Vector3 FloatingOriginStartPosition;

        public Transform TransformOfFloatingOrigin;

        protected override void SetupComponent(object[] setupData)
        {
            SetupFloatingOrigin();

            if (DirectionalLight == null)
            {
                FindDirectionalLight();
            }
        }

        public void FindDirectionalLight()
        {
            Light selectedLight = null;
            var intensity = float.MinValue;

#if UNITY_6000_0_OR_NEWER
            Light[] lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
#else
            Light[] lights = Object.FindObjectsOfType<Light>();
#endif
            for (var i = 0; i <= lights.Length - 1; i++)
            {
                if (lights[i].type == LightType.Directional)
                {
                    if (lights[i].intensity > intensity)
                    {
                        intensity = lights[i].intensity;
                        selectedLight = lights[i];
                    }
                }
            }

            DirectionalLight = selectedLight;
        }

        public void SetupFloatingOrigin()
        {
            Transform anchor = GetFloatingOriginAnchor();
            FloatingOriginStartPosition = anchor.position;
        }

        public void UpdateFloatingOrigin()
        {
            if (Application.isPlaying)
            {
                Transform anchor = GetFloatingOriginAnchor();
                FloatingOriginOffset = anchor.transform.position - FloatingOriginStartPosition;
            }
            else
            {
                FloatingOriginOffset = Vector3.zero;
            }
        }

        public Transform GetFloatingOriginAnchor()
        {
            if (TransformOfFloatingOrigin)
            {
                return TransformOfFloatingOrigin;
            }

            return RendererStackManager.Instance.SceneDataManager.transform;
        }
    }
}
