using System;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    internal static class UIHandlerBindingId
    {
        public static string FromHandler(UIHandler handler) => handler.Parent?.GetType().FullName ?? "0";

        public static string FromParentType(Type parentType) => parentType?.FullName ?? "0";
    }
}
