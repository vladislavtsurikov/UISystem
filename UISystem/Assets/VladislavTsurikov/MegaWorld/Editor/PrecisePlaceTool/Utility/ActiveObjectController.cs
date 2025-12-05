#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.Undo.Editor.GameObject;
using VladislavTsurikov.Undo.Editor.TerrainObjectRenderer;
using PrototypeTerrainObject =
    VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject.
    PrototypeTerrainObject;
#if RENDERER_STACK
#endif

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool
{
    public static class ActiveObjectController
    {
        private static PlacedObjectData _placedObjectData;

        public static PlacedObjectData PlacedObjectData
        {
            get
            {
                if (_placedObjectData == null || _placedObjectData.GameObject == null)
                {
                    return null;
                }

                return _placedObjectData;
            }
            set
            {
                if (value == null || value.GameObject == null)
                {
                    return;
                }

                if (_placedObjectData != null && _placedObjectData.GameObject != null)
                {
                    var settings = (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool),
                        typeof(PrecisePlaceToolSettings));

                    settings.MouseActionStack.End();

                    if (_placedObjectData.Proto.GetType() == typeof(PrototypeGameObject))
                    {
                        GameObjectCollider.Editor.GameObjectCollider.RegisterGameObjectToCurrentScene?.Invoke(
                            _placedObjectData.GameObject);
                        Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedGameObject(_placedObjectData.GameObject));
                        _placedObjectData = value;
                    }
#if RENDERER_STACK
                    else if (_placedObjectData.Proto.GetType() == typeof(PrototypeTerrainObject))
                    {
                        var prototypeTerrainObject = (PrototypeTerrainObject)_placedObjectData.Proto;

                        Transform transform = _placedObjectData.GameObject.transform;

                        TerrainObjectInstance terrainObjectInstance =
                            TerrainObjectRendererAPI.AddInstance(prototypeTerrainObject.RendererPrototype,
                                transform.position, transform.lossyScale, transform.rotation);
                        Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedTerrainObject(terrainObjectInstance));
                        Object.DestroyImmediate(_placedObjectData.GameObject);
                        _placedObjectData = value;
                    }
#endif
                }
                else
                {
                    _placedObjectData = value;
                }
            }
        }

        public static void DestroyObjectIfNecessary()
        {
            if (_placedObjectData == null)
            {
                return;
            }

            if (_placedObjectData.Proto.Selected == false)
            {
                DestroyObject();
            }
        }

        public static void DestroyObject()
        {
            if (_placedObjectData != null && _placedObjectData.GameObject != null)
            {
                Object.DestroyImmediate(_placedObjectData.GameObject);
            }

            _placedObjectData = null;
        }
    }
}
#endif
