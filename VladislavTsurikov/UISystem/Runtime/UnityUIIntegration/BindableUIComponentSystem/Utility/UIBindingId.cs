using System;

namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration.Utility
{
    public static class UIBindingId
    {
        public static string FromTypeAndIndex(Type handlerType, string bindingId, int index = 0)
        {
            var handlerName = handlerType.Name;

            return $"{handlerName}:{bindingId}#{index}";
        }
    }
}
