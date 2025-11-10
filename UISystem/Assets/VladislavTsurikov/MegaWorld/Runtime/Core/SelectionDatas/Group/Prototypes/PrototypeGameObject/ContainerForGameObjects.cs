using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.DefaultComponentsSystem;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject
{
    public class ContainerForGameObjects : DefaultGroupComponent
    {
        private Dictionary<Scene, GameObject> _containerForGameObjects = new();

        public void ParentGameObject(GameObject gameObject)
        {
            _containerForGameObjects ??= new Dictionary<Scene, GameObject>();

            if (!_containerForGameObjects.ContainsKey(gameObject.scene) ||
                _containerForGameObjects[gameObject.scene] == null)
            {
                FindParentObject(gameObject.scene);
            }

            GameObjectUtility.ParentGameObject(gameObject, _containerForGameObjects[gameObject.scene]);
        }

        public void DestroyGameObjects()
        {
            if (_containerForGameObjects == null)
            {
                return;
            }

            foreach (GameObject gameObject in _containerForGameObjects.Values)
            {
                Object.DestroyImmediate(gameObject);
            }

            _containerForGameObjects.Clear();
        }

        private void FindParentObject(Scene scene)
        {
            var groupName = Group.name;

            _containerForGameObjects.Remove(scene);
            _containerForGameObjects.Add(scene, GameObjectUtility.FindParentGameObject(groupName, scene));
        }
    }
}
