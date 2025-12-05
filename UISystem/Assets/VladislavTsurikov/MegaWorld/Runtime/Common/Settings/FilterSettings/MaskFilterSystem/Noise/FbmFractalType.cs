using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise.API;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Shaders;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise
{
    /// <summary>
    ///     A FractalType implementation for Fractal Brownian Motion
    /// </summary>
    [Serializable]
    public class FbmFractalType : FractalType<FbmFractalType>
    {
        [SerializeField]
        private FbmFractalInput _mInput;

        public override FractalTypeDescriptor GetDescription() =>
            new()
            {
                Name = "Fbm",
                TemplatePath = MaskFilterShadersPath.Path + "/NoiseLib/Templates/FractalFbm.noisehlsltemplate",
                SupportedDimensions = NoiseDimensionFlags._1D | NoiseDimensionFlags._2D | NoiseDimensionFlags._3D,
                InputStructDefinition = new List<HlslInput>
                {
                    new() { Name = "octaves", floatValue = new HlslFloat(3f) },
                    new() { Name = "amplitude", floatValue = new HlslFloat(.5f) },
                    new() { Name = "persistence", floatValue = new HlslFloat(.5f) },
                    new() { Name = "frequency", floatValue = new HlslFloat(1) },
                    new() { Name = "lacunarity", floatValue = new HlslFloat(2) },
                    new() { Name = "warpIterations", floatValue = new HlslFloat(0) },
                    new() { Name = "warpStrength", floatValue = new HlslFloat(.5f) },
                    new() { Name = "warpOffsets", float4Value = new HlslFloat4(2.5f, 1.4f, 3.2f, 2.7f) }
                },
                AdditionalIncludePaths = new List<string>
                {
                    MaskFilterShadersPath.Path + "/NoiseLib/NoiseCommon.hlsl"
                }
            };

        public override string GetDefaultSerializedString()
        {
            var fbm = new FbmFractalInput();

            fbm.Reset();

            return ToSerializedString(fbm);
        }

        [Serializable]
        public struct FbmFractalInput
        {
            public Vector4 WarpOffsets;
            public Vector2 OctavesMinMax;
            public Vector2 AmplitudeMinMax;
            public Vector2 FrequencyMinMax;
            public Vector2 LacunarityMinMax;
            public Vector2 PersistenceMinMax;
            public Vector2 WarpIterationsMinMax;
            public Vector2 WarpStrengthMinMax;
            public float Octaves;
            public float Amplitude;
            public float Frequency;
            public float Persistence;
            public float Lacunarity;
            public float WarpIterations;
            public float WarpStrength;
            public bool WarpEnabled;
            public bool WarpExpanded;

            public void Reset()
            {
                Octaves = 3;
                Amplitude = 0.5f;
                Frequency = 1;
                Lacunarity = 2;
                Persistence = 0.5f;

                OctavesMinMax = new Vector2(0, 16);
                AmplitudeMinMax = new Vector2(0, 1);
                FrequencyMinMax = new Vector2(0, 2);
                LacunarityMinMax = new Vector2(0, 4);
                PersistenceMinMax = new Vector2(0, 1);

                WarpExpanded = true;
                WarpEnabled = false;
                WarpIterations = 1;
                WarpStrength = 0.5f;
                WarpOffsets = new Vector4(2.5f, 1.4f, 3.2f, 2.7f);

                WarpIterationsMinMax = new Vector2(0, 8);
                WarpStrengthMinMax = new Vector2(-2, 2);
            }
        }

#if UNITY_EDITOR

        public override string DoGUI(Rect rect, string serializedString)
        {
            if (string.IsNullOrEmpty(serializedString))
            {
                serializedString = GetDefaultSerializedString();
            }

            // deserialize string
            var fbm = (FbmFractalInput)FromSerializedString(serializedString);

            // do gui here
            fbm.Octaves = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                Styles.Octaves, fbm.Octaves, fbm.OctavesMinMax.x, fbm.OctavesMinMax.y);
            rect.y += EditorGUIUtility.singleLineHeight;
            fbm.Amplitude = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                Styles.Amplitude, fbm.Amplitude, fbm.AmplitudeMinMax.x, fbm.AmplitudeMinMax.y);
            rect.y += EditorGUIUtility.singleLineHeight;
            fbm.Persistence = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                Styles.Persistence, fbm.Persistence, fbm.PersistenceMinMax.x, fbm.PersistenceMinMax.y);
            rect.y += EditorGUIUtility.singleLineHeight;
            fbm.Frequency = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                Styles.Frequency, fbm.Frequency, fbm.FrequencyMinMax.x, fbm.FrequencyMinMax.y);
            rect.y += EditorGUIUtility.singleLineHeight;
            fbm.Lacunarity = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                Styles.Lacunarity, fbm.Lacunarity, fbm.LacunarityMinMax.x, fbm.LacunarityMinMax.y);
            rect.y += EditorGUIUtility.singleLineHeight;

            //bool toggled = fbm.warpEnabled;

            fbm.WarpExpanded =
                EditorGUI.Foldout(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    fbm.WarpExpanded, Styles.DomainWarpSettings);
            if (fbm.WarpExpanded)
            {
                EditorGUI.indentLevel++;
                {
                    rect.y += EditorGUIUtility.singleLineHeight;
                    DomainWarpSettingsGUI(ref fbm, rect);
                }
                EditorGUI.indentLevel--;
            }

            //fbm.warpEnabled = toggled;

            return ToSerializedString(fbm);
        }

        private void DomainWarpSettingsGUI(ref FbmFractalInput fbm)
        {
            using (new EditorGUI.DisabledScope(!fbm.WarpEnabled))
            {
                fbm.WarpIterations = EditorGUILayout.Slider(Styles.WarpIterations, fbm.WarpIterations,
                    fbm.WarpIterationsMinMax.x, fbm.WarpIterationsMinMax.y);
                fbm.WarpStrength = EditorGUILayout.Slider(Styles.WarpStrength, fbm.WarpStrength,
                    fbm.WarpStrengthMinMax.x, fbm.WarpStrengthMinMax.y);
                fbm.WarpOffsets = EditorGUILayout.Vector4Field(Styles.WarpOffsets, fbm.WarpOffsets);
            }
        }

        private void DomainWarpSettingsGUI(ref FbmFractalInput fbm, Rect rect)
        {
            fbm.WarpEnabled = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                Styles.WarpEnabled, fbm.WarpEnabled);
            rect.y += EditorGUIUtility.singleLineHeight;
            using (new EditorGUI.DisabledScope(!fbm.WarpEnabled))
            {
                fbm.WarpIterations =
                    EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        Styles.WarpIterations, fbm.WarpIterations, fbm.WarpIterationsMinMax.x,
                        fbm.WarpIterationsMinMax.y);
                rect.y += EditorGUIUtility.singleLineHeight;
                fbm.WarpStrength =
                    EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        Styles.WarpStrength, fbm.WarpStrength, fbm.WarpStrengthMinMax.x, fbm.WarpStrengthMinMax.y);
                rect.y += EditorGUIUtility.singleLineHeight;
                fbm.WarpOffsets =
                    EditorGUI.Vector4Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        Styles.WarpOffsets, fbm.WarpOffsets);
                rect.y += EditorGUIUtility.singleLineHeight;
            }
        }

        public override void SetupMaterial(Material mat, string serializedString)
        {
            if (string.IsNullOrEmpty(serializedString))
            {
                serializedString = GetDefaultSerializedString();
            }

            var fbm = (FbmFractalInput)FromSerializedString(serializedString);

            // set noise domain values
            mat.SetFloat("_FbmOctaves", fbm.Octaves);
            mat.SetFloat("_FbmAmplitude", fbm.Amplitude);
            mat.SetFloat("_FbmFrequency", fbm.Frequency);
            mat.SetFloat("_FbmPersistence", fbm.Persistence);
            mat.SetFloat("_FbmLacunarity", fbm.Lacunarity);

            // warp values
            mat.SetFloat("_FbmWarpIterations", fbm.WarpEnabled ? fbm.WarpIterations : 0);
            mat.SetFloat("_FbmWarpStrength", fbm.WarpStrength);
            mat.SetVector("_FbmWarpOffsets", fbm.WarpOffsets);
        }

        public override string ToSerializedString(object target)
        {
            if (target == null)
            {
                return null;
            }

            if (!(target is FbmFractalInput))
            {
                Debug.LogError($"Attempting to serialize an object that is not of type {typeof(FbmFractalInput)}");
                return null;
            }

            var fbm = (FbmFractalInput)target;

            var serializedString = JsonUtility.ToJson(fbm);

            return serializedString;
        }

        public override object FromSerializedString(string serializedString)
        {
            if (string.IsNullOrEmpty(serializedString))
            {
                serializedString = GetDefaultSerializedString();
            }

            // TODO(wyatt): do validation/upgrading here

            FbmFractalInput target = JsonUtility.FromJson<FbmFractalInput>(serializedString);

            return target;
        }

        private static class Styles
        {
            public static readonly GUIContent WarpEnabled =
                EditorGUIUtility.TrTextContent("Enabled", "Turn on/off warp");

            public static readonly GUIContent WarpStrength =
                EditorGUIUtility.TrTextContent("Strength", "The overall strength of the warping effect");

            public static readonly GUIContent WarpIterations =
                EditorGUIUtility.TrTextContent("Iterations", "The number of warping iterations applied to the Noise");

            public static readonly GUIContent WarpOffsets =
                EditorGUIUtility.TrTextContent("Offset", "The offset direction to be used when warping the Noise");

            public static readonly GUIContent DomainWarpSettings =
                EditorGUIUtility.TrTextContent("Warp Settings", "Settings for applying turbulence to the Noise");

            public static readonly GUIContent Octaves = EditorGUIUtility.TrTextContent("Octaves",
                "The number of Octaves of Noise to generate. Each Octave is generally a smaller scale than the previous Octave and a larger scale than the next");

            public static readonly GUIContent Amplitude = EditorGUIUtility.TrTextContent("Amplitude",
                "The unmodified amplitude applied to each Octave of Noise. At each Octave, the amplitude is multiplied by the Persistence");

            public static readonly GUIContent Persistence = EditorGUIUtility.TrTextContent("Persistence",
                "The scaling factor applied to the Noise Amplitude at each Octave");

            public static readonly GUIContent Frequency = EditorGUIUtility.TrTextContent("Frequency",
                "The unmodified frequency of Noise at each Octave. At each Octave, the Frequency is multiplied by the Lacunarity");

            public static readonly GUIContent Lacunarity = EditorGUIUtility.TrTextContent("Lacunarity",
                "The scaling factor applied to the Noise Frequency at each Octave");
        }
#endif
    }
}
