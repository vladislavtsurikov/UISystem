using System;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DrawersSelectionDatasAttribute : Attribute
    {
        public readonly Type SelectionGroupWindowType;
        public readonly Type SelectionPrototypeWindowType;

        public DrawersSelectionDatasAttribute(Type selectionPrototypeWindowType, Type selectionGroupWindowType)
        {
            SelectionPrototypeWindowType = selectionPrototypeWindowType;
            SelectionGroupWindowType = selectionGroupWindowType;
        }

        public DrawersSelectionDatasAttribute(Type selectionPrototypeWindowType) =>
            SelectionPrototypeWindowType = selectionPrototypeWindowType;
    }
}
