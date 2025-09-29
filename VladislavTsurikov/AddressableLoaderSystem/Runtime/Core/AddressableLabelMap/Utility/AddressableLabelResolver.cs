using System;
using System.Collections.Generic;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core.AddressableLabelMap
{
    public static class AddressableLabelResolver
    {
        public static string GetLabel(string address) =>
            AddressableLabelMapAsset.Instance.TryGetLabel(address, out var label) ? label : null;

        public static Dictionary<string, string> GetLabelsByType(Type type) =>
            AddressableLabelMapAsset.Instance.GetLabelsByType(type);
    }
}
