using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility
{
    public static class MaskFilterUtility
    {
        private static Material _paintTextureMaterial;
        private static Material _blendModesMaterial;
        private static Material _blendMat;
        private static Material _brushPreviewMat;

        public static Material blendModesMaterial
        {
            get
            {
                if (_blendModesMaterial == null)
                {
                    _blendModesMaterial = new Material(Shader.Find("Hidden/MegaWorld/BlendModes"));
                }

                return _blendModesMaterial;
            }
        }

        public static Material GetPaintMaterial()
        {
            if (_paintTextureMaterial == null)
            {
                _paintTextureMaterial = new Material(Shader.Find("PaintTexture"));
            }

            return _paintTextureMaterial;
        }

        public static Material GetBlendMaterial()
        {
            if (_blendMat == null)
            {
                _blendMat = new Material(Shader.Find("Hidden/TerrainTools/BlendModes"));
            }

            return _blendMat;
        }

        public static Material GetBrushPreviewMaterial()
        {
            if (_brushPreviewMat == null)
            {
                _brushPreviewMat = new Material(Shader.Find("Hidden/MegaWorld/PaintMaterialBrushPreview"));
            }

            return _brushPreviewMat;
        }

        public static MaskFilterStack GetMaskFilterFromSelectedPrototype(SelectionData data)
        {
            if (data.SelectedData.HasOneSelectedPrototype())
            {
                var maskFilterComponentSettings =
                    (MaskFilterComponentSettings)data.SelectedData.SelectedPrototype.GetElement(
                        typeof(MaskFilterComponentSettings));

                return maskFilterComponentSettings.MaskFilterStack;
            }

            return null;
        }
    }
}
