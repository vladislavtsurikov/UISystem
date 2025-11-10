using UnityEngine;
using Group = VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Group;

namespace VladislavTsurikov.MegaWorld.Runtime.TerrainSpawner
{
    public class TerrainMask : MonoBehaviour
    {
        public Group Group;
        public Texture2D Mask;

        public bool IsFit() => Group != null && Mask != null;
    }
}
