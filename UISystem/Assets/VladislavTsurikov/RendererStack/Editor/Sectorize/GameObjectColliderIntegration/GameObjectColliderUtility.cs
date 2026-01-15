#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;
using GameObjectUtility = VladislavTsurikov.UnityUtility.Runtime.GameObjectUtility;

namespace VladislavTsurikov.RendererStack.Editor.Sectorize.GameObjectColliderIntegration
{
    [InitializeOnLoad]
    public class GameObjectColliderUtility
    {
        static GameObjectColliderUtility() =>
            GameObjectCollider.Editor.GameObjectCollider.RegisterGameObjectToCurrentScene +=
                RegisterGameObjectToCurrentScene;

        public static void RegisterGameObjectToCurrentScene(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }

            ChangeGameObjectSceneIfNecessary(gameObject);

            List<SceneDataManager> sceneDataManagers =
                SceneDataManagerFinder.OverlapPosition(gameObject.transform.position,
                    Runtime.Sectorize.Sectorize.GetSectorLayerTag(), false);

            if (sceneDataManagers.Count == 0)
            {
                return;
            }

            SceneDataManager sceneDataManager = sceneDataManagers[0];

            var gameObjectCollider =
                (GameObjectCollider.Editor.GameObjectCollider)sceneDataManager.SceneDataStack.GetElement(
                    typeof(GameObjectCollider.Editor.GameObjectCollider));

            if (gameObjectCollider == null)
            {
                SceneDataStackUtility.InstanceSceneData<GameObjectCollider.Editor.GameObjectCollider>(sceneDataManager
                    .Scene);
            }
            else
            {
                gameObjectCollider.RegisterGameObjectWithChildren(gameObject);
            }
        }

        private static void ChangeGameObjectSceneIfNecessary(GameObject gameObject)
        {
            List<SceneDataManager> sceneDataManagers =
                SceneDataManagerFinder.OverlapPosition(gameObject.transform.position,
                    Runtime.Sectorize.Sectorize.GetSectorLayerTag(),
                    false);

            if (sceneDataManagers.Count <= 1)
            {
                return;
            }

            SceneDataManager sceneDataManager = sceneDataManagers[0];

            if (gameObject.scene != sceneDataManager.Scene)
            {
                GameObject prefabRoot = GameObjectUtility.GetPrefabRoot(gameObject);

                if (prefabRoot != null)
                {
                    if (prefabRoot.transform.parent != null)
                    {
                        var prefabParentName = prefabRoot.transform.parent.gameObject.name;

                        prefabRoot.transform.parent = null;

                        SceneManager.MoveGameObjectToScene(prefabRoot, sceneDataManager.Scene);

                        GameObject parent =
                            GameObjectUtility.FindParentGameObject(prefabParentName, sceneDataManager.Scene);

                        GameObjectUtility.ParentGameObject(prefabRoot, parent);
                    }
                    else
                    {
                        SceneManager.MoveGameObjectToScene(prefabRoot, sceneDataManager.Scene);
                    }
                }
                else
                {
                    SceneManager.MoveGameObjectToScene(gameObject, sceneDataManager.Scene);
                }
            }
        }
    }
}
#endif
