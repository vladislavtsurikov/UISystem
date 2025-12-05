#if UNITY_EDITOR
using System;
using System.Linq;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.TemplatesSystem
{
    public class Template
    {
        public void Apply(Type[] supportedResourceTypes, Runtime.Core.SelectionDatas.Group.Group group, Prototype proto)
        {
            if (supportedResourceTypes.Contains(group.PrototypeType))
            {
                Apply(group);
                Apply(proto);
            }
        }

        protected virtual void Apply(Runtime.Core.SelectionDatas.Group.Group group)
        {
        }

        protected virtual void Apply(Prototype proto)
        {
        }
    }
}
#endif
