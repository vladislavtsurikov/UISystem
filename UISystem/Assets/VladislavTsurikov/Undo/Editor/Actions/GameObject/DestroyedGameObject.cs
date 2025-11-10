#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Events;
using VladislavTsurikov.UnityUtility.Runtime;
using GameObjectUtility = VladislavTsurikov.UnityUtility.Runtime.GameObjectUtility;

namespace VladislavTsurikov.Undo.Editor.GameObject
{
    public class DestroyedGameObject : UndoRecord
    {
        private readonly List<DestroyedData> _destroyDataList = new();

        public DestroyedGameObject(UnityEngine.GameObject gameObject)
        {
            UnityEngine.GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);

            if (gameObject.transform.parent == null)
            {
                _destroyDataList.Add(new DestroyedData(prefabRoot, null));
            }
            else
            {
                _destroyDataList.Add(new DestroyedData(prefabRoot, prefabRoot.transform.parent.gameObject));
            }
        }

        public static event UnityAction<List<UnityEngine.GameObject>> UndoPerformed;

        public override void Merge(UndoRecord record)
        {
            if (record is DestroyedGameObject gameObjectUndo)
            {
                _destroyDataList.AddRange(gameObjectUndo._destroyDataList);
            }
        }

        public override void Undo()
        {
            var gameObjectList = new List<UnityEngine.GameObject>();

            foreach (DestroyedData destroyData in _destroyDataList)
            {
                if (destroyData.Prefab != null)
                {
                    gameObjectList.Add(destroyData.Instantiate());
                }
            }

            UndoPerformed?.Invoke(gameObjectList);
        }

        private class DestroyedData
        {
            private readonly Instance _instance;
            private readonly UnityEngine.GameObject _parent;

            public readonly UnityEngine.GameObject Prefab;

            public DestroyedData(UnityEngine.GameObject gameObject, UnityEngine.GameObject parent)
            {
                _parent = parent;

                Prefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
                _instance = new Instance(gameObject);
            }

            public UnityEngine.GameObject Instantiate()
            {
                UnityEngine.GameObject go;

                go = PrefabUtility.InstantiatePrefab(Prefab) as UnityEngine.GameObject;

                go.transform.position = _instance.Position;
                go.transform.localScale = _instance.Scale;
                go.transform.rotation = _instance.Rotation;

                GameObjectUtility.ParentGameObject(go, _parent);

                return go;
            }
        }
    }
}
#endif
