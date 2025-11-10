#if UNITY_EDITOR
using System.Collections.Generic;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;

namespace VladislavTsurikov.Undo.Editor.TerrainObjectRenderer
{
    public class DestroyedTerrainObject : UndoRecord
    {
        private readonly List<DestroyedData> _destroyDataList = new();

        public DestroyedTerrainObject(TerrainObjectInstance gameObject) =>
            _destroyDataList.Add(new DestroyedData(gameObject));

        public override void Merge(UndoRecord record)
        {
            if (record is DestroyedTerrainObject gameObjectUndo)
            {
                _destroyDataList.AddRange(gameObjectUndo._destroyDataList);
            }
        }

        public override void Undo()
        {
            foreach (DestroyedData destroyData in _destroyDataList)
            {
                if (destroyData.Prefab != null)
                {
                    destroyData.Instantiate();
                }
            }
        }

        private class DestroyedData
        {
            private readonly TerrainObjectInstance _instance;

            public readonly UnityEngine.GameObject Prefab;

            public DestroyedData(TerrainObjectInstance gameObject)
            {
                Prefab = gameObject.Proto.Prefab;
                _instance = gameObject;
            }

            public void Instantiate() =>
                TerrainObjectRendererAPI.AddInstance(Prefab, _instance.Position, _instance.Scale, _instance.Rotation);
        }
    }
}
#endif
