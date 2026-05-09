using System;
using System.Runtime.CompilerServices;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    internal readonly struct UIPresenterKey
    {
        public Type ParentType { get; }
        public int ParentRuntimeId { get; }
        public string InstanceKey { get; }
        public bool IsDynamic { get; }

        public string Id
        {
            get
            {
                if (IsDynamic)
                {
                    return $"{ParentRuntimeId}:{InstanceKey ?? string.Empty}";
                }

                string parentId = ParentType?.FullName ?? "0";
                return string.IsNullOrEmpty(InstanceKey)
                    ? parentId
                    : $"{parentId}:{InstanceKey}";
            }
        }

        private UIPresenterKey(Type parentType, int parentRuntimeId, string instanceKey, bool isDynamic)
        {
            ParentType = parentType;
            ParentRuntimeId = parentRuntimeId;
            InstanceKey = instanceKey;
            IsDynamic = isDynamic;
        }

        public static UIPresenterKey FromPresenter(UIPresenter presenter)
        {
            return IsDynamicChild(presenter)
                ? FromDynamicParent(presenter.Parent, presenter.InstanceKey)
                : FromParentType(presenter.Parent?.GetType(), presenter.InstanceKey);
        }

        public static UIPresenterKey FromParentType(Type parentType, string instanceKey = null)
        {
            return new UIPresenterKey(parentType, 0, instanceKey, false);
        }

        public static UIPresenterKey FromDynamicParent(UIPresenter parent, string instanceKey)
        {
            return new UIPresenterKey(null, parent != null ? RuntimeHelpers.GetHashCode(parent) : 0, instanceKey, true);
        }

        private static bool IsDynamicChild(UIPresenter presenter)
        {
            return presenter != null &&
                   Attribute.IsDefined(presenter.GetType(), typeof(DynamicUIPresenterChildAttribute), true);
        }
    }
}
