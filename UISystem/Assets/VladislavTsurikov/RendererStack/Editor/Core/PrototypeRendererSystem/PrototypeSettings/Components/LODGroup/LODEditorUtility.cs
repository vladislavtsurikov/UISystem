using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Common.GlobalSettings;
using VladislavTsurikov.RendererStack.Runtime.Common.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.RenderModelData;
using LOD = VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.RenderModelData.LOD;

namespace VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem.PrototypeSettings.LODGroup
{
    internal static class LODEditorUtility
    {
        public static void DrawLODSettingsStack(Rect rect, Type rendererType, Prototype proto, RenderModel renderModel)
        {
            GUILayout.Space(2);

            var quality =
                (Quality)Runtime.Core.GlobalSettings.GlobalSettings.Instance.GetElement(typeof(Quality), rendererType);

            var lodGroup =
                (Runtime.Core.PrototypeRendererSystem.PrototypeSettings.LODGroup)proto.GetSettings(
                    typeof(Runtime.Core.PrototypeRendererSystem.PrototypeSettings.LODGroup));
            var distanceCulling = (DistanceCulling)proto.GetSettings(typeof(DistanceCulling));

            #region LODs reset and preparation to draw

            var lodBias = QualitySettings.lodBias * quality.LODBias * lodGroup.LODBias;

            var sliderRect = new Rect(rect.x, rect.y, rect.width, 44);

            List<LodguiInfo> infos = CreateLodguiInfo(rendererType, renderModel, sliderRect, lodBias);

            #endregion

            if (!distanceCulling.IsValid())
            {
                DrawLODLevelsSlider(GetDistanceMeasures(renderModel.LODs, lodBias), infos, sliderRect,
                    quality.MaxRenderDistance);
            }
            else
            {
                var maxProtoDistance = Mathf.Min(distanceCulling.MaxDistance, quality.MaxRenderDistance);

                DrawLODLevelsSlider(GetDistanceMeasures(renderModel.LODs, lodBias), infos, sliderRect,
                    quality.MaxRenderDistance, maxProtoDistance);
            }
        }

        public static List<LodguiInfo> CreateLodguiInfo(Type rendererType, RenderModel renderModel, Rect sliderRect,
            float lodBias)
        {
            var quality =
                (Quality)Runtime.Core.GlobalSettings.GlobalSettings.Instance.GetElement(typeof(Quality), rendererType);

            var infos = new List<LodguiInfo>();

            Rect buttonsRect = sliderRect;
            buttonsRect = new Rect(sliderRect.x, sliderRect.y, sliderRect.width, sliderRect.height - 14);

            for (var i = 0; i < renderModel.LODs.Count; i++)
            {
                var name = GetLODName(i, renderModel.LODs.Count);

                float percent = 1;

                if (i != renderModel.LODs.Count - 1)
                {
                    percent = renderModel.LODs[i + 1].Distance * lodBias / quality.MaxRenderDistance;
                }

                var info = new LodguiInfo(i, name, percent);
                info.ButtonRect = CalcLODButton(buttonsRect, info.LODPercentage);

                var previousPerc = 0f;
                if (i != 0)
                {
                    previousPerc = infos[i - 1].LODPercentage;
                }

                info.RangeRect = CalcLODRange(buttonsRect, previousPerc, info.LODPercentage);

                infos.Add(info);
            }

            return infos;
        }

        public static void DrawLODLevelsSlider(float[] measure, List<LodguiInfo> lods, Rect lodRect, float maxDistance,
            float maxProtoDistance)
        {
            var sliderId = GUIUtility.GetControlID(_sSliderControlId, FocusType.Passive);
            Event evt = Event.current;

            if (evt.GetTypeForControl(sliderId) == EventType.Repaint)
            {
                DrawLODSlider(measure, lodRect, lods, maxDistance);
                DrawHiddenRange(lodRect, maxProtoDistance / maxDistance, maxProtoDistance);
            }
        }

        public static void DrawLODLevelsSlider(float[] measure, List<LodguiInfo> lods, Rect lodRect, float maxDistance)
        {
            var sliderId = GUIUtility.GetControlID(_sSliderControlId, FocusType.Passive);
            Event evt = Event.current;

            if (evt.GetTypeForControl(sliderId) == EventType.Repaint)
            {
                DrawLODSlider(measure, lodRect, lods, maxDistance);
            }
        }

        /// <summary>
        ///     Drawing slider for LOD level
        /// </summary>
        public static void DrawLODSlider(float[] measure, Rect area, IList<LodguiInfo> lodInfos, float maxDistance)
        {
            Rect areaA = area;

            areaA = new Rect(area.x, area.y, area.width, area.height - 14);

            // Drawing BG Rectangle
            Styles.LODSliderBgStyle.Draw(areaA, GUIContent.none, false, false, false, false);

            // Drawing LOD rectangle areas
            for (var i = 0; i < lodInfos.Count; i++)
            {
                LodguiInfo lodInfo = lodInfos[i];

                var startPercent = 0.0f;
                if (i != 0)
                {
                    startPercent = lodInfos[i - 1].LODPercentage;
                }

                DrawLODRange(lodInfo, startPercent, measure[i], maxDistance);
            }

            // Draw culled range as last one
            if (lodInfos == null)
            {
                return;
            }

            if (lodInfos.Count == 0)
            {
                return;
            }

            if (lodInfos[lodInfos.Count - 1] == null)
            {
            }
        }

        /// <summary>
        ///     Calculating rect for LOD button
        /// </summary>
        public static Rect CalcLODButton(Rect totalRect, float percentage)
        {
            var rectW = totalRect.width - 43;
            var startX = Mathf.Round(rectW * percentage);

            return new Rect(totalRect.x + startX - 5, totalRect.y, 10, totalRect.height);
        }

        /// <summary>
        ///     Calculating rect for drwing slot range box
        /// </summary>
        public static Rect CalcLODRange(Rect totalRect, float startPercent, float endPercent, bool hidden = false)
        {
            var rectW = totalRect.width;
            var startX = Mathf.Round(rectW * startPercent);

            var endX = Mathf.Round(rectW * endPercent);

            return new Rect(totalRect.x + startX, totalRect.y, endX - startX, totalRect.height - (hidden ? 14 : 0));
        }

        /// <summary>
        ///     Drawing LOD slider box
        /// </summary>
        private static void DrawLODRange(LodguiInfo currentLOD, float previousLODPercentage, float nextUnits,
            float maxDistance)
        {
            Color tempColor = GUI.backgroundColor;
            var startPercentageString = currentLOD.LODName + "\n";

            var preMeters = 0f;
            if (preMeters == 0f)
            {
                preMeters = Mathf.Lerp(0f, maxDistance, previousLODPercentage);
            }

            preMeters = (float)System.Math.Round(preMeters, 1);

            nextUnits = (float)System.Math.Round(nextUnits, 1);

            startPercentageString += preMeters.ToString();
            var content = new GUIContent(startPercentageString);

            GUI.backgroundColor = LODColors[currentLOD.LODIndex];
            GUI.backgroundColor *= 0.6f;
            Styles.LODSliderRangeStyle.Draw(currentLOD.RangeRect, GUIContent.none, false, false, false, false);
            Styles.LODSliderTextStyle.Draw(currentLOD.RangeRect, content, false, false, false, false);

            GUI.backgroundColor = tempColor;
        }

        private static void DrawHiddenRange(Rect totalRect, float cullStartPercent, float maxDist)
        {
            GetHiddenBox(totalRect);
            // Draw the range of a lod level on the slider
            Color preColor = GUI.color;
            Color preBgColor = GUI.backgroundColor;

            if (cullStartPercent > 0f)
            {
                GUI.color = SHiddenLODColor;
            }
            else
            {
                GUI.color = SCulledLODColor;
            }

            Styles.LODSliderRangeStyle.Draw(GetLookingAwayBox(totalRect, cullStartPercent), GUIContent.none, false,
                false, false, false);

            GUI.color = preColor;
            GUI.backgroundColor = preBgColor;

            // Draw some details for the current marker
            var startPercentageString = "Looking Away";
            var style = new GUIStyle(Styles.LODSliderTextStyle);
            style.alignment = TextAnchor.UpperLeft;
            style.padding = new RectOffset(4, 0, 1, 0);
            style.fontSize = 9;
            style.Draw(GetLookingAwayBox(totalRect, cullStartPercent), new GUIContent(startPercentageString), false,
                false, false, false);

            if (cullStartPercent > 0f && cullStartPercent != 1)
            {
                GUI.backgroundColor = SCulledLODColor * new Color(1.5f, 1.5f, 1.5f, 0.84f);
                GUI.color = SCulledLODColor * new Color(1.5f, 1.5f, 1.5f, 0.84f);
                Styles.LODSliderRangeStyle.Draw(GetHiddenBox(totalRect, cullStartPercent), GUIContent.none, false,
                    false, false, false);

                var culledStyle = new GUIStyle(Styles.LODSliderTextStyle);
                culledStyle.alignment = TextAnchor.UpperLeft;
                culledStyle.padding = new RectOffset(4, 0, 1, 0);
                culledStyle.fontSize = 9;

                GUI.color = preColor;
                GUI.backgroundColor = preBgColor;

                var startCulledString = "Culled > " + Mathf.Round(maxDist);
                culledStyle.Draw(GetHiddenBox(totalRect, cullStartPercent), new GUIContent(startCulledString), false,
                    false, false, false);
            }
        }

        public static Rect GetHiddenBox(Rect totalRect)
        {
            var rect = new Rect(totalRect.x, totalRect.y + totalRect.height - 14, totalRect.width, 14);
            return rect;
        }

        public static Rect GetHiddenBox(Rect totalRect, float cullFrom)
        {
            var rectW = totalRect.width;
            var startX = Mathf.Round(rectW * cullFrom);
            var endX = Mathf.Round(rectW);

            return new Rect(totalRect.x + startX, totalRect.y + totalRect.height - 14, endX - startX, 14);
        }

        public static Rect GetLookingAwayBox(Rect totalRect, float cullFrom)
        {
            var rectW = totalRect.width;
            var startX = Mathf.Round(rectW * cullFrom);
            ;

            return new Rect(totalRect.x, totalRect.y + totalRect.height - 14, startX, 14);
        }

        public static float[] GetDistanceMeasures(List<LOD> loDs, float bias)
        {
            var lods = new float[loDs.Count];
            for (var i = 0; i < loDs.Count; i++)
            {
                lods[i] = loDs[i].Distance * bias;
            }

            return lods;
        }

        public static string GetLODName(int i, int count)
        {
            var name = "LOD " + i;
            if (i == count)
            {
                name = "Farthest";
            }

            if (count <= 1)
            {
                name = "Active";
            }

            return name;
        }

        public class GUIStyles
        {
            public readonly GUIStyle LODSliderBgStyle = "LODSliderBG";
            public readonly GUIStyle LODSliderRangeStyle = "LODSliderRange";
            public readonly GUIStyle LODSliderTextStyle = "LODSliderText";
        }

        public class LodguiInfo
        {
            public readonly int LODIndex;
            public readonly string LODName;
            public readonly float LODPercentage;
            public Rect ButtonRect;
            public Rect RangeRect;

            public LodguiInfo(int lodLevel, string name, float percentage)
            {
                LODIndex = lodLevel;
                LODName = name;
                LODPercentage = percentage;
            }
        }

        #region Pre Defines

        private static readonly int _sSliderControlId = "LODSliderIDHash".GetHashCode();

        public static readonly Color[] LODColors =
        {
            new(0.4831376f, 0.6211768f, 0.0219608f, 1.0f), new(0.2792160f, 0.4078432f, 0.5835296f, 1.0f),
            new(0.2070592f, 0.5333336f, 0.6556864f, 1.0f), new(0.5333336f, 0.1600000f, 0.0282352f, 1.0f),
            new(0.3827448f, 0.2886272f, 0.5239216f, 1.0f), new(0.8000000f, 0.4423528f, 0.0000000f, 1.0f),
            new(0.4486272f, 0.4078432f, 0.0501960f, 1.0f), new(0.7749016f, 0.6368624f, 0.0250984f, 1.0f)
        };

        public static readonly Color SCulledLODColor = new(.4f, 0f, 0f, 1f);
        public static readonly Color SHiddenLODColor = new(.4f, .2f, .2f, .5f);

        private static GUIStyles _sStyles;

        public static GUIStyles Styles
        {
            get
            {
                if (_sStyles == null)
                {
                    _sStyles = new GUIStyles();
                }

                return _sStyles;
            }
        }

        #endregion
    }
}
