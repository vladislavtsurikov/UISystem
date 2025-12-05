using System;
using System.Collections.Generic;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas
{
    public abstract class ClipboardObject
    {
        protected readonly Type PrototypeType;
        protected readonly Type ToolType;

        protected ClipboardObject(Type prototypeType, Type toolType)
        {
            PrototypeType = prototypeType;
            ToolType = toolType;
        }

        public static T GetCurrentClipboardObject<T>(Type prototypeType, Type toolType, List<T> clipboards)
            where T : ClipboardObject
        {
            foreach (T clipboardPrototype in clipboards)
            {
                if (clipboardPrototype.PrototypeType == prototypeType && clipboardPrototype.ToolType == toolType)
                {
                    return clipboardPrototype;
                }
            }

            return null;
        }

        protected abstract void Copy(List<IHasElementStack> objects);
        protected abstract void ClipboardAction(List<IHasElementStack> objects, bool paste);
        protected abstract void ClipboardAction(List<IHasElementStack> objects, Type settingsType, bool paste);
    }
}
