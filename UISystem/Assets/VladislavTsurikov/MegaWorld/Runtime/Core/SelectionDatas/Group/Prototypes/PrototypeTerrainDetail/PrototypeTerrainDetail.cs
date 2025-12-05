using System;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Runtime.ElementStack.IconStack;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail
{
    public enum PrefabType
    {
        Mesh = 0,
        Texture = 1
    }

    [Serializable]
    [Name("Unity/Terrain Detail")]
    [MissingIconsWarning("Drag & Drop Prefabs or Textures Here")]
    [DropObjects(new[] { typeof(Texture2D), typeof(GameObject) })]
    public class PrototypeTerrainDetail : Prototype
    {
        public int TerrainProtoId;

        public PrefabType PrefabType = PrefabType.Mesh;
        public Texture2D DetailTexture;
        public GameObject Prefab;

        public override string Name
        {
            get
            {
                if (PrefabType == PrefabType.Mesh)
                {
                    if (Prefab != null)
                    {
                        return Prefab.name;
                    }

                    return "Missing Prefab";
                }

                if (DetailTexture != null)
                {
                    return DetailTexture.name;
                }

                return "Missing Texture";
            }
        }

        public override Object PrototypeObject
        {
            get
            {
                if (PrefabType == PrefabType.Texture)
                {
                    return DetailTexture;
                }

                return Prefab;
            }
        }

#if UNITY_EDITOR
        public override Texture2D PreviewTexture
        {
            get
            {
                if (Prefab != null)
                {
                    return TextureUtility.GetPrefabPreviewTexture(Prefab);
                }

                return DetailTexture;
            }
        }
#endif

        public override void OnCreatePrototype(Object obj)
        {
            if (obj is GameObject gameObject)
            {
                PrefabType = PrefabType.Mesh;

                Prefab = gameObject;
            }
            else
            {
                PrefabType = PrefabType.Texture;

                DetailTexture = (Texture2D)obj;
            }
        }

        public override bool IsSamePrototypeObject(Object obj) => PrototypeObject == obj;
    }
}
