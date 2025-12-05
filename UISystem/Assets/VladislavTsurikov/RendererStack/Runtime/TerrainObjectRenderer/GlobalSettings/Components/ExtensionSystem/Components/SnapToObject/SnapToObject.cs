using UnityEngine;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.GlobalSettings.ExtensionSystem.SnapToObject
{
    [Name("Snap To Object")]
    public class SnapToObject : Extension
    {
        public LayerMask Layers;
        public float MaxRayDistance = 6500f;
        public float RaycastPositionOffset = 0;
        public float SpawnCheckOffset = 3000;

        public void Snap()
        {
            /*
            for (int persistentCellIndex = 0; persistentCellIndex < StorageTerrainCells.PersistentStoragePackage.CellList.Count; persistentCellIndex++)
            {
                Cell persistentCell = StorageTerrainCells.PersistentStoragePackage.CellList[persistentCellIndex];

                StorageTerrainCells.CellModifier.AddModifiedСell(persistentCell);

                for (int persistentInfoIndex = 0; persistentInfoIndex < persistentCell.ItemInfoList.Count; persistentInfoIndex++)
                {
                    for (int itemIndex = 0; itemIndex < persistentCell.ItemInfoList[persistentInfoIndex].InstanceDataList.Count; itemIndex++)
                    {
                        InstanceData persistentItem = persistentCell.ItemInfoList[persistentInfoIndex].InstanceDataList[itemIndex];

                        InstantPrototype proto = InstantRenderer.InstantPrototypesPackage.GetInstantItem(persistentCell.ItemInfoList[persistentInfoIndex].ID);

                        if(proto == null)
                        {
                            continue;
                        }

                        if(proto.Selected == false)
                        {
                            continue;
                        }

                        RaycastHit hitInfo;
                        Ray ray = new Ray(new Vector3(persistentItem.Position.x, persistentItem.Position.y + SpawnCheckOffset, persistentItem.Position.z),
                            Vector3.down);

                        if (Physics.Raycast(ray, out hitInfo, MaxRayDistance, Layers))
                        {
                            Vector3 finalPosition = new Vector3(hitInfo.point.x, hitInfo.point.y + RaycastPositionOffset, hitInfo.point.z);
                            persistentItem.Position = finalPosition;
                        }

                        persistentCell.ItemInfoList[persistentInfoIndex].InstanceDataList[itemIndex] = persistentItem;
                    }
                }
            }
            */
        }
    }
}
