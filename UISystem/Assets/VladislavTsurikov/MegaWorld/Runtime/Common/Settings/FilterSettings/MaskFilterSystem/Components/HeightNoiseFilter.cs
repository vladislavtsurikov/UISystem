using System;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise.API;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    [Serializable]
    [Name("Height Noise")]
    public class HeightNoiseFilter : MaskFilter
    {
#if UNITY_EDITOR
        public NoiseSettings NoiseSettings;

        public BlendMode BlendMode = BlendMode.Multiply;

        public float MinHeight;
        public float MaxHeight;

        [SerializeField]
        private float _maxRangeNoise = 5f;

        public float MaxRangeNoise
        {
            get => _maxRangeNoise;
            set
            {
                if (value < 0.1)
                {
                    _maxRangeNoise = 0.1f;
                }
                else
                {
                    _maxRangeNoise = value;
                }
            }
        }

        [SerializeField]
        private float _minRangeNoise = 5f;

        public float MinRangeNoise
        {
            get => _minRangeNoise;
            set
            {
                if (value < 0.1)
                {
                    _minRangeNoise = 0.1f;
                }
                else
                {
                    _minRangeNoise = value;
                }
            }
        }

        private Material _heightNoiseMat;

        public Material GetMaterial()
        {
            if (_heightNoiseMat == null)
            {
                _heightNoiseMat = new Material(Shader.Find("Hidden/MegaWorld/HeightNoise"));
            }

            return _heightNoiseMat;
        }

        public override void Eval(MaskFilterContext maskFilterContext, int index)
        {
            CreateNoiseSettingsIfNecessary();

            Vector3 brushPosWs = maskFilterContext.BrushPos;
            var brushSize = maskFilterContext.BoxArea.BoxSize;
            var brushRotation = maskFilterContext.BoxArea.Rotation;

            // TODO(wyatt): remove magic number and tie it into NoiseSettingsGUI preview size somehow
            var previewSize = 1 / 512f;

            // get proper noise material from current noise settings
            Material mat = NoiseUtils.GetDefaultBlitMaterial(NoiseSettings);

            // setup the noise material with values in noise settings
            NoiseSettings.SetupMaterial(mat);

            // convert brushRotation to radians
            brushRotation *= Mathf.PI / 180;

            // change pos and scale so they match the noiseSettings preview
            var isWorldSpace = true;
            //brushSize = isWorldSpace ? brushSize * previewSize : 1;
            brushSize = brushSize * previewSize;
            brushPosWs = isWorldSpace ? brushPosWs * previewSize : Vector3.zero;

            // // override noise transform
            var rotQ = Quaternion.AngleAxis(-brushRotation, Vector3.up);
            var translation = Matrix4x4.Translate(brushPosWs);
            var rotation = Matrix4x4.Rotate(rotQ);
            var scale = Matrix4x4.Scale(Vector3.one * brushSize);
            Matrix4x4 noiseToWorld = translation * scale;

            mat.SetMatrix(NoiseSettings.ShaderStrings.Transform, NoiseSettings.trs * noiseToWorld);

            var pass = NoiseUtils.KNumBlitPasses * NoiseLib.GetNoiseIndex(NoiseSettings.DomainSettings.NoiseTypeName);

            var desc = new RenderTextureDescriptor(maskFilterContext.DestinationRenderTexture.width,
                maskFilterContext.DestinationRenderTexture.height, RenderTextureFormat.RFloat);
            var rt = RenderTexture.GetTemporary(desc);

            Graphics.Blit(maskFilterContext.SourceRenderTexture, rt, mat, pass);

            Material matFinal = GetMaterial();

            matFinal.SetTexture("_NoiseTex", rt);
            matFinal.SetTexture("_BaseMaskTex", maskFilterContext.SourceRenderTexture);
            matFinal.SetTexture("_HeightTex", maskFilterContext.HeightContext.sourceRenderTexture);

            SetMaterial(matFinal, maskFilterContext, index);

            Graphics.Blit(maskFilterContext.SourceRenderTexture, maskFilterContext.DestinationRenderTexture, matFinal,
                0);

            RenderTexture.ReleaseTemporary(rt);
        }

        public void SetMaterial(Material cs, MaskFilterContext fc, int index)
        {
            Terrain terrain = fc.BoxArea.TerrainUnder;

            if (index == 0)
            {
                cs.SetInt("_BlendMode", (int)BlendMode.Multiply);
            }
            else
            {
                cs.SetInt("_BlendMode", (int)BlendMode);
            }

            cs.SetFloat("_MinHeight", MinHeight);
            cs.SetFloat("_MaxHeight", MaxHeight);

            Vector3 position = terrain.transform.position;
            cs.SetFloat("_ClampMinHeight", position.y);
            cs.SetFloat("_ClampMaxHeight", terrain.terrainData.size.y + position.y);

            cs.SetFloat("_MaxRangeNoise", MaxRangeNoise);
            cs.SetFloat("_MinRangeNoise", MinRangeNoise);
        }

        private void CreateNoiseSettingsIfNecessary()
        {
            if (NoiseSettings == null)
            {
                NoiseSettings = new NoiseSettings();
            }
        }
#endif
    }
}
