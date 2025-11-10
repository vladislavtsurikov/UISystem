using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.Utility.Runtime;
using Area = VladislavTsurikov.MegaWorld.Runtime.Common.Stamper.Area;

namespace VladislavTsurikov.MegaWorld.Runtime.TextureStamperTool
{
    public class TextureStamperArea : Area
    {
        public List<Bounds> CellList = new();
        public float CellSize = 1000;
        public CustomMasks CustomMasks = new();

        public bool ShowCells = true;

        public bool UseMask;
        public bool UseSpawnCells;

        public TextureStamper TextureStamper => (TextureStamper)StamperTool;

        protected override void SetupArea()
        {
#if UNITY_EDITOR
            OnSetAreaBounds -= () =>
                TextureStamper.StamperVisualisation.StamperMaskFilterVisualisation.NeedUpdateMask = true;
            OnSetAreaBounds += () =>
                TextureStamper.StamperVisualisation.StamperMaskFilterVisualisation.NeedUpdateMask = true;
#endif
            OnSetAreaBounds -= ClearCellList;
            OnSetAreaBounds += ClearCellList;
        }

        public void CreateCells()
        {
            CellList.Clear();

            var expandedBounds = new Bounds(Bounds.center, Bounds.size);
            expandedBounds.Expand(new Vector3(CellSize * 2f, 0, CellSize * 2f));

            var cellXCount = Mathf.CeilToInt(Bounds.size.x / CellSize);
            var cellZCount = Mathf.CeilToInt(Bounds.size.z / CellSize);

            var corner = new Vector2(Bounds.center.x - Bounds.size.x / 2f, Bounds.center.z - Bounds.size.z / 2f);

            for (var x = 0; x <= cellXCount - 1; x++)
            for (var z = 0; z <= cellZCount - 1; z++)
            {
                var rect = new Rect(
                    new Vector2(CellSize * x + corner.x, CellSize * z + corner.y),
                    new Vector2(CellSize, CellSize));

                Bounds bounds = RectExtension.CreateBoundsFromRect(rect, Bounds.center.y, Bounds.size.y);

                CellList.Add(bounds);
            }
        }

        private void ClearCellList()
        {
            if (UseSpawnCells)
            {
                CellList.Clear();
            }
        }

        public override Texture2D GetCurrentRaw()
        {
            if (UseMask == false || UseSpawnCells)
            {
                return Texture2D.whiteTexture;
            }

            return CustomMasks.GetSelectedBrush();
        }

        public BoxArea GetAreaVariablesFromSpawnCell(RayHit hit, Bounds bounds)
        {
            var area = new BoxArea(hit, bounds.size.x);

            return area;
        }
    }
}
