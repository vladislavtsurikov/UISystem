#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.GameObjectCollider.Editor;

namespace VladislavTsurikov.Undo.Editor.GameObject
{
    public class CreatedGameObject : UndoRecord
    {
        private readonly List<UnityEngine.GameObject> _gameObjectList = new();

        public CreatedGameObject(UnityEngine.GameObject gameObject)
        {
            UnityEngine.GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);

            _gameObjectList.Add(prefabRoot);
        }

        public override void Merge(UndoRecord record)
        {
            if (record is not CreatedGameObject gameObjectUndo)
            {
                return;
            }

            _gameObjectList.AddRange(gameObjectUndo._gameObjectList);
        }

        public override void Undo()
        {
            foreach (UnityEngine.GameObject gameObject in _gameObjectList)
            {
                Object.DestroyImmediate(gameObject);
            }

            GameObjectColliderUtility.RemoveNullObjectNodesForAllScenes();
        }
    }
}
#endif
