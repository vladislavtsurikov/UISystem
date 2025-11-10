using VladislavTsurikov.AutoDefines.Editor.Core;

namespace VladislavTsurikov.AutoDefines.Editor
{
    public abstract class StaticDefineRule : AutoDefineRule
    {
        public override void Run()
        {
            DefineSymbolsBatcher.ApplyDefine(GetDefineToApplySymbol(), true);
        }
    }
}
