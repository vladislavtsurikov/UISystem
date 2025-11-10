#if UNITY_EDITOR
using System.Collections.Generic;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using Instance = VladislavTsurikov.UnityUtility.Runtime.Instance;

namespace VladislavTsurikov.Undo.Editor.TerrainObjectRenderer
{
    public class TerrainObjectTransform : UndoRecord
    {
        private readonly List<TransformData> _transformList = new();

        public TerrainObjectTransform(TerrainObjectInstance gameObject) =>
            _transformList.Add(new TransformData(gameObject));

        public override void Merge(UndoRecord record)
        {
            if (record is TerrainObjectTransform gameObjectUndo)
            {
                _transformList.AddRange(gameObjectUndo._transformList);
            }
        }

        public override void Undo()
        {
            foreach (TransformData transformUndoData in _transformList)
            {
                transformUndoData?.SetTransform();
            }
        }

        private class TransformData
        {
            private readonly TerrainObjectInstance _gameObject;
            private readonly Instance _instance;

            public TransformData(TerrainObjectInstance gameObject)
            {
                _gameObject = gameObject;
                _instance = new Instance(gameObject.Position, gameObject.Scale, gameObject.Rotation);
            }

            public void SetTransform()
            {
                if (_gameObject == null || _gameObject.IsDestroy)
                {
                    return;
                }

                _gameObject.Position = _instance.Position;
                _gameObject.Scale = _instance.Scale;
                _gameObject.Rotation = _instance.Rotation;
            }
        }
    }
}
#endif
