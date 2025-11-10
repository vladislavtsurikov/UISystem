#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Core.Runtime;

namespace VladislavTsurikov.ShaderVariantCollection.Editor
{
    public static class ShaderVariantCollectionPath
    {
        private static UnityEngine.ShaderVariantCollection _shaderVariantCollection;

        public static UnityEngine.ShaderVariantCollection ShaderVariantCollection
        {
            get
            {
                if (_shaderVariantCollection == null)
                {
                    _shaderVariantCollection = GetPackage();
                }

                return _shaderVariantCollection;
            }
        }

        private static UnityEngine.ShaderVariantCollection GetPackage()
        {
            UnityEngine.ShaderVariantCollection shaderVariantCollection =
                Resources.Load<UnityEngine.ShaderVariantCollection>(CommonPath.CombinePath(CommonPath.Publisher,
                    CommonPath.ShaderVariantCollectionName));

            if (shaderVariantCollection == null)
            {
                shaderVariantCollection = new UnityEngine.ShaderVariantCollection();

                if (!Application.isPlaying)
                {
                    if (!Directory.Exists(CommonPath.PathToResources))
                    {
                        Directory.CreateDirectory(CommonPath.PathToResources);
                    }

                    AssetDatabase.CreateAsset(shaderVariantCollection,
                        CommonPath.PathToShaderVariantCollection + ".shadervariants");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }

            return shaderVariantCollection;
        }
    }
}
#endif
