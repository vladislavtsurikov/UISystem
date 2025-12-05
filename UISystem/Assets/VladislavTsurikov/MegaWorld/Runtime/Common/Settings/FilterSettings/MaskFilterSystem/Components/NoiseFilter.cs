using System;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise.API;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    [Serializable]
    [Name("Noise")]
    public class NoiseFilter : MaskFilter
    {
        public BlendMode BlendMode = BlendMode.Multiply;

        public NoiseSettings NoiseSettings = new();

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

            Material blendMat = MaskFilterUtility.blendModesMaterial;

            if (index == 0)
            {
                blendMat.SetInt("_BlendMode", (int)BlendMode.Multiply);
            }
            else
            {
                blendMat.SetInt("_BlendMode", (int)BlendMode);
            }

            blendMat.SetTexture("_MainTex", maskFilterContext.SourceRenderTexture);
            blendMat.SetTexture("_BlendTex", rt);

            Graphics.Blit(maskFilterContext.SourceRenderTexture, maskFilterContext.DestinationRenderTexture, blendMat,
                0);

            RenderTexture.ReleaseTemporary(rt);
        }

        private void CreateNoiseSettingsIfNecessary()
        {
            if (NoiseSettings == null)
            {
                NoiseSettings = new NoiseSettings();
            }
        }
    }
}
