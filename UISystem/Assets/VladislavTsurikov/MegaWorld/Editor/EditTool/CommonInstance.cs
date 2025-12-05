#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.GameObjectCollider.Editor;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool
{
    public class CommonInstance
    {
        public readonly object Obj;
        public readonly Type PrototypeType;

        public CommonInstance(Prototype proto, object obj)
        {
            Obj = obj;

            PrototypeType = proto.GetType();
        }

        public Vector3 Position
        {
            get
            {
                if (PrototypeType == typeof(PrototypeGameObject))
                {
                    var gameObject = (GameObject)Obj;
                    return gameObject.transform.position;
                }
#if RENDERER_STACK

                if (PrototypeType == typeof(PrototypeTerrainObject))
                {
                    var instantObject = (TerrainObjectInstance)Obj;

                    return instantObject.Position;
                }
#endif

                return Vector3.zero;
            }
            set
            {
                if (PrototypeType == typeof(PrototypeGameObject))
                {
                    var gameObject = (GameObject)Obj;
                    gameObject.transform.position = value;
                    GameObjectColliderUtility.HandleTransformChangesForAllScenes();
                }
#if RENDERER_STACK
                else if (PrototypeType == typeof(PrototypeTerrainObject))
                {
                    var instantObject = (TerrainObjectInstance)Obj;

                    instantObject.Position = value;
                }
#endif
            }
        }

        public Quaternion Rotation
        {
            get
            {
                if (PrototypeType == typeof(PrototypeGameObject))
                {
                    var gameObject = (GameObject)Obj;
                    return gameObject.transform.rotation;
                }
#if RENDERER_STACK

                if (PrototypeType == typeof(PrototypeTerrainObject))
                {
                    var instantObject = (TerrainObjectInstance)Obj;

                    return instantObject.Rotation;
                }
#endif

                return Quaternion.identity;
            }
            set
            {
                if (PrototypeType == typeof(PrototypeGameObject))
                {
                    var gameObject = (GameObject)Obj;
                    gameObject.transform.rotation = value;
                    GameObjectColliderUtility.HandleTransformChangesForAllScenes();
                }
#if RENDERER_STACK
                else if (PrototypeType == typeof(PrototypeTerrainObject))
                {
                    var instantObject = (TerrainObjectInstance)Obj;

                    instantObject.Rotation = value;
                }
#endif
            }
        }

        public Vector3 Scale
        {
            get
            {
                if (PrototypeType == typeof(PrototypeGameObject))
                {
                    var gameObject = (GameObject)Obj;
                    return gameObject.transform.localScale;
                }
#if RENDERER_STACK

                if (PrototypeType == typeof(PrototypeTerrainObject))
                {
                    var instantObject = (TerrainObjectInstance)Obj;

                    return instantObject.Scale;
                }
#endif

                return Vector3.zero;
            }
            set
            {
                if (PrototypeType == typeof(PrototypeGameObject))
                {
                    var gameObject = (GameObject)Obj;
                    gameObject.transform.localScale = value;
                    GameObjectColliderUtility.HandleTransformChangesForAllScenes();
                }
#if RENDERER_STACK
                else if (PrototypeType == typeof(PrototypeTerrainObject))
                {
                    var instantObject = (TerrainObjectInstance)Obj;

                    instantObject.Scale = value;
                }
#endif
            }
        }

        public void DestroyObject()
        {
            if (PrototypeType == typeof(PrototypeGameObject))
            {
                var gameObject = (GameObject)Obj;
                Object.DestroyImmediate(gameObject);
                GameObjectColliderUtility.RemoveNullObjectNodesForAllScenes();
            }
#if RENDERER_STACK
            else if (PrototypeType == typeof(PrototypeTerrainObject))
            {
                var instantObject = (TerrainObjectInstance)Obj;

                instantObject.Destroy();
            }
#endif
        }

        public bool IsValid() => Obj != null;
    }
}
#endif
