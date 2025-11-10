#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.Undo.Editor.GameObject
{
    public class GameObjectTransform : UndoRecord
    {
        private readonly List<TransformData> _transformList = new();

        public GameObjectTransform(UnityEngine.GameObject gameObject)
        {
            UnityEngine.GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);

            _transformList.Add(new TransformData(prefabRoot));
        }

        public override void Merge(UndoRecord record)
        {
            if (record is GameObjectTransform gameObjectUndo)
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
            private readonly UnityEngine.GameObject _gameObject;
            private readonly Instance _instance;

            public TransformData(UnityEngine.GameObject gameObject)
            {
                _gameObject = gameObject;
                _instance = new Instance(gameObject);
            }

            public void SetTransform()
            {
                if (_gameObject == null)
                {
                    return;
                }

                _gameObject.transform.position = _instance.Position;
                _gameObject.transform.localScale = _instance.Scale;
                _gameObject.transform.rotation = _instance.Rotation;
            }
        }
    }
}
#endif
