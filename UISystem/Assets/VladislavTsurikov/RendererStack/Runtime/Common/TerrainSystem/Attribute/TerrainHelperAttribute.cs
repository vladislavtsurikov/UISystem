using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameObjectUtility = VladislavTsurikov.UnityUtility.Runtime.GameObjectUtility;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.RendererStack.Runtime.Common.TerrainSystem
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TerrainHelperAttribute : Attribute
    {
        public readonly string Name;
        public readonly Type RequiredСomponent;

        internal TerrainHelperAttribute(Type settingsType, string name)
        {
            RequiredСomponent = settingsType;
            Name = name;
        }

        public List<GameObject> GetTerrains(Scene scene)
        {
            List<Object> terrains = GameObjectUtility.FindObjectsOfType(RequiredСomponent, scene);

            var terrainList = new List<GameObject>();

            for (var i = 0; i < terrains.Count; i++)
            {
                var component = (Behaviour)terrains[i];
                terrainList.Add(component.gameObject);
            }

            return terrainList;
        }
    }
}
