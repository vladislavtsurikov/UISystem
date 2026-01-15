using System;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings
{
    [Name("Layer Settings")]
    public class LayerSettings : Node
    {
        public LayerMask PaintLayers = 1;

        public LayerMask GetCurrentPaintLayers(Type prototypeType)
        {
            if (prototypeType == typeof(PrototypeTerrainDetail) || prototypeType == typeof(PrototypeTerrainTexture))
            {
                if (Terrain.activeTerrain == null)
                {
                    Debug.LogWarning("Not present in the scene with an active Unity Terrain.");
                    return PaintLayers;
                }

                return LayerMask.GetMask(LayerMask.LayerToName(Terrain.activeTerrain.gameObject.layer));
            }

            return PaintLayers;
        }
    }
}
