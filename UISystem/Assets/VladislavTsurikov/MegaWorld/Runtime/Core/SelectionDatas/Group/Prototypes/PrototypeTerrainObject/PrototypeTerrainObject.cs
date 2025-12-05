using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Runtime.ElementStack.IconStack;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.DefaultComponentsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;
using Object = UnityEngine.Object;
#if RENDERER_STACK
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject
{
    [Name("Terrain Object")]
    [DropObjects(new[] { typeof(GameObject) })]
    [MissingIconsWarning("Drag & Drop Prefabs Here")]
    [AddDefaultGroupComponents(new[] { typeof(ContainerForGameObjects) })]
    public class PrototypeTerrainObject : PlacedObjectPrototype
    {
        public override bool IsSamePrototypeObject(Object obj)
        {
            var go = (GameObject)obj;

            return GameObjectUtility.IsSameGameObject(go, Prefab);
        }
#if RENDERER_STACK
        public RendererStack.Runtime.TerrainObjectRenderer.PrototypeTerrainObject RendererPrototype =>
            (RendererStack.Runtime.TerrainObjectRenderer.PrototypeTerrainObject)TerrainObjectRendererAPI
                .AddMissingPrototype(Prefab);

        public int RendererPrototypeID
        {
            get
            {
                var rendererPrototype =
                    (RendererStack.Runtime.TerrainObjectRenderer.PrototypeTerrainObject)PrototypesStorage.Instance
                        .GetPrototype(Prefab, typeof(TerrainObjectRenderer));

                if (rendererPrototype == null)
                {
                    return -1;
                }

                return rendererPrototype.ID;
            }
        }


        public override void SetupPrototype() => AllAvailableTerrainObjectPrototypes.AddPrototype(this);

        public override void OnDisablePrototype() => AllAvailableTerrainObjectPrototypes.RemovePrototype(this);
#endif
    }
}
