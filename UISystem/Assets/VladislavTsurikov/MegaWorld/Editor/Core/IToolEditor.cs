using System;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;

namespace VladislavTsurikov.MegaWorld.Editor.Core
{
    public interface IToolEditor
    {
        SelectionData SelectionData { get; }
        Type TargetType { get; }
    }
}
