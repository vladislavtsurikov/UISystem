using System;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.Common.TerrainSystem
{
    public static class AllTerrainHelperTypes
    {
        public static readonly Type[] Types;

        static AllTerrainHelperTypes() => Types = AllTypesDerivedFrom<TerrainHelper>.Types;

        public static bool HasTerrainMonoBehaviourType(GameObject go)
        {
            if (go == null)
            {
                return false;
            }

            Behaviour[] list = go.GetComponents<Behaviour>();
            foreach (Behaviour component in list)
            {
                if (component == null)
                {
                    continue;
                }

                foreach (Type type in Types)
                {
                    Type terrainMonoBehaviourType = type.GetAttribute<TerrainHelperAttribute>().RequiredСomponent;

                    if (component.GetType() == terrainMonoBehaviourType)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static TerrainHelper CreateTerrainHelper(GameObject go)
        {
            if (go == null)
            {
                return null;
            }

            Behaviour[] list = go.GetComponents<Behaviour>();
            foreach (Behaviour component in list)
            {
                if (component == null)
                {
                    continue;
                }

                foreach (Type type in Types)
                {
                    Type terrainMonoBehaviourType = type.GetAttribute<TerrainHelperAttribute>().RequiredСomponent;

                    if (component.GetType().Name == terrainMonoBehaviourType.Name)
                    {
                        var terrainType = (TerrainHelper)Activator.CreateInstance(type);
                        terrainType.InternalInit(component);
                        return terrainType;
                    }
                }
            }

            return null;
        }
    }
}
