#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration.Attributes;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    [ResourceLoaderTemplateBaseType(typeof(PrefabResourceLoader))]
    public class PrefabResourceLoaderTemplate : ResourceLoaderTemplate
    {
        public string PrefabAddress;

        public override void Run()
        {
        }

        protected override void OnBuildFrom(Type loaderType)
        {
            PrefabAddressAttribute prefabAddressAttribute = loaderType.GetAttribute<PrefabAddressAttribute>();
            PrefabAddress = prefabAddressAttribute?.Address ?? string.Empty;
        }

        public override void Validate(List<string> issues)
        {
            if (issues == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(PrefabAddress))
            {
                issues.Add("Prefab address is empty.");
            }
        }
    }
}
#endif
