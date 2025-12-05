using System;
using System.Collections.Generic;
using OdinSerializer;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;

namespace VladislavTsurikov.RendererStack.Runtime.Common.TerrainSystem
{
    [Serializable]
    public class TerrainManager : SceneData
    {
        public delegate void ChangedTerrainCount(SceneDataManager sceneDataManager);

        public static ChangedTerrainCount ChangedTerrainCountEvent;
        public Bounds Bounds;

        [OdinSerialize]
        public List<GameObject> TerrainGameObjectList = new();

        [OdinSerialize]
        public List<TerrainHelper> TerrainHelperList = new();

        protected override void SetupSceneData()
        {
            ChangedTerrainCountEvent -= NewTerrainAction;
            ChangedTerrainCountEvent += NewTerrainAction;

            FindAll();

            if (RefreshTerrainHelper())
            {
                ChangedTerrainCountEvent(SceneDataManager);
            }
        }

        protected override void OnDisableElement()
        {
            foreach (TerrainHelper item in TerrainHelperList)
            {
                item?.OnDisable();
            }
        }

        public void FindAll()
        {
            foreach (Type type in AllTerrainHelperTypes.Types)
            {
                TerrainHelperAttribute terrainHelperAttribute = type.GetAttribute<TerrainHelperAttribute>();

                AddTerrains(terrainHelperAttribute.GetTerrains(SceneDataManager.Scene));
            }
        }

        public void AddTerrain(GameObject go) => AddTerrains(new List<GameObject> { go });

        public void AddTerrains(List<GameObject> terrainList)
        {
            var refreshTerrains = false;

            for (var i = 0; i <= terrainList.Count - 1; i++)
            {
                if (AllTerrainHelperTypes.HasTerrainMonoBehaviourType(terrainList[i]))
                {
                    if (!TerrainGameObjectList.Contains(terrainList[i]))
                    {
                        TerrainGameObjectList.Add(terrainList[i]);
                        refreshTerrains = true;
                    }
                }
            }

            if (refreshTerrains)
            {
                RefreshTerrainHelper();

                ChangedTerrainCountEvent(SceneDataManager);
            }
        }

        public void RemoveTerrains(List<GameObject> terrainList)
        {
            if (TerrainGameObjectList.RemoveAll(terrainList.Contains) != 0)
            {
                RefreshTerrainHelper();
                ChangedTerrainCountEvent(SceneDataManager);
            }
        }

        public void CalculateBounds()
        {
            var newBounds = new Bounds(Vector3.zero, Vector3.zero);
            for (var i = 0; i < TerrainHelperList.Count; i++)
            {
                TerrainHelper terrain = TerrainHelperList[i];
                if (terrain != null)
                {
                    if (i == 0)
                    {
                        newBounds = terrain.GetTerrainBounds();
                    }
                    else
                    {
                        newBounds.Encapsulate(terrain.GetTerrainBounds());
                    }
                }
            }

            Bounds = newBounds;
        }

        public void RefreshTerrainHelperData()
        {
            foreach (TerrainHelper terrainHelper in TerrainHelperList)
            {
                if (terrainHelper != null)
                {
                    terrainHelper.RefreshData();
                }
            }
        }

        private bool DeleteInvalidTerrainData()
        {
            var removeTerrains = new List<GameObject>();
            foreach (GameObject gameObject in TerrainGameObjectList)
            {
                if (gameObject == null || gameObject.scene != SceneDataManager.Scene ||
                    !AllTerrainHelperTypes.HasTerrainMonoBehaviourType(gameObject))
                {
                    removeTerrains.Add(gameObject);
                }
            }

            foreach (GameObject terrain in removeTerrains)
            {
                TerrainGameObjectList.Remove(terrain);
            }

            if (removeTerrains.Count != 0)
            {
                return true;
            }

            return false;
        }

        private bool RefreshTerrainHelper()
        {
            var happenedDelete = DeleteInvalidTerrainData();
            TerrainHelperList.Clear();

            foreach (GameObject go in TerrainGameObjectList)
            {
                TerrainHelper terrainHelper = AllTerrainHelperTypes.CreateTerrainHelper(go);

                if (terrainHelper != null)
                {
                    TerrainHelperList.Add(terrainHelper);
                }
            }

            CalculateBounds();

            return happenedDelete;
        }

        public void NewTerrainAction(SceneDataManager sceneDataManager)
        {
#if UNITY_EDITOR
            SceneDataStackUtility.Setup<GameObjectCollider.Editor.GameObjectCollider>(true);
#endif
        }

#if UNITY_EDITOR
        public void DrawHandles()
        {
            Handles.color = Color.yellow;
            Handles.DrawWireCube(Bounds.center, Bounds.size);

            Handles.color = Color.green;
            foreach (TerrainHelper terrainHelper in TerrainHelperList)
            {
                Bounds terrainBounds = terrainHelper.GetTerrainBounds();
                Handles.DrawWireCube(terrainBounds.center, terrainBounds.size);
            }
        }
#endif
    }
}
