using System;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.UnityUtility.Runtime;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes
{
    public abstract class PlacedObjectPrototype : Prototype
    {
        [NonSerialized]
        public Vector3 Extents = Vector3.one;

        [NonSerialized]
        public PastTransform PastTransform;

        public GameObject Prefab;

        public override string Name
        {
            get
            {
                if (Prefab != null)
                {
                    return Prefab.name;
                }

                return "Missing Prefab";
            }
        }

        public override Object PrototypeObject => Prefab;

#if UNITY_EDITOR
        public override Texture2D PreviewTexture
        {
            get
            {
                if (Prefab != null)
                {
                    return TextureUtility.GetPrefabPreviewTexture(Prefab);
                }

                return null;
            }
        }
#endif

        public override void OnCreatePrototype(Object obj)
        {
            Prefab = (GameObject)obj;
            PastTransform = new PastTransform(Prefab.transform);
            Extents = GameObjectUtility.CalculateBoundsInstantiate(Prefab).extents;
        }
    }
}
