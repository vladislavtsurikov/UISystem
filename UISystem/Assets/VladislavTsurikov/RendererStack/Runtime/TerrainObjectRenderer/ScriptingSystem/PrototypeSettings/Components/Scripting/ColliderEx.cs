using UnityEngine;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.MonoBehaviour;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Scripting
{
    public static class ColliderEx
    {
        public static bool TryGetScript<T>(this Collider collider, out T script) where T : Script
        {
            script = null;

            var hierarchyTerrainObjectInstance =
                (HierarchyTerrainObjectInstance)collider.GetComponentInParent(typeof(HierarchyTerrainObjectInstance));

            if (hierarchyTerrainObjectInstance != null)
            {
                script = (T)hierarchyTerrainObjectInstance.TerrainObjectInstance.GetScript(typeof(T));
                if (script != null)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetScript<T>(this GameObject gameObject, out T script) where T : Script
        {
            script = null;

            var hierarchyTerrainObjectInstance =
                (HierarchyTerrainObjectInstance)gameObject.GetComponentInParent(typeof(HierarchyTerrainObjectInstance));

            if (hierarchyTerrainObjectInstance != null)
            {
                script = (T)hierarchyTerrainObjectInstance.TerrainObjectInstance.GetScript(typeof(T));
                if (script != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
