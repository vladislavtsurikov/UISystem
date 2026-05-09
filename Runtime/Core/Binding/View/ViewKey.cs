using System;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public readonly struct ViewKey
    {
        public Type ViewType { get; }
        public Type PresenterType { get; }
        public string BindingId { get; }
        public int Index { get; }
        public string InstanceKey { get; }

        public string Id
        {
            get
            {
                string presenterName = PresenterType.Name;

                return string.IsNullOrEmpty(InstanceKey)
                    ? $"{presenterName}:{BindingId}#{Index}"
                    : $"{presenterName}:{InstanceKey}:{BindingId}#{Index}";
            }
        }

        public ViewKey(
            Type viewType,
            Type presenterType,
            string bindingId,
            int index = 0,
            string instanceKey = null)
        {
            ViewType = viewType;
            PresenterType = presenterType;
            BindingId = bindingId;
            Index = index;
            InstanceKey = instanceKey;
        }
    }
}
